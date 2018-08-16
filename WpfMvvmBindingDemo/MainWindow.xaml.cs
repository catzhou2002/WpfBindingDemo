using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfMvvmBindingDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            VM vm = new VM();
            this.DataContext = vm;

            this.TextMobile.SetBinding(TextBlock.TextProperty, new Binding(nameof(vm.TextMobile)) { Mode = BindingMode.OneTime });
            this.TextVerifyCode.SetBinding(TextBlock.TextProperty, new Binding(nameof(vm.TextVerifyCode)) { Mode = BindingMode.OneTime });
            this.BGetVerifyCode.SetBinding(Button.ContentProperty, new Binding(nameof(vm.TextGetVerifyCode)) { Mode = BindingMode.OneWay });
            this.BGetVerifyCode.SetBinding(Button.IsEnabledProperty, new Binding(nameof(vm.EnableGetVerifyCode)) { Mode = BindingMode.OneWay });

            this.BGetVerifyCode.SetBinding(Button.CommandProperty, new Binding(nameof(vm.CommandGetVerifyCode)));
            this.BRegister.SetBinding(Button.ContentProperty, new Binding(nameof(vm.TextRegister)) { Mode = BindingMode.OneTime });

            this.BRegister.SetBinding(Button.CommandProperty, new Binding(nameof(vm.CommandRegister)));

            this.TextMessage.SetBinding(TextBlock.TextProperty, new Binding(nameof(vm.ValueMessage)) { Mode = BindingMode.OneWay });
            this.TextMessage.SetBinding(TextBlock.VisibilityProperty, new Binding(nameof(vm.VisibilityMessage)) { Mode = BindingMode.OneWay });

            this.TBMobile.SetBinding(TextBox.TextProperty, new Binding(nameof(vm.ValueMobile)) { Mode = BindingMode.OneWayToSource });
            this.TBVerifyCode.SetBinding(TextBox.TextProperty, new Binding(nameof(vm.ValueVerifyCode)) { Mode = BindingMode.OneWayToSource });

        }
    }
    public class VM : VMBase
    {
        /// <summary>
        /// 定时器
        /// </summary>
        private System.Timers.Timer _Timer = new System.Timers.Timer();
        /// <summary>
        /// 定时器计数
        /// </summary>
        private int _TimerOffset;
        /// <summary>
        /// 手机提示
        /// </summary>
        public string TextMobile
        {
            get => "手机号码";
        }

        private string _ValueMobile;
        /// <summary>
        /// 手机值
        /// </summary>
        public string ValueMobile
        {
            get => _ValueMessage;
            set
            {
                _ValueMobile = value;
                EnableGetVerifyCode = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// 验证码提示
        /// </summary>
        public string TextVerifyCode
        {
            get => "验证码";
        }
        /// <summary>
        /// 验证码值
        /// </summary>
        public string ValueVerifyCode { get; set; }





        /// <summary>
        /// 获取验证码命令
        /// </summary>
        public ICommand CommandGetVerifyCode { get; set; }

        /// <summary>
        /// 获取验证码提示
        /// </summary>
        private string _TextGetVerifyCode;
        public string TextGetVerifyCode
        {
            get => _TextGetVerifyCode;
            set
            {
                _TextGetVerifyCode = value;
                RaisePropertyChangedAuto();
            }
        }

        private bool _EnableGetVerifyCode;
        /// <summary>
        /// 获取验证码有效性
        /// </summary>
        public bool EnableGetVerifyCode
        {
            get => _EnableGetVerifyCode;
            set
            {
                _EnableGetVerifyCode = value;
                RaisePropertyChangedAuto();
            }
        }

        /// <summary>
        /// 注册提示
        /// </summary>
        public string TextRegister
        {
            get => "注册";
        }
        /// <summary>
        /// 注册命令
        /// </summary>
        public ICommand CommandRegister { get; set; }


        private string _ValueMessage;
        /// <summary>
        /// 注册结果提示语
        /// </summary>
        public string ValueMessage
        {
            get => _ValueMessage;
            set
            {
                _ValueMessage = value;
                if (string.IsNullOrEmpty(value))
                    VisibilityMessage = Visibility.Collapsed;
                else
                    VisibilityMessage = Visibility.Visible;

                RaisePropertyChangedAuto();
            }
        }
        private Visibility _VisibilityMessage;
        /// <summary>
        /// 注册结果提示语可见性
        /// </summary>
        public Visibility VisibilityMessage
        {
            get => _VisibilityMessage;
            set
            {
                _VisibilityMessage = value;
                RaisePropertyChangedAuto();
            }
        }


        public VM()
        {

            _VisibilityMessage = Visibility.Hidden;
            _TextGetVerifyCode = "获取验证码";
            EnableGetVerifyCode = true;
            _Timer.Interval = 1000;
            _Timer.Elapsed += _Timer_Elapsed;

            CommandGetVerifyCode = new DelegateCommand((o) =>
            {
                EnableGetVerifyCode = false;
                _TimerOffset = 60;
                _Timer.Start();

            }, null);

            CommandRegister = new DelegateCommand((o) =>
            {
                if (string.IsNullOrEmpty(ValueMobile))
                {
                    ValueMessage = "手机号码不能为空";
                    return;
                }
                if (ValueVerifyCode != "1234")
                {
                    ValueMessage = "验证码错误";
                    return;
                }
                ValueMessage = "注册成功";
            }, null);

        }

        private void _Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_TimerOffset > 0)
            {
                TextGetVerifyCode = _TimerOffset + "秒后可重新获取验证码";
                _TimerOffset--;
            }
            else
            {
                TextGetVerifyCode = "获取验证码";
                EnableGetVerifyCode = true;
                _Timer.Stop();
            }

        }
    }
    public class VMBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RaisePropertyChangedAuto([CallerMemberName]  string propertyName = "")
        {
            if (!string.IsNullOrEmpty(propertyName))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public class DelegateCommand : ICommand
    {

        public Action<object> ExecuteCommand
        { get; set; }


        public Predicate<object> CanExecuteCommand
        {
            get;
            set;
        }

        public DelegateCommand(Action<object> executeCommand, Predicate<object> canExecuteCommand)
        {
            this.ExecuteCommand = executeCommand;
            this.CanExecuteCommand = canExecuteCommand;
        }


        #region 接口
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (CanExecuteCommand != null)
            {
                return this.CanExecuteCommand(parameter);
            }
            else
            {
                return true;
            }
        }

        public void Execute(object parameter)
        {
            if (this.ExecuteCommand != null)
            {
                this.ExecuteCommand(parameter);
            }
        }

        #endregion



        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
