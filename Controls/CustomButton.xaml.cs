using MinecraftServerDownloader.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinecraftServerDownloader.Controls
{
    /// <summary>
    /// CustomButton.xaml 的交互逻辑
    /// </summary>
    public partial class CustomButton : Border
    {
        public CustomButton()
        {
            InitializeComponent();

            ButtonScale.CenterX = ButtonBorder.Width / 2;
            ButtonScale.CenterY = ButtonBorder.Height / 2;
        }

        #region 属性

        bool isClick = false;

        // 新建属性 Text
        string text;
        public static readonly DependencyProperty textProperty = 
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(CustomButton),
                new PropertyMetadata("Button"));
        public string Text
        {
            set
            {
                text = value;
                ButtonText.Text = value;
            }

            get
            {
                return text;
            }
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Border));
        public event RoutedEventHandler Click
        {
            add
            {
                AddHandler(ClickEvent, value);
            }
            remove
            {
                RemoveHandler(ClickEvent, value);
            }
        }

        #endregion

        #region 方法

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            isClick = true;
            ModAnimation.Animate(this, ScaleTransform.ScaleXProperty, 0.9, TimeSpan.FromMilliseconds(200));
            ModAnimation.Animate(this, ScaleTransform.ScaleYProperty, 0.9, TimeSpan.FromMilliseconds(200));
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            ModAnimation.Animate(this, BackgroundProperty, Colors.LightGray, TimeSpan.FromMilliseconds(100));
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            isClick = false;
            ModAnimation.Animate(this, BackgroundProperty, Colors.Transparent, TimeSpan.FromMilliseconds(100));
            ModAnimation.Animate(this, ScaleTransform.ScaleXProperty, 1, TimeSpan.FromMilliseconds(200));
            ModAnimation.Animate(this, ScaleTransform.ScaleYProperty, 1, TimeSpan.FromMilliseconds(200));
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            ModAnimation.Animate(this, ScaleTransform.ScaleXProperty, 1, TimeSpan.FromMilliseconds(200));
            ModAnimation.Animate(this, ScaleTransform.ScaleYProperty, 1, TimeSpan.FromMilliseconds(200));

            if (isClick)
            {
                RaiseEvent(new RoutedEventArgs(ClickEvent, this));
            }
        }

        #endregion
    }
}
