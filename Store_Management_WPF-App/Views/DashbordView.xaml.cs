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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Store_Management_WPF_App.Services;

namespace Store_Management_WPF_App.Views
{
    /// <summary>
    /// Interaction logic for DashbordView.xaml
    /// </summary>
    public partial class DashbordView : UserControl
    {
        public DashbordView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCounts();
        }

        private void LoadCounts()
        {
            txtEmployeeCount.Text = DbConnect.GetCount("Employees").ToString();
            txtOrderCount.Text = DbConnect.GetCount("PurchaseOrders").ToString();
            txtSupplierCount.Text = DbConnect.GetCount("Suppliers").ToString();
            txtProductCount.Text = DbConnect.GetCount("Products").ToString();
        }
    }
}
