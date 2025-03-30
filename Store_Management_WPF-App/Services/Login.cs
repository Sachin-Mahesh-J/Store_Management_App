using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;
using Store_Management_WPF_App.Views;

namespace Store_Management_WPF_App.Services
{
    public class Login
    {
        private int currentuserid;
        private string currentusername;
        private string currentuserrole;

        public int Currentuserid { get => currentuserid; }
        public string Currentusername { get => currentusername; }
        public string Currentuserrole { get => currentuserrole; }

        /*public void LoginBackend(string username, string password, Window window)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(window, "Please fill all fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MySqlConnection conn = DbConnect.GetConnection();
            MySqlDataReader reader = DbConnect.CheckCredentials(username, password, conn);

            if (reader != null && reader.Read())
            {
                currentuserid = Convert.ToInt32(reader["UserID"]);
                currentusername = reader["Username"].ToString();
                currentuserrole = reader["Role"].ToString();
                Window dashbord;

                if (currentuserrole == "Admin")
                {
                    dashbord = new AdminDashbordView();
                    dashbord.Show();
                    window.Close();
                }
                else if (currentuserrole == "Cashier")
                {
                    dashbord = new CashierDashbordView();
                    dashbord.Show();
                    window.Close();
                }
                else if (currentuserrole == "Manager")
                {
                    dashbord = new ManagerDashbordView();
                    dashbord.Show();
                    window.Close();
                }
                else
                {
                    MessageBox.Show(window, "Unknown role detected. Contact support.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(window, "Invalid Username or Password.", "Invalid Login", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (reader != null)
            {
                reader.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }
        }*/
        public void LoginBackend(string username, string password, Window window)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(window, "Please fill all fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MySqlConnection conn = DbConnect.GetConnection();

            if (DbConnect.CheckCredentials(username, password, conn, out int currentuserid, out string currentuserrole, out string currentusername))
            {
                Window dashboard;

                if (currentuserrole == "Admin")
                {
                    dashboard = new AdminDashbordView();
                }
                else if (currentuserrole == "Cashier")
                {
                    dashboard = new CashierDashbordView();
                }
                else if (currentuserrole == "Manager")
                {
                    dashboard = new ManagerDashbordView();
                }
                else
                {
                    MessageBox.Show(window, "Unknown role detected. Contact support.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                dashboard.Show();
                window.Close();
            }
            else
            {
                MessageBox.Show(window, "Invalid Username or Password.", "Invalid Login", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (conn != null)
            {
                conn.Close();
            }
        }



    }
}

