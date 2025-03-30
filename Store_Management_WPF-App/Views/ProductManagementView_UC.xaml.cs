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
    /// Interaction logic for ProductManagementView_UC.xaml
    /// </summary>
    public partial class ProductManagementView_UC : UserControl
    {
        Product product = new Product();
        public ProductManagementView_UC()
        {
            InitializeComponent();
            Cmbbox_LoadSuppliers();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            product.LoadProducts(productDataGrid);
        }

        private void Cmbbox_LoadSuppliers()
        {
            List<Suppliers> suppliers = DbConnect.GetSuppliers();
            cmbSupplier.ItemsSource = suppliers;
            cmbSupplier.DisplayMemberPath = "SupplierName";
            cmbSupplier.SelectedValuePath = "SupplierID";
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            product.SearchProduct(productDataGrid, txtSearch.Text);
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.CommandParameter == null)
            {
                return;
            }

            if (!int.TryParse(btn.CommandParameter.ToString(), out int productId))
            {
                return;
            }

            // Confirmation MessageBox
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this product?",
                                                      "Confirm Deletion",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return; // User canceled the action
            }

            Product product = new Product { ProductID = productId };
            if (product.DeleteProduct())
            {
                MessageBox.Show("Product deleted successfully!", "Delete Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                product.LoadProducts(productDataGrid);
                return;
            }
            else
            {
                MessageBox.Show("Failed to delete the product.", "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            string productName = txtProductName.Text;
            string category = txtCategory.Text;
            if (!decimal.TryParse(txtCostPrice.Text, out decimal costPrice))
            {
                MessageBox.Show("Please enter a valid cost price.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(txtSellingPrice.Text, out decimal sellingPrice))
            {
                MessageBox.Show("Please enter a valid selling price.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cmbSupplier.SelectedValue == null)
            {
                MessageBox.Show("Please select a supplier.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int supplierID = (int)cmbSupplier.SelectedValue;

            Product product = new Product(productName, category, costPrice, sellingPrice, supplierID);
            if (product.VerifyProductData(this))
            {
                if (product.AddProduct(this))
                {
                    product.LoadProducts(productDataGrid);
                    product.clearFields(this);
                    return;
                }
            }
        }

        private void btnUpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            if (txtProductName.Tag == null)
            {
                MessageBox.Show("No product selected for update!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int productId = (int)txtProductName.Tag;
            string productName = txtProductName.Text;
            string category = txtCategory.Text;
            decimal costPrice = decimal.Parse(txtCostPrice.Text);
            decimal sellingPrice = decimal.Parse(txtSellingPrice.Text);
            int supplierId = (cmbSupplier.SelectedItem as Suppliers)?.SupplierID ?? 0;

            // Confirmation MessageBox
            MessageBoxResult result = MessageBox.Show("Are you sure you want to update this product?",
                                                      "Confirm Update",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return; // User canceled the action
            }

            Product product = new Product
            {
                ProductID = productId,
                ProductName = productName,
                Category = category,
                CostPrice = costPrice,
                SellingPrice = sellingPrice,
                SupplierID = supplierId
            };

            if (product.UpdateProduct())
            {
                MessageBox.Show("Product updated successfully!", "Update Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                product.LoadProducts(productDataGrid);
                return;
            }
            else
            {
                MessageBox.Show("Failed to update the product.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteSelectedProducts_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.CommandParameter == null)
            {
                return;
            }

            if (!int.TryParse(btn.CommandParameter.ToString(), out int productId))
            {
                return;
            }

            // Confirmation MessageBox
            MessageBoxResult result = MessageBox.Show("Are you sure you want to edit this product?",
                                                      "Confirm Edit",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return; // User canceled the action
            }

            // Get product details
            Product product = Product.GetProductById(productId);
            if (product == null)
            {
                MessageBox.Show("Product not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Load data into text fields
            txtProductName.Text = product.ProductName;
            txtCategory.Text = product.Category;
            txtCostPrice.Text = product.CostPrice.ToString();
            txtSellingPrice.Text = product.SellingPrice.ToString();

            // Set the Supplier combo box by finding the item that matches the supplier name
            var supplierItem = cmbSupplier.Items.Cast<Suppliers>().FirstOrDefault(item => item.SupplierName == product.SupplierName);

            if (supplierItem != null)
            {
                cmbSupplier.SelectedItem = supplierItem;
            }

            // Store ProductID in the tag for update reference
            txtProductName.Tag = product.ProductID;

        }

        private void BuyProduct_Click(object sender, RoutedEventArgs e)
        {
            int productId = (int)((Button)sender).CommandParameter;
            Window orderWindow = new ProductOrderView(productId);
            orderWindow.ShowDialog();

        }

        private void btnViewOrders_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
