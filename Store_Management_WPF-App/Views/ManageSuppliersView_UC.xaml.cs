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
    /// Interaction logic for ManageSuppliersView_UC.xaml
    /// </summary>
    public partial class ManageSuppliersView_UC : UserControl
    {
        public ManageSuppliersView_UC()
        {
            InitializeComponent();
            supplier.LoadSuppliers(suppliersDataGrid);
        }

        Suppliers supplier = new Suppliers();

        private void btnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            string supplierName = txtSupplierName.Text;
            string contactPerson = txtContactPerson.Text;
            string phoneNumber = txtPhoneNumber.Text;
            string email = txtEmail.Text;
            string address = txtAddress.Text;

            Suppliers supplier = new Suppliers(txtSupplierName.Text, txtContactPerson.Text, txtPhoneNumber.Text, txtEmail.Text, txtAddress.Text);
            if (supplier.VerifySupplierDetails(this))
            {
                if (supplier.AddSuppliers(this)) 
                {
                    supplier.clearFields(this);
                    supplier.LoadSuppliers(suppliersDataGrid);
                    return;
                }
                
            } 
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            supplier.SearchSuppliers(suppliersDataGrid, txtSearch.Text);
        }

        private void EditSupplier_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.CommandParameter == null)
            {
                return;
            }

            if (!int.TryParse(btn.CommandParameter.ToString(), out int supplierId))
            {
                return;
            }

            // Confirmation MessageBox
            MessageBoxResult result = MessageBox.Show("Are you sure you want to edit this Supplier?",
                                                      "Confirm Edit",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return;
            }
            Suppliers supplier = Suppliers.GetSupplierById(supplierId);
            if (supplier == null)
            {
                MessageBox.Show("Supplier not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Load data into text fields
            txtSupplierName.Text = supplier.SupplierName;
            txtContactPerson.Text = supplier.ContactPerson;
            txtPhoneNumber.Text = supplier.PhoneNumber;
            txtEmail.Text = supplier.Email;
            txtAddress.Text = supplier.Address;

            txtSupplierName.Tag = supplier.SupplierID;
        }

        private void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.Tag == null)
            {
                MessageBox.Show("Error: Invalid selection!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int supplierId;
            if (!int.TryParse(btn.Tag.ToString(), out supplierId))
            {
                MessageBox.Show("Error: Invalid SupplierID!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this Supplier?",
                                                      "Confirm Deletion",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }
            Suppliers supplier = new Suppliers { SupplierID = supplierId };

            if (supplier.DeleteSupplier(this))
            {
                supplier.LoadSuppliers(suppliersDataGrid);
            }
        }

        private void btnUpdateSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (txtSupplierName.Tag == null)
            {
                MessageBox.Show("No Supplier selected for update!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int supplierId = (int)txtSupplierName.Tag;
            string supplierName = txtSupplierName.Text;
            string contactPerson = txtContactPerson.Text;
            string phoneNumber = txtPhoneNumber.Text;
            string email = txtEmail.Text;
            string address = txtAddress.Text;

            MessageBoxResult result = MessageBox.Show("Are you sure you want to update this supplier?",
                                                      "Confirm Update",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return;
            }
            Suppliers supplier = new Suppliers(supplierId, supplierName, contactPerson, phoneNumber, email, address);

            if (supplier.VerifySupplierDetails(this))
            {
                if (supplier.UpdateSupplier(this))
                {
                    supplier.LoadSuppliers(suppliersDataGrid);
                    supplier.clearFields(this);

                    MessageBox.Show("Supplier updated successfully!", "Update Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
        }

        private void btnDeleteSelectedSuppliers_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = suppliersDataGrid.SelectedItems.Cast<object>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select suppliers to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<int> supplierIds = new List<int>();

            foreach (var item in selectedItems)
            {
                if (item is DataRowView rowView)
                {
                    if (int.TryParse(rowView["SupplierID"].ToString(), out int supplierId))
                    {
                        supplierIds.Add(supplierId);
                    }
                }
                else if (item is Suppliers sup)
                {
                    supplierIds.Add(sup.SupplierID);
                }
            }

            if (supplierIds.Count == 0)
            {
                MessageBox.Show("Could not retrieve user IDs from selected rows.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Confirmation message
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {supplierIds.Count} suppliers?",
                                                      "Confirm Deletion",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return; // User canceled deletion
            }

            if (Suppliers.DeleteMultipleSuppliers(this,supplierIds))
            {
                Suppliers supplier = new Suppliers();
                supplier.LoadSuppliers(suppliersDataGrid);
            }
        }
    }
}
