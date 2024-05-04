using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MarketPlace.Model
{
    public class Product
    {
        public int ProductID { get; set; } 
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
        public int Аmount { get; set; }
        public string SellerID { get; set; }

    }

}

