using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Data;

namespace Store_Management_WPF_App.Services
{
    public class Employee
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        private string PasswordHash { get; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }

        public Employee() { }

        public Employee(int userId, string username, string password, string email, string phoneNumber, string role)
        {
            UserID = userId;
            Username = username;
            PasswordHash = HashPassword(password);
            Email = email;
            PhoneNumber = phoneNumber;
            Role = role;
        }

        public Employee(string username, string password, string email, string phoneNumber, string role)
        {
            Username = username;
            PasswordHash = HashPassword(password);
            Email = email;
            PhoneNumber = phoneNumber;
            Role = role;
        }

        public string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        private static Window GetParentWindow(UserControl control)
        {
            return Window.GetWindow(control);
        }

        public void clearFields(UserControl window)
        {
            TextBox txtUsername = window.FindName("txtUsername") as TextBox;
            PasswordBox txtPassword = window.FindName("txtPassword") as PasswordBox;
            TextBox txtEmail = window.FindName("txtEmail") as TextBox;
            TextBox txtPhone = window.FindName("txtPhone") as TextBox;
            ComboBox cmbRole = window.FindName("cmbRole") as ComboBox;
            txtUsername.Clear();
            txtPassword.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            cmbRole.Items.Clear();
        }

        public bool AddEmployee(UserControl window)
        {
            MySqlConnection conn = DbConnect.GetConnection();
            if (conn != null)
            {
                if (DbConnect.SaveEmployee(Username, PasswordHash, Email, PhoneNumber, Role, conn))
                {
                    MessageBox.Show(GetParentWindow(window), "Employee added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show(GetParentWindow(window), "Error adding employee. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                MessageBox.Show(GetParentWindow(window), "Error connecting to database. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool VerifyEnteredData(UserControl window, ComboBox comboBox, bool isUpdate = false, string newPassword = "")
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(PhoneNumber) ||
                string.IsNullOrWhiteSpace(Role))
            {
                MessageBox.Show(GetParentWindow(window), "Please fill all fields except password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Role == "Select a role")
            {
                MessageBox.Show(GetParentWindow(window), "Please select a valid role.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show(GetParentWindow(window), "Please enter a valid email address.", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(PhoneNumber, @"^\+?\d{10,11}$"))
            {
                MessageBox.Show(GetParentWindow(window), "Please enter a valid phone number.", "Invalid Phone Number", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // If it's an ADD operation (isUpdate == false), password is required
            if (!isUpdate && string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show(GetParentWindow(window), "Password is required for new employees.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
                
            }

            // If it's an UPDATE operation, validate password only if entered
            if (isUpdate && !string.IsNullOrWhiteSpace(newPassword))
            {
                if (newPassword.Length < 6)
                {
                    MessageBox.Show(GetParentWindow(window), "Password must be at least 6 characters long.", "Weak Password", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }


        public void LoadEmployees(DataGrid dataGrid)
        {
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                if (conn != null)
                {
                    DataTable dt = DbConnect.LoadEmployees(conn);
                    dataGrid.ItemsSource = dt.DefaultView;
                }
                else
                {
                    MessageBox.Show("Error connecting to database.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static Employee GetEmployeeById(int userId)
        {
            return DbConnect.GetEmployeeById(userId);
        }

        public bool UpdateEmployee()
        {
            return DbConnect.UpdateEmployee(UserID,Username,Email,PhoneNumber,Role,PasswordHash);
        }

        public bool DeleteEmployee(UserControl window)
        {
            if (DbConnect.DeleteEmployee(UserID))
            {
                MessageBox.Show(GetParentWindow(window), "Employee deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            else
            {
                MessageBox.Show(GetParentWindow(window), "Error deleting employee or employee not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool DeleteMultipleEmployees(UserControl window, List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                MessageBox.Show(GetParentWindow(window), "No employees selected for deletion!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DbConnect.DeleteEmployees(userIds))
            {
                MessageBox.Show(GetParentWindow(window), "Employees deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            else
            {
                MessageBox.Show(GetParentWindow(window), "Error deleting employees.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void SearchEmployees(DataGrid dataGrid, string searchQuery)
        {
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                if (conn != null)
                {
                    DataTable dt = DbConnect.LoadEmployees(conn, searchQuery);
                    dataGrid.ItemsSource = dt.DefaultView;
                }
                else
                {
                    MessageBox.Show("Error connecting to database.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
