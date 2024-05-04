using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Data;
using Catel.MVVM;
using MarketPlace.Model;
using MarketPlace.View;

namespace MarketPlace.ViewModel
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        private string _selectedFilter;
        public string SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged(nameof(SelectedFilter));
                    FilterProducts();
                }
            }
        }


        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    FilterProducts();
                }
            }
        }

        public MainWindowModel()
        {
            LoadData();
        }

        public void LoadData()
        {
            Products = new ObservableCollection<Product>();

            string connectionString = ConfigurationManager.ConnectionStrings["MarketPlaceDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT P.[ProductId], P.[Name], P.[Price], P.[Rating], P.[Аmount], S.[Name] AS SellerName FROM Products P INNER JOIN Sellers S ON P.SellerId = S.SellerId";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Products.Add(new Product
                    {
                        ProductID = (int)reader["ProductId"],
                        ProductName = (string)reader["Name"],
                        Price = (decimal)reader["Price"],
                        Rating = (decimal)reader["Rating"],
                        Аmount = (int)reader["Аmount"],
                        SellerID = (string)reader["SellerName"],
                    });
                }
            }
        }

        private ICollectionView _collectionView;

        private void FilterProducts()
        {
            if (_collectionView == null)
            {
                _collectionView = CollectionViewSource.GetDefaultView(Products);
            }

            _collectionView.Filter = item =>
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    return true;
                }

                if (item is Product product)
                {
                    switch (SelectedFilter)
                    {
                        case "Артикул":
                            return product.ProductID.ToString().Contains(SearchText);
                        case "Название":
                            return product.ProductName.Contains(SearchText);
                        case "Продавец":
                            return product.SellerID.Contains(SearchText);
                        default:
                            return false;
                    }
                }

                return false;
            };

            _collectionView.Refresh();
        }

        public Command ResetFilterCommand
        {
            get
            {
                return new Command(() =>
                {
                    SelectedFilter = null;
                    SearchText = null;
                });
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Command ShowLoginWindow
        {
            get
            {
                return new Command(() =>
                {
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                });
            }
        }
    }
}
