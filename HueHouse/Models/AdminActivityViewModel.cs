using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HueHouse.Models
{
    public class AdminActivityViewModel
    {
        public string DataType { get; set; } // Loại dữ liệu
        public string Action { get; set; }   // Hành động
        public DateTime CreatedAt { get; set; } // Thời gian tạo
    }
}