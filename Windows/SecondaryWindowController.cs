using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace iMine.Launcher.Windows
{
    public static class SecondaryWindowController
    {
        public static readonly List<Window> Windows = new List<Window>();

        public static void HookWindow(Window window)
        {
            window.Loaded += (s, e) =>
            {
                if (Windows.Count == 0)
                {
                    MainWindow.Instance.IsEnabled = false;
                    MainWindow.Instance.BlackTint.Visibility = Visibility.Visible;
                }
                else if (!(window is BugreportWindow))
                    window.Left += 400;

                Windows.Add(window);
            };
            window.Closed += (s, e) =>
            {
                if (Windows.Count == 2)
                {
                    if (!(window is BugreportWindow) && Equals(Windows.First(), window))
                        Windows[1].Left -= 400;
                }
                else if (Windows.Count <= 1)
                {
                    MainWindow.Instance.IsEnabled = true;
                    MainWindow.Instance.BlackTint.Visibility = Visibility.Hidden;
                }

                Windows.Remove(window);
            };
        }
    }
}