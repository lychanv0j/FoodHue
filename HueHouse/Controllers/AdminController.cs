using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HueHouse.Models;

namespace HueHouse.Controllers
{
    public class AdminController : Controller
    {
        private readonly huefood db;

        public AdminController()
        {
            db = new huefood();   // Initialize database context
        }


        // GET: Admin/Dashboard
        // Trang Dashboard dành cho Admin
        public ActionResult DashBoard()
        {
            // Kiểm tra nếu chưa đăng nhập, chuyển hướng về trang Login
            if (Session["AdminID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Lấy thông tin tổng quan
            var userCount = db.Users.Count();
            var orderCount = db.Orders.Count();
            var productCount = db.Products.Count();
            var totalRevenue = db.Orders
                .Where(o => o.Status == "Đang Xử Lý")
                .Sum(o => (int?)o.TotalAmount) ?? 0;

            // Ghi nhận hoạt động gần đây của Admin
            var adminActivities = db.AdminManagement
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new AdminActivityViewModel
                {
                    DataType = a.DataType,
                    Action = a.Action,
                    CreatedAt = a.CreatedAt
                })
                .ToList();

            // Truyền dữ liệu sang View
            ViewBag.UserCount = userCount;
            ViewBag.OrderCount = orderCount;
            ViewBag.ProductCount = productCount;
            ViewBag.TotalRevenue = totalRevenue;

            // Truyền danh sách hoạt động qua ViewBag
            ViewBag.AdminActivities = adminActivities;

            return View();
        }



        // Hiển thị trang quản lý người dùng
        public ActionResult Manage_Users(string searchQuery = "")
        {
            var users = string.IsNullOrEmpty(searchQuery)
                ? db.Users.ToList()  // Nếu không có tìm kiếm, lấy tất cả người dùng
                : db.Users
                    .Where(u => u.FullName.Contains(searchQuery) || u.Phone.Contains(searchQuery)
                            || u.Address.Contains(searchQuery) || u.Email.Contains(searchQuery)
                            || u.Username.Contains(searchQuery))
                    .ToList(); // Tìm kiếm theo các trường cần thiết

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count;

            return View(); // Trả lại trang Manage_Users
        }

        // Thêm mới người dùng
        [HttpPost]
        public ActionResult AddUser(string fullName, string sex, string phone, string address, string email)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email))
            {
                // Trả về thông báo lỗi nếu thiếu dữ liệu bắt buộc
                ModelState.AddModelError("", "Tên và email là bắt buộc.");
                return RedirectToAction("Manage_Users");
            }

            var user = new Users
            {
                FullName = fullName,
                Sex = sex,
                Phone = phone,
                Address = address,
                Email = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            db.Users.Add(user);
            db.SaveChanges();

            // Sau khi thêm người dùng mới, quay lại trang quản lý
            return RedirectToAction("Manage_Users");
        }

