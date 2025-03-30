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
    public class Suppliers
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public Suppliers() { }

        public Suppliers(string supplierName, string contactPerson, string phoneNumber, string email, string address) 
        {
            SupplierName = supplierName;
            ContactPerson = contactPerson;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
        }

        public Suppliers(int supplierID, string supplierName, string contactPerson, string phoneNumber, string email, string address)
        {
            SupplierID = supplierID;
            SupplierName = supplierName;
            ContactPerson = contactPerson;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
        }

        private static Window GetParentWindow(UserControl control)
        {
            return Window.GetWindow(control);
        }

        public void clearFields(UserControl window)
        {
            TextBox txtSupplierName = window.FindName("txtSupplierName") as TextBox;
            TextBox txtContactPerson = window.FindName("txtContactPerson") as TextBox;
            TextBox txtPhoneNumber = window.FindName("txtPhoneNumber") as TextBox;
            TextBox txtEmail = window.FindName("txtEmail") as TextBox;
            TextBox txtAddress = window.FindName("txtAddress") as TextBox;
            txtAddress.Clear();
            txtSupplierName.Clear();
            txtContactPerson.Clear();
            txtPhoneNumber.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
        }

        public bool AddSuppliers(UserControl window)
        {
            MySqlConnection conn = DbConnect.GetConnection();
            if (conn != null)
            {
                if (DbConnect.SaveSuppliers(SupplierName,ContactPerson,PhoneNumber,Email,Address,conn))
                {
                    MessageBox.Show(GetParentWindow(window), "Supplier added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show(GetParentWindow(window), "Error adding Supplier. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                MessageBox.Show(GetParentWindow(window), "Error connecting to database. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public bool VerifySupplierDetails(UserControl window)
        {
            // Check for empty fields
            if (string.IsNullOrWhiteSpace(SupplierName) ||
                string.IsNullOrWhiteSpace(ContactPerson) ||
                string.IsNullOrWhiteSpace(PhoneNumber) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Address))
            {
                MessageBox.Show(GetParentWindow(window), "Please fill all fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate phone number (10-11 digits)
            if (!Regex.IsMatch(PhoneNumber, @"^\+?\d{10,11}$"))
            {
                MessageBox.Show(GetParentWindow(window), "Please enter a valid phone number.", "Invalid Phone Number", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate email format
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show(GetParentWindow(window), "Please enter a valid email address.", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Check if address exceeds the limit
            if (Address.Length > 255)
            {
                MessageBox.Show(GetParentWindow(window), "Address should not exceed 255 characters.", "Invalid Address", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public void LoadSuppliers(DataGrid dataGrid)
        {
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                if (conn != null)
                {
                    DataTable dt = DbConnect.LoadSuppliers(conn);
                    dataGrid.ItemsSource = dt.DefaultView;
                }
                else
                {
                    MessageBox.Show("Error connecting to database.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public static Suppliers GetSupplierById(int supplierId)
        {
            return DbConnect.GetSupplierById(supplierId);
        }

        public void SearchSuppliers(DataGrid dataGrid, string searchQuery)
        {
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                if (conn != null)
                {
                    DataTable dt = DbConnect.LoadSuppliers(conn, searchQuery);
                    dataGrid.ItemsSource = dt.DefaultView;
                }
                else
                {
                    MessageBox.Show("Error connecting to database.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public bool UpdateSupplier(UserControl window)
        {
            return DbConnect.UpdateSuppliers(SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address);
        }

        public bool DeleteSupplier(UserControl window)
        {
            if (DbConnect.DeleteSupplier(SupplierID))
            {
                MessageBox.Show(GetParentWindow(window), "Supplier deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            else
            {
                MessageBox.Show(GetParentWindow(window), "Error deleting Supplier or Supplier not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool DeleteMultipleSuppliers(UserControl window, List<int> supplierIds)
        {
            if (supplierIds == null || supplierIds.Count == 0)
            {
                MessageBox.Show(GetParentWindow(window), "No Suppliers selected for deletion!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DbConnect.DeleteSuppliers(supplierIds))
            {
                MessageBox.Show(GetParentWindow(window), "Suppliers deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            else
            {
                MessageBox.Show(GetParentWindow(window), "Error deleting Suppliers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }



}
