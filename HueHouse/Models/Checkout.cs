using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HueHouse.Models
{
    public class Checkout
    {
        public Users User { get; set; }  // Dữ liệu người dùng
        public List<Cart> CartItems { get; set; }  // Dữ liệu giỏ hàng
        public decimal TotalAmount { get; set; }  // Tổng tiền giỏ hàng
        public string SelectedPaymentMethod { get; set; } // Phương thức thanh toán được chọn
    }
}