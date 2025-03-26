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

        public void LoginBackend(string username, string password, Window window)
        {
            MySqlConnection conn = DbConnect.GetConnection();
            MySqlDataReader reader = DbConnect.CheckCredentials(username, password, conn);

            if (reader != null)
            {
                currentuserid = Convert.ToInt32(reader["UserID"]);
                currentusername = reader["Username"].ToString();
                currentuserrole = reader["Role"].ToString();
                Window dashbord;

                if (currentuserrole == "Admin")
                {
                    MessageBox.Show("Welcome, Admin");
                    dashbord = new AdminDashbordView();
                    dashbord.Show();
                    window.Close();
                }
                else if (currentuserrole == "Cashier")
                {
                    MessageBox.Show("Welcome, Cashier!");
                    dashbord = new CashierDashbordView();
                    dashbord.Show();
                    window.Close();
                }
                else if (currentuserrole == "Manager")
                {
                    MessageBox.Show("Welcome, Manager!");
                    dashbord = new ManagerDashbordView();
                    dashbord.Show();
                    window.Close();
                }
                else
                {
                    MessageBox.Show("Unknown role detected. Contact support.");
                }
            }
            else
            {
                MessageBox.Show("Invalid Username or Password. login");
            }

            if (reader != null)
            {
                reader.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }
        }


    }
}

