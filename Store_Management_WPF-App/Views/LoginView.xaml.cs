using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Store_Management_WPF_App.Services;

namespace Store_Management_WPF_App.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            CommonButtons.minimizebutton(this);
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            CommonButtons.maximizebutton(this, MaximizeButton);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CommonButtons.closebutton(this);
        }

        private void topbar_darg(object sender, MouseButtonEventArgs e)
        {
            CommonButtons.dragmove(this, e);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.LoginBackend(txtUsername.Text, txtPassword.Password, this);
        }
    }
}
