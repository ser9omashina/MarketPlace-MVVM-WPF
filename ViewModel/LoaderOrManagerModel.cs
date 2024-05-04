using Catel.MVVM;
using MarketPlace.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MarketPlace.ViewModel
{
    public class LoaderOrManagerModel
    {

        string connectionString = ConfigurationManager.ConnectionStrings["MarketPlaceDB"].ConnectionString;
        //string connectionString = ConfigurationManager.ConnectionStrings["tusyuksa_marketplace_2"].ConnectionString;

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Product> filteredProducts { get; set; }

        public LoaderOrManagerModel()
        {
            LoadData();
        }

        public void LoadData()
        {
            Products = new ObservableCollection<Product>();


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
        public string ProductId { get; set; }
        public int IncreaseAmount { get; set; }
        public string PickupPointId { get; set; }
        public Command IncreaseProductAmount
        {
            get
            {
                return new Command(() =>
                {

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sqlQuery = "UPDATE Products SET Аmount = Аmount + @IncreaseAmount WHERE ProductId = @ProductId";

                        SqlCommand command = new SqlCommand(sqlQuery, connection);

                        command.Parameters.AddWithValue("@ProductId", ProductId);
                        command.Parameters.AddWithValue("@IncreaseAmount", IncreaseAmount);

                        command.ExecuteNonQuery();
                    }

                    var product = Products.FirstOrDefault(p => p.ProductID == int.Parse(ProductId));
                    if (product != null)
                    {

                        product.Аmount += IncreaseAmount;
                        MessageBox.Show("Количество товара изменено");
                    }
                    else
                        MessageBox.Show("говно");
                });
            }
        }

        public Command SearchPickupPointCommand
        {
            get
            {
                return new Command(() =>
                {


                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sqlQuery = "SELECT [Orders for last month] FROM [Pick-up points] WHERE [Pick-up-pointId] = @PickupPointId";

                        SqlCommand command = new SqlCommand(sqlQuery, connection);
                        command.Parameters.AddWithValue("@PickupPointId", PickupPointId);

                        var ordersForLastMonth = command.ExecuteScalar();

                        if (ordersForLastMonth != null)
                        {
                            MessageBox.Show($"Количество заказов за последний месяц на данном ПВЗ: {ordersForLastMonth}");
                        }
                        else
                        {
                            MessageBox.Show("ПВЗ с таким артикулом не найден");
                        }
                    }
                });
            }
        }
    }
}
