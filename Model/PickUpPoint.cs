using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace.Model
{
    public class PickUpPoint
    {
        public int PickUpPointID { get; set; }
        public string Address { get; set; }
        public decimal Rating { get; set; }
        public int OrdersLastMonth { get; set; }
    }

}
