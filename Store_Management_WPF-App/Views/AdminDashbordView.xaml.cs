using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for AdminDashbordView.xaml
    /// </summary>
    public partial class AdminDashbordView : Window
    {
        

        public AdminDashbordView()
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

        private void btnM_employee_Click(object sender, RoutedEventArgs e)
        {

            MainContentControl.Content = new ManageEmployeeView_UC();
            Page_name.Text = "Manage Employee";
        }

        private void btnM_product_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new ProductManagementView_UC();
            Page_name.Text = "Manage Product";
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
