using Catel.MVVM;
using MarketPlace.Model;
using MarketPlace.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace MarketPlace.ViewModel
{


    public class LoginViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<User> Users { get; set; }

        private string _userTextBoxValue;
        private string _passwordTextBoxValue;

        public string UserTextBoxValue
        {
            get { return _userTextBoxValue; }
            set
            {
                if (_userTextBoxValue != value)
                {
                    _userTextBoxValue = value;
                    OnPropertyChanged(nameof(UserTextBoxValue));
                }
            }
        }

        public string PasswordTextBoxValue
        {
            get { return _passwordTextBoxValue; }
            set
            {
                if (_passwordTextBoxValue != value)
                {
                    _passwordTextBoxValue = value;
                    OnPropertyChanged(nameof(PasswordTextBoxValue));
                }
            }
        }


        public Command LoginButton
        {
            get
            {
                return new Command(() =>
                {
                    string role = CheckIfUserExists(UserTextBoxValue, PasswordTextBoxValue);
                    if (!string.IsNullOrEmpty(role))
                    {
                        MessageBox.Show("Вход выполнен как " + role + ":  " + UserTextBoxValue);
                        Application.Current.MainWindow?.Close();
                        if (role == "Administrator")
                        {
                            AdministratorWindow administratorWindow = new AdministratorWindow();
                            administratorWindow.Show();
                            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
                        }
                        else
                        {
                            LoaderOrManagerWindow loaderOrManagerWindow = new LoaderOrManagerWindow();
                            loaderOrManagerWindow.Show();
                            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Вход не выполнен", "Неверный логин или пароль");
                    }


                });
            }
        }

        private string CheckIfUserExists(string UserTextBoxValue, string PasswordTextBoxValue)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MarketPlaceDB"].ConnectionString;

            string query = "SELECT Role FROM Employees WHERE Name = @Name and Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", UserTextBoxValue);
                    command.Parameters.AddWithValue("@Password", PasswordTextBoxValue);
                    object result = command.ExecuteScalar();
                    return result as string;
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
