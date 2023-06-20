using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MinecraftServerDownloader.Modules
{
    internal class ModAnimation
    {
        public static void Animate(UIElement targetElement, DependencyProperty property, double toValue, Duration duration)
        {
            var animation = new DoubleAnimation
            {
                To = toValue,
                Duration = duration
            };

            Storyboard.SetTarget(animation, targetElement);
            Storyboard.SetTargetProperty(animation, new PropertyPath($"(RenderTransform).(ScaleTransform.{property.Name})"));

            var storyboard = new Storyboard
            {
                Duration = duration
            };
            storyboard.Children.Add(animation);

            storyboard.Begin();
        }
        public static void Animate(UIElement targetElement, DependencyProperty property, Color toValue, Duration duration)
        {
            var animation = new ColorAnimation
            {
                To = toValue,
                Duration = duration
            };

            Storyboard.SetTarget(animation, targetElement);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(0).(1)", property, SolidColorBrush.ColorProperty));

            var storyboard = new Storyboard
            {
                Duration = duration
            };
            storyboard.Children.Add(animation);

            storyboard.Begin();
        }
    }
}
