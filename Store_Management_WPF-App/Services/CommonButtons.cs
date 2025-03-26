using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace Store_Management_WPF_App.Services
{
    public static class CommonButtons
    {
        public static void closebutton(Window window)
        {
            window.Close();
        }

        public static void minimizebutton(Window window)
        {
            window.WindowState = WindowState.Minimized;
        }

        public static void maximizebutton(Window window, Button maximizeButton)
        {
            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Normal;
                maximizeButton.Content = "□";
            }
            else
            {
                window.WindowState = WindowState.Maximized;
                maximizeButton.Content = "❐";
            }
        }

        public static void dragmove(Window window, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                window.DragMove();
            }
        }

        public static void HighlightActiveButton(Button buttonToHighlight, string currentPage, string buttonPage)
        {
            if (currentPage == buttonPage)
            {
                buttonToHighlight.Background = new SolidColorBrush(Color.FromRgb(106, 17, 203)); // Active color (Highlight: #6A11CB)
            }
            else
            {
                buttonToHighlight.Background = new SolidColorBrush(Color.FromRgb(58, 12, 163)); // Default color (Dark Purple: #3A0CA3)
            }
        }
    }
}
