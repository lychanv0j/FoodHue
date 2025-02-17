using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HueHouse.Models
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Price { get; set; }
        public string CategoryName { get; set; }
        public int CategoryID { get; set; } // Thêm thuộc tính CategoryID
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
}