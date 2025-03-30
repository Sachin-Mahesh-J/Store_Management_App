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
    /// Interaction logic for ProductOrderView.xaml
    /// </summary>
    public partial class ProductOrderView : Window
    {
        private int productId;
        private decimal costPrice;
        public ProductOrderView(int productID)
        {
            InitializeComponent();
            productId = productID;
            LoadProductDetails();
        }
        private void LoadProductDetails()
        {
            var product = Product.GetProductById(productId);
            if (product != null)
            {
                ProductNameTextBox.Text = product.ProductName;
                SupplierNameTextBox.Text = product.SupplierName;
                costPrice = product.CostPrice;
            }
        }

        private void QuantityTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                decimal totalCost = costPrice * quantity;
                TotalCostTextBox.Text = totalCost.ToString("F2");
            }
            else
            {
                TotalCostTextBox.Clear();
            }
        }

        private void ConfirmPurchase_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(QuantityTextBox.Text, out int quantity) && quantity > 0)
            {
                decimal totalCost = costPrice * quantity;

                // Create the purchase order and save it to the database
                int purchaseId = DbConnect.CreatePurchaseOrder(productId, quantity, totalCost);

                if (purchaseId > 0)
                {
                    MessageBox.Show("Purchase successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Purchase failed. Try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid quantity.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CommonButtons.dragmove(this, e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel the order.", "Info", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result ==MessageBoxResult.Yes)
            { this.Close(); }
            else
            { return; }


        }
    }
}
