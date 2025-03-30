using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace Store_Management_WPF_App.Services
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public int SupplierID { get; set; }
        public string DateAdded { get; set; }

        public string SupplierName { get; set; }

        public Product() { }
        public Product(string productName, string category, decimal costPrice, decimal sellingPrice, int supplierID)
        {
            ProductName = productName;
            Category = category;
            CostPrice = costPrice;
            SellingPrice = sellingPrice;
            Quantity = 0;
            SupplierID = supplierID;
        }

        public Product(int productID, string productName, string category, decimal costPrice, decimal sellingPrice, int quantity, int supplierID, string dateAdded)
        {
            ProductID = productID;
            ProductName = productName;
            Category = category;
            CostPrice = costPrice;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            SupplierID = supplierID;
            DateAdded = dateAdded;
        }

        private static Window GetParentWindow(UserControl control)
        {
            return Window.GetWindow(control);
        }

        public void clearFields(UserControl window)
        {
            TextBox txtProductName = window.FindName("txtProductName") as TextBox;
            TextBox txtCategory = window.FindName("txtCategory") as TextBox;
            TextBox txtCostPrice = window.FindName("txtCostPrice") as TextBox;
            TextBox txtSellingPrice = window.FindName("txtSellingPrice") as TextBox;
            TextBox txtQuantity = window.FindName("txtQuantity") as TextBox;
            ComboBox cmbSupplier = window.FindName("cmbSupplier") as ComboBox;
            txtProductName.Clear();
            txtCategory.Clear();
            txtCostPrice.Clear();
            txtSellingPrice.Clear();
            cmbSupplier.SelectedIndex = -1;
        }

        public bool AddProduct(UserControl window)
        {
            MySqlConnection conn = DbConnect.GetConnection();
            if (conn != null)
            {
                if (DbConnect.SaveProduct(ProductName,Category,CostPrice,SellingPrice,Quantity,SupplierID,conn))
                {
                    MessageBox.Show(GetParentWindow(window), "Product added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show(GetParentWindow(window), "Error adding Product. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                MessageBox.Show(GetParentWindow(window), "Error connecting to database. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool VerifyProductData(UserControl window)
        {
            // Check for empty or whitespace fields
            if (string.IsNullOrWhiteSpace(ProductName) ||
                string.IsNullOrWhiteSpace(Category) ||
                CostPrice <= 0 ||
                SellingPrice <= 0 ||
                SupplierID <= 0)
            {
                MessageBox.Show(GetParentWindow(window), "Please fill in all the required fields with valid data.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate product name (only letters, numbers, and spaces allowed)
            if (!Regex.IsMatch(ProductName, @"^[a-zA-Z0-9\s]+$"))
            {
                MessageBox.Show(GetParentWindow(window), "Product name contains invalid characters.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate category (only letters and spaces allowed)
            if (!Regex.IsMatch(Category, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show(GetParentWindow(window), "Category name contains invalid characters.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Ensure cost price is a valid decimal value
            if (CostPrice <= 0)
            {
                MessageBox.Show(GetParentWindow(window), "Cost price must be a positive decimal number.", "Invalid Cost Price", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Ensure selling price is a valid decimal value
            if (SellingPrice <= 0)
            {
                MessageBox.Show(GetParentWindow(window), "Selling price must be a positive decimal number.", "Invalid Selling Price", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Check if selling price is less than cost price (optional warning)
            if (SellingPrice < CostPrice)
            {
                MessageBox.Show(GetParentWindow(window), "Warning: Selling price is less than the cost price.", "Price Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Quantity can be zero initially, but must not be negative
            if (Quantity < 0)
            {
                MessageBox.Show(GetParentWindow(window), "Quantity cannot be negative.", "Invalid Quantity", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate Supplier ID (should be a positive integer)
            if (SupplierID <= 0)
            {
                MessageBox.Show(GetParentWindow(window), "Please select a valid supplier.", "Invalid Supplier", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public void LoadProducts(DataGrid dataGrid)
        {
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                if (conn != null)
                {
                    DataTable dt = DbConnect.LoadProducts(conn);
                    dataGrid.ItemsSource = dt.DefaultView;
                }
                else
                {
                    MessageBox.Show("Error connecting to database.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public static Product GetProductById(int productId)
        {
            return DbConnect.GetProductById(productId);
        }

        public bool UpdateProduct()
        {
            return DbConnect.UpdateProduct(ProductID, ProductName, Category, CostPrice, SellingPrice, SupplierID);
        }

        public void SearchProduct(DataGrid dataGrid, string searchQuery)
        {
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                if (conn != null)
                {
                    DataTable dt = DbConnect.LoadProducts(conn, searchQuery);
                    dataGrid.ItemsSource = dt.DefaultView;
                }
                else
                {
                    MessageBox.Show("Error connecting to database.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public bool DeleteProduct()
        {
            return DbConnect.DeleteProduct(ProductID);
        }

    }
}
