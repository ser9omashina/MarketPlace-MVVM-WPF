using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace.Model
{
    public class Employees
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public int Pick_up_pointId { get; set; }
        public decimal Salary { get; set; }
    }
}
