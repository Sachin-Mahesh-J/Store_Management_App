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

        public static MySqlDataReader CheckCredentials(string username, string password, MySqlConnection conn)
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
        }

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

        public static DataTable GetEmployees(MySqlConnection conn, string searchQuery = "")
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



    }
}
