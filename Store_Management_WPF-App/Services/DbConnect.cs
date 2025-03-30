using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Store_Management_WPF_App.Services
{
    public static class DbConnect
    {
        private static readonly string connectionString = "server=localhost;port=3306;database=store_db;uid=root;password=;";
        public static MySqlConnection GetConnection()
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();
                Console.WriteLine("Database Connected Successfully!");
                return conn;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        /*Login Database methods*/

        /*public static MySqlDataReader CheckCredentials(string username, string password, MySqlConnection conn)
        {
            try
            {
                string query = "SELECT * FROM Employees WHERE Username = @username";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string hashedPassword = reader["PasswordHash"].ToString();
                    bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                    if (isPasswordCorrect)
                    {
                        Console.WriteLine("Passwords match");
                        return reader;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect Password!");
                        return null;
                    }
                }
                return reader;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                conn.Close();
                return null;
            }
        }*/
        public static bool CheckCredentials(string username, string password, MySqlConnection conn, out int userId, out string userRole, out string userName)
        {
            userId = 0;
            userRole = string.Empty;
            userName = string.Empty;

            try
            {
                string query = "SELECT * FROM Employees WHERE Username = @username";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string hashedPassword = reader["PasswordHash"].ToString();
                    bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

                    if (isPasswordCorrect)
                    {
                        userId = Convert.ToInt32(reader["UserID"]);
                        userRole = reader["Role"].ToString();
                        userName = reader["Username"].ToString();
                        reader.Close();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect Password!");
                    }
                }

                reader.Close();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                conn.Close();
                return false;
            }
        }


        /*Employess Database methods*/


        public static bool SaveEmployee(string username, string password, string email, string phone, string role, MySqlConnection conn)
        {
            try
            {
                string query = "INSERT INTO Employees (Username, PasswordHash, Email, PhoneNumber, Role) VALUES (@username, @password, @email, @phone, @role )";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@role", role);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Employee Added Successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static DataTable LoadEmployees(MySqlConnection conn, string searchQuery = "")
        {
            string query = "SELECT UserID, Username, Email, PhoneNumber, Role FROM Employees";

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query += " WHERE Username LIKE @search OR Email LIKE @search OR PhoneNumber LIKE @search OR Role LIKE @search";
            }

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@search", "%" + searchQuery + "%");

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        public static Employee GetEmployeeById(int userId)
        {
            Employee employee = null;
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                string query = "SELECT UserID, Username, Email, PhoneNumber, Role FROM Employees WHERE UserID = @userId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            UserID = reader.GetInt32("UserID"),
                            Username = reader["Username"].ToString(),
                            Email = reader["Email"].ToString(),
                            PhoneNumber = reader["PhoneNumber"].ToString(),
                            Role = reader["Role"].ToString()
                        };
                    }
                }
            }
            return employee;
        }

        public static bool UpdateEmployee(int userId, string username, string email, string phone, string role, string Password)
        {
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                if (conn != null)
                {
                    try
                    {
                        string query = "UPDATE Employees SET Username = @username, Email = @email, PhoneNumber = @phone, Role = @role";

                        if (!string.IsNullOrWhiteSpace(Password))
                        {
                            query += ", PasswordHash = @passwordHash";
                        }

                        query += " WHERE UserID = @userId";

                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.Parameters.AddWithValue("@role", role);
                        cmd.Parameters.AddWithValue("@userId", userId);

                        if (!string.IsNullOrWhiteSpace(Password))
                        {
                            cmd.Parameters.AddWithValue("@passwordHash", Password);
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating employee: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                return false;
            }
        }

        public static bool DeleteEmployee(int userId)
        {
            MySqlConnection conn = GetConnection();
            if (conn == null)
            {
                return false;
            }

            string query = "DELETE FROM Employees WHERE UserID = @UserID";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting employee: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static bool DeleteEmployees(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return false;
            }

            MySqlConnection conn = GetConnection();
            if (conn == null)
            {
                return false;
            }

            string query = $"DELETE FROM Employees WHERE UserID IN ({string.Join(",", userIds)})";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting employees: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /*Supliers Database methods*/

        public static bool SaveSuppliers(string supplierName, string contactPerson, string phoneNumber, string email, string address, MySqlConnection conn)
        {
            try
            {
                string query = "INSERT INTO Suppliers (SupplierName, ContactPerson, PhoneNumber, Email, Address) VALUES (@supplierName, @contactPerson, @phoneNumber, @email, @address )";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@supplierName", supplierName);
                cmd.Parameters.AddWithValue("@contactPerson", contactPerson);
                cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@address", address);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Suppliers Added Successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static DataTable LoadSuppliers(MySqlConnection conn, string searchQuery = "")
        {
            string query = "SELECT SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address FROM Suppliers";

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query += " WHERE SupplierName LIKE @search OR ContactPerson LIKE @search OR Email LIKE @search OR PhoneNumber LIKE @search OR Address LIKE @search";
            }

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@search", "%" + searchQuery + "%");

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        public static Suppliers GetSupplierById(int supplierId)
        {
            Suppliers supplier = null;
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                string query = "SELECT SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address FROM Suppliers WHERE SupplierID = @supplierId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@supplierId", supplierId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        supplier = new Suppliers();
                        {
                            supplier.SupplierID = reader.GetInt32("SupplierID");
                            supplier.SupplierName = reader["SupplierName"].ToString();
                            supplier.ContactPerson = reader["ContactPerson"].ToString();
                            supplier.PhoneNumber = reader["PhoneNumber"].ToString();
                            supplier.Email = reader["Email"].ToString();
                            supplier.Address = reader["Address"].ToString();

                        };
                    }
                }
            }
            return supplier;
        }

        public static bool UpdateSuppliers(int supplierID, string supplierName, string contactPerson, string phoneNumber, string email, string address)
        {
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                if (conn != null)
                {
                    try
                    {
                        string query = "UPDATE Suppliers SET SupplierName = @supplierName,  ContactPerson = @contactPerson, PhoneNumber = @phoneNumber, Email = @email, Address = @address WHERE SupplierID= @supplierID";

                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@supplierName", supplierName);
                        cmd.Parameters.AddWithValue("@contactPerson", contactPerson);
                        cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.Parameters.AddWithValue("@supplierID", supplierID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating Supplier: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                return false;
            }
        }

        public static bool DeleteSupplier(int supplierId)
        {
            MySqlConnection conn = GetConnection();
            if (conn == null)
            {
                return false;
            }

            string query = "DELETE FROM Suppliers WHERE SupplierID= @supplierID";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("supplierID", supplierId);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting supplier " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static bool DeleteSuppliers(List<int> supplierIds)
        {
            if (supplierIds == null || supplierIds.Count == 0)
            {
                return false;
            }

            MySqlConnection conn = GetConnection();
            if (conn == null)
            {
                return false;
            }

            string query = $"DELETE FROM Suppliers WHERE SupplierID IN ({string.Join(",", supplierIds)})";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Suppliers: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /*Products Database methods*/

        //method to  get suplier list to populate combobox
        public static List<Suppliers> GetSuppliers()
        {
            List<Suppliers> suppliers = new List<Suppliers>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT SupplierID, SupplierName FROM Suppliers";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            suppliers.Add(new Suppliers
                            {
                                SupplierID = reader.GetInt32("SupplierID"),
                                SupplierName = reader.GetString("SupplierName")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suppliers: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return suppliers;
        }

        public static bool SaveProduct(string productname, string category, decimal costprice, decimal sellingprice, int quantity, int supplierId, MySqlConnection conn)
        {
            try
            {
                string query = "INSERT INTO Products (ProductName, Category, CostPrice, SellingPrice, Quantity, SupplierID) VALUES (@productname, @category, @costprice, @sellingprice, @quantity, @supplierId )";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@productname", productname);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@costprice", costprice);
                cmd.Parameters.AddWithValue("@sellingprice", sellingprice);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@supplierId", supplierId);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static DataTable LoadProducts(MySqlConnection conn, string searchQuery = "")
        {
            string query = @"SELECT p.ProductID, p.ProductName, p.Category, p.CostPrice, p.SellingPrice, p.Quantity, s.SupplierName 
                     FROM Products p 
                     LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID";

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query += " WHERE p.ProductName LIKE @search OR p.Category LIKE @search OR s.SupplierName LIKE @search";
            }

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@search", "%" + searchQuery + "%");

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        public static Product GetProductById(int productId)
        {
            Product product = null;
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                try
                {
                    string query = @"
                SELECT p.ProductName, p.Category, p.CostPrice, p.SellingPrice, s.SupplierName 
                FROM Products p 
                LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID 
                WHERE p.ProductID = @productId";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@productId", productId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Product
                            {
                                ProductID = productId,
                                ProductName = reader["ProductName"].ToString(),
                                Category = reader["Category"].ToString(),
                                CostPrice = reader.GetDecimal("CostPrice"),
                                SellingPrice = reader.GetDecimal("SellingPrice"),
                                SupplierName = reader["SupplierName"].ToString()
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching product details: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return product;
        }

        public static bool UpdateProduct(int productId, string productName, string category, decimal costPrice, decimal sellingPrice, int supplierId)
        {
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                try
                {
                    string query = @"UPDATE Products SET ProductName = @productName, Category = @category, CostPrice = @costPrice, SellingPrice = @sellingPrice, SupplierID = @supplierId WHERE ProductID = @productId";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@productName", productName);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@costPrice", costPrice);
                    cmd.Parameters.AddWithValue("@sellingPrice", sellingPrice);
                    cmd.Parameters.AddWithValue("@supplierId", supplierId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating product: " + ex.Message);
                    return false;
                }
            }
        }

        public static bool DeleteProduct(int productId)
        {
            using (MySqlConnection conn = DbConnect.GetConnection())
            {
                if (conn != null)
                {
                    try
                    {
                        string query = "DELETE FROM Products WHERE ProductID = @productId";

                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@productId", productId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting product: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                return false;
            }
        }

        public static int CreatePurchaseOrder(int productId, int quantity, decimal totalCost)
        {
            int purchaseId = 0;
            try
            {
                using (var connection = DbConnect.GetConnection())
                {

                    // Insert the purchase order
                    string query = "INSERT INTO PurchaseOrders (SupplierID, TotalAmount) " +
                                   "SELECT SupplierID, @TotalAmount FROM Products WHERE ProductID = @ProductID";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", productId);
                        cmd.Parameters.AddWithValue("@TotalAmount", totalCost);
                        cmd.ExecuteNonQuery();
                        purchaseId = (int)cmd.LastInsertedId;
                    }

                    // Insert purchase details
                    query = "INSERT INTO PurchaseDetails (PurchaseID, ProductID, Quantity, TotalAmount) " +
                            "VALUES (@PurchaseID, @ProductID, @Quantity, @TotalAmount)";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.Parameters.AddWithValue("@ProductID", productId);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@TotalAmount", totalCost);
                        cmd.ExecuteNonQuery();
                    }
                    query = "UPDATE Products SET Quantity = Quantity + @Quantity WHERE ProductID = @ProductID";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", productId);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating purchase order: " + ex.Message);
            }

            return purchaseId;
        }

        public static int GetCount(string tableName)
        {
            int count = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = $"SELECT COUNT(*) FROM {tableName}";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return count;
        }

    }
}
