using Catel.MVVM;
using MarketPlace.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;

namespace MarketPlace.ViewModel
{
    public class AdministratorModel
    {            
        string connectionString = ConfigurationManager.ConnectionStrings["MarketPlaceDB"].ConnectionString;
        //string connectionString = ConfigurationManager.ConnectionStrings["tusyuksa_marketplace_2"].ConnectionString;

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Product> filteredProducts { get; set; }
        public ObservableCollection<PickUpPoint> PickUpPoints { get; set; }


        public AdministratorModel()
        {
            LoadData();
            LoadSellers();
            LoadPickUpPoints();
        }
        public void LoadPickUpPoints()
        {
            PickUpPoints = new ObservableCollection<PickUpPoint>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT * FROM [Pick-up points]";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    PickUpPoints.Add(new PickUpPoint
                    {
                        PickUpPointID = reader.GetInt32(0),
                        Address = reader.GetString(1),
                        Rating = reader.GetDecimal(2),
                        OrdersLastMonth = reader.GetInt32(3)
                    });
                }
            }
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
        public ObservableCollection<Seller> Sellers { get; set; }

        public void LoadSellers()
        {
            Sellers = new ObservableCollection<Seller>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT * FROM Sellers";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Sellers.Add(new Seller
                    {
                        SellerID = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
        }



        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
        public int Amount { get; set; }
        public string SellerId { get; set; }

        public Command AddProductCommand
        {
            get
            {
                return new Command(() =>
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Check if the ProductId is available
                        string checkAvailabilityQuery = "SELECT COUNT(*) FROM Products WHERE ProductId = @ProductId";
                        SqlCommand checkAvailabilityCommand = new SqlCommand(checkAvailabilityQuery, connection);
                        checkAvailabilityCommand.Parameters.AddWithValue("@ProductId", ProductId);
                        int count = (int)checkAvailabilityCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Артикул занят. Введите другой");
                            return; // Exit the command
                        }

                        // Proceed with the insertion
                        string enableIdentityInsertQuery = "SET IDENTITY_INSERT Products ON";
                        SqlCommand enableIdentityInsertCommand = new SqlCommand(enableIdentityInsertQuery, connection);
                        enableIdentityInsertCommand.ExecuteNonQuery();

                        string sqlQuery = "INSERT INTO Products (ProductId, Name, Price, Rating, Аmount, SellerId) VALUES (@ProductId, @ProductName, @Price, @Rating, @Amount, @SellerId)";
                        SqlCommand command = new SqlCommand(sqlQuery, connection);
                        command.Parameters.AddWithValue("@ProductId", ProductId);
                        command.Parameters.AddWithValue("@ProductName", ProductName);
                        command.Parameters.AddWithValue("@Price", Price);
                        command.Parameters.AddWithValue("@Rating", Rating);
                        command.Parameters.AddWithValue("@Amount", Amount);
                        command.Parameters.AddWithValue("@SellerId", SellerId);

                        command.ExecuteNonQuery();
                    }

                    Products.Add(new Product
                    {
                        ProductID = int.Parse(ProductId),
                        ProductName = ProductName,
                        Price = Price,
                        Rating = Rating,
                        Аmount = Amount,
                        SellerID = SellerId
                    });

                    MessageBox.Show("Продукт добавлен");
                });
            }
        }


        public Command EditProductCommand
        {
            get
            {
                return new Command(() =>
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sqlQuery = "UPDATE Products SET Name = @ProductName, Price = @Price, Rating = @Rating, Аmount = @Amount, SellerId = @SellerId WHERE ProductId = @ProductId";

                        SqlCommand command = new SqlCommand(sqlQuery, connection);
                        command.Parameters.AddWithValue("@ProductId", ProductId);
                        command.Parameters.AddWithValue("@ProductName", ProductName);
                        command.Parameters.AddWithValue("@Price", Price);
                        command.Parameters.AddWithValue("@Rating", Rating);
                        command.Parameters.AddWithValue("@Amount", Amount);
                        command.Parameters.AddWithValue("@SellerId", SellerId);

                        command.ExecuteNonQuery();
                    }

                    var product = Products.FirstOrDefault(p => p.ProductID == int.Parse(ProductId));
                    if (product != null)
                    {
                        product.ProductName = ProductName;
                        product.Price = Price;
                        product.Rating = Rating;
                        product.Аmount = Amount;
                        product.SellerID = SellerId;
                    }
                });
            }
        }



        public Command DeleteProductCommand
        {
            get
            {
                return new Command(() =>
                {
                    
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        
                        // Delete related records in the Orders_Products table
                        string deleteOrdersQuery = "DELETE FROM Orders_Products WHERE ProductId = @ProductId";
                        SqlCommand deleteOrdersCommand = new SqlCommand(deleteOrdersQuery, connection);
                        deleteOrdersCommand.Parameters.AddWithValue("@ProductId", ProductId);
                        deleteOrdersCommand.ExecuteNonQuery();

                        // Delete the product from the Products table
                        string deleteProductQuery = "DELETE FROM Products WHERE ProductId = @ProductId";
                        SqlCommand deleteProductCommand = new SqlCommand(deleteProductQuery, connection);
                        deleteProductCommand.Parameters.AddWithValue("@ProductId", ProductId);
                        deleteProductCommand.ExecuteNonQuery();
                        
                        MessageBox.Show("Вы удалили товар с артикулом  " + ProductId);
                    }

                    // Remove the product from the Products list
                    for (int i = Products.Count - 1; i >= 0; i--)
                    {
                        if (Products[i].ProductID == int.Parse(ProductId))
                        {
                            Products.RemoveAt(i);
                        }
                    }


                });
            }
        }
        public string EmployeeId { get; set; }

        public Command ButSearchEmp
        {
            get
            {
                return new Command(() =>
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sqlQuery1 = "SELECT [Name], [Role], [Salary], [Pick-up-pointId] FROM [Employees] WHERE [EmployeeId] = @EmployeeId";
                        SqlCommand command1 = new SqlCommand(sqlQuery1, connection);
                        command1.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                        SqlDataReader reader = command1.ExecuteReader();

                        if (reader.Read())
                        {
                            string name = reader["Name"].ToString();
                            string role = reader["Role"].ToString();
                            string salary = reader["Salary"].ToString();
                            string pickUpPointId = reader["Pick-up-pointId"].ToString();

                            MessageBox.Show($"Статистика Работника: \nИмя: {name}\nРоль: {role}\nЗарплата: {salary}\nID Пункта выдачи: {pickUpPointId}");
                        }
                        else
                        {
                            MessageBox.Show("Работника с таким ID нет");
                        }
                    }
                });
            }
        }

    }
}
