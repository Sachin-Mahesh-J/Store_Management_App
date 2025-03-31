using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ManageEmployeeView_UC.xaml
    /// </summary>
    public partial class ManageEmployeeView_UC : UserControl
    {
        private Employee employee = new Employee();
        public ManageEmployeeView_UC()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            employee.LoadEmployees(employeeDataGrid);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            employee.clearFields(this);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            employee.SearchEmployees(employeeDataGrid, txtSearch.Text);
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.CommandParameter == null)
            {
                return;
            }

            if (!int.TryParse(btn.CommandParameter.ToString(), out int userId))
            {
                return;
            }

            // Confirmation MessageBox
            MessageBoxResult result = MessageBox.Show("Are you sure you want to edit this employee?",
                                                      "Confirm Edit",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return; // User canceled the action
            }

            Employee employee = Employee.GetEmployeeById(userId);
            if (employee == null)
            {
                MessageBox.Show("Employee not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Load data into text fields
            txtUsername.Text = employee.Username;
            txtEmail.Text = employee.Email;
            txtPhone.Text = employee.PhoneNumber;
            cmbRole.Text = employee.Role;

            // Store UserID in the tag for update reference
            txtUsername.Tag = employee.UserID;
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string email = txtEmail.Text;
            string phone = txtPhone.Text;
            string role = cmbRole.Text;

            Employee employee = new Employee(username, password, email, phone, role);
            if (employee.VerifyEnteredData(this, cmbRole, false, password))
            {
                if (employee.AddEmployee(this))
                {
                    employee.LoadEmployees(employeeDataGrid);
                    employee.clearFields(this);
                    return;
                }
            }
        }

        private void btnUpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Tag == null)
            {
                MessageBox.Show("No employee selected for update!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int userId = (int)txtUsername.Tag;
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string email = txtEmail.Text;
            string phone = txtPhone.Text;
            string role = cmbRole.Text;

            // Confirmation MessageBox
            MessageBoxResult result = MessageBox.Show("Are you sure you want to update this employee?",
                                                      "Confirm Update",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return; // User canceled the action
            }

            Employee employee = new Employee(userId, username, password, email, phone, role);

            if (employee.VerifyEnteredData(this, cmbRole, true, password))
            {
                if (employee.UpdateEmployee())
                {
                    employee.LoadEmployees(employeeDataGrid);
                    employee.clearFields(this);

                    MessageBox.Show("Employee updated successfully!", "Update Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
        }


        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is int userId)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this employee?",
                                                          "Confirm Deletion",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Employee employee = new Employee { UserID = userId };

                    if (employee.DeleteEmployee(this))
                    {
                        employee.LoadEmployees(employeeDataGrid); // Refresh the DataGrid
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete employee.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Error: Invalid selection!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnDeleteSelectedEmployees_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = employeeDataGrid.SelectedItems.Cast<object>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select employees to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<int> userIds = new List<int>();

            foreach (var item in selectedItems)
            {
                if (item is DataRowView rowView)
                {
                    if (int.TryParse(rowView["UserID"].ToString(), out int userId))
                    {
                        userIds.Add(userId);
                    }
                }
                else if (item is Employee emp)
                {
                    userIds.Add(emp.UserID);
                }
            }

            if (userIds.Count == 0)
            {
                MessageBox.Show("Could not retrieve user IDs from selected rows.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Confirmation message
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {userIds.Count} employees?",
                                                      "Confirm Deletion",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return; // User canceled deletion
            }

            if (Employee.DeleteMultipleEmployees(this, userIds))
            {
                Employee employee = new Employee();
                employee.LoadEmployees(employeeDataGrid); // Refresh DataGrid after deletion
            }
        }

        
    }
}