        // Sửa thông tin người dùng
        [HttpPost]
        public ActionResult EditUser(int userId, string fullName, string username)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(username))
            {
                // Trả về thông báo lỗi nếu dữ liệu thiếu
                ModelState.AddModelError("", "Tên và tên đăng nhập là bắt buộc.");
                return RedirectToAction("Manage_Users");
            }

            var user = db.Users.Find(userId);
            if (user != null)
            {
                user.FullName = fullName;
                user.Username = username;
                user.UpdatedAt = DateTime.Now; // Cập nhật thời gian sửa đổi

                db.SaveChanges();
            }

            return RedirectToAction("Manage_Users");
        }

        // Xóa người dùng
        [HttpPost]
        public ActionResult DeleteUser(int userId)
        {
            var user = db.Users.Find(userId);
            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }

            return RedirectToAction("Manage_Users");
        }

        // Tìm kiếm người dùng theo FullName, Phone, Address, Email, Username
        [HttpPost]
        public ActionResult SearchUsers(string searchQuery)
        {
            var users = db.Users
                .Where(u => u.FullName.Contains(searchQuery) || u.Phone.Contains(searchQuery) || u.Address.Contains(searchQuery)
                        || u.Email.Contains(searchQuery) || u.Username.Contains(searchQuery))
                .ToList();

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count;
            return View("ManageUsers"); // Trả lại trang quản lý người dùng với kết quả tìm kiếm
        }



        // Trang Quản lý đơn hàng với chức năng lọc theo trạng thái
        public ActionResult Manage_Orders()
        {
            // Lấy danh sách đơn hàng từ cơ sở dữ liệu
            var orders = db.Orders.ToList();

            // Lấy danh sách người dùng từ cơ sở dữ liệu
            var users = db.Users.ToList();

            // Truyền danh sách người dùng qua ViewData
            ViewData["Users"] = users;

            // Trả về view với danh sách đơn hàng
            return View(orders);
        }

        [HttpPost]
        public ActionResult UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order != null)
            {
                // Cập nhật trạng thái đơn hàng
                order.Status = newStatus;
                order.UpdatedAt = DateTime.Now; // Cập nhật thời gian sửa đổi
                db.SaveChanges();

                // Ghi nhận hoạt động của admin
                var adminId = 1; // Giả sử AdminID = 1 (bạn có thể thay đổi theo cách lấy AdminID phù hợp)
                var activity = new AdminManagement
                {
                    AdminID = adminId,
                    DataType = "Đơn hàng", // Thao tác trên đơn hàng
                    Action = $"Cập nhật trạng thái đơn hàng {orderId} thành {newStatus}", // Mô tả hành động
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                db.AdminManagement.Add(activity); // Thêm hoạt động vào bảng AdminManagement
                db.SaveChanges(); // Lưu hoạt động vào cơ sở dữ liệu
            }

            return RedirectToAction("Manage_Orders");
        }



        // Lấy danh sách sản phẩm và danh mục cho trang quản lý sản phẩm
        public ActionResult Manage_Products()
        {
            var categories = db.Categories.ToList();

            var products = db.Products
                             .Join(db.ProductDetails, p => p.ProductID, pd => pd.ProductID, (p, pd) => new ProductViewModel
                             {
                                 ProductID = p.ProductID,
                                 ProductName = p.ProductName,
                                 ProductImage = p.ProductImage,
                                 Price = p.Price,
                                 Quantity = pd.Quantity,
                                 Status = pd.Status,
                                 CategoryName = p.Category.CategoryName,
                                 CategoryID = p.Category.CategoryID,
                                 Description = pd.Description
                             })
                             .ToList();

            ViewBag.Categories = categories; // Danh sách danh mục sản phẩm
            return View(products);           // Truyền danh sách sản phẩm qua Model
        }

        // Thêm sản phẩm mới
        [HttpPost]
        public ActionResult AddProduct(string ProductName, HttpPostedFileBase ProductImage, int Price, int CategoryID, string Description)
        {
            // Lưu file ảnh
            string filePath = null;
            if (ProductImage != null && ProductImage.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + "_" + ProductImage.FileName;
                filePath = "~/Images/" + fileName;
                ProductImage.SaveAs(Server.MapPath(filePath));
            }

            var newProduct = new Products
            {
                ProductName = ProductName,
                ProductImage = filePath, // Lưu đường dẫn ảnh
                Price = Price,
                CategoryID = CategoryID
            };

            db.Products.Add(newProduct);
            db.SaveChanges();

            var newProductDetail = new ProductDetails
            {
                ProductID = newProduct.ProductID,
                Quantity = 0, // Số lượng mặc định
                Status = "Còn Hàng",
                Description = Description
            };

            db.ProductDetails.Add(newProductDetail);
            db.SaveChanges();

            return RedirectToAction("Manage_Products");
        }

        // Cập nhật số lượng sản phẩm
        [HttpPost]
        public ActionResult UpdateQuantity(int ProductID, int Quantity)
        {
            var productDetail = db.ProductDetails.FirstOrDefault(pd => pd.ProductID == ProductID);
            if (productDetail != null)
            {
                productDetail.Quantity = Quantity;
                productDetail.Status = Quantity > 0 ? "Còn Hàng" : "Hết Hàng"; // Cập nhật trạng thái
                productDetail.UpdatedAt = DateTime.Now; // Thời gian cập nhật
                db.SaveChanges();
            }

            return RedirectToAction("Manage_Products");
        }

        // Xóa sản phẩm
        [HttpPost]
        public ActionResult DeleteProduct(int ProductID)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductID == ProductID);
            var productDetail = db.ProductDetails.FirstOrDefault(pd => pd.ProductID == ProductID);

            if (product != null && productDetail != null)
            {
                db.ProductDetails.Remove(productDetail); // Xóa chi tiết sản phẩm
                db.Products.Remove(product);            // Xóa sản phẩm
                db.SaveChanges();
            }

            return RedirectToAction("Manage_Products");
        }


        public ActionResult View_Feedback()
        {
            return View();
        }
    }
}