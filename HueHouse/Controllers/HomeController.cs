using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HueHouse.Models;
using System.Data.Entity;

namespace HueHouse.Controllers
{
    public class HomeController : Controller
    {
        private readonly huefood db;

        public HomeController()
        {
            db = new huefood();   // Khởi tạo database
        }



        // Trang Hướng Dẫn Thanh Toán
        public ActionResult Payment_Instructions()
        {
            return View();
        }



        // Trang Hướng Dẫn Đặt - Mua Hàng 
        public ActionResult Order_Instructions()
        {
            return View();
        }



        // Trang Giới Thiệu
        public ActionResult About()
        {
            return View();
        }



        // Trang Liên Hệ & Góp Ý
        public ActionResult Contact()
        {
            return View();
        }



        // HTTP GET: /Home/Register
        public ActionResult Register()
        {
            // Trả về View cho trang đăng ký người dùng
            // Phương thức GET này chỉ hiển thị giao diện để người dùng nhập thông tin đăng ký
            return View();
        }

        // HTTP POST: /Home/Register
        [HttpPost]
        public ActionResult Register(FormCollection form)
        {
            // Tạo một đối tượng mới của lớp Users
            // Đây là đối tượng dùng để lưu trữ thông tin người dùng khi họ đăng ký
            Users user = new Users();

            // Gán giá trị từ form vào đối tượng user
            // Sử dụng FormCollection để lấy dữ liệu từ form và gán vào các thuộc tính của đối tượng
            user.FullName = form["FullName"];      // Họ và tên
            user.Sex = form["Sex"];                // Giới tính
            user.Phone = form["Phone"];            // Số điện thoại
            user.Address = form["Address"];        // Địa chỉ
            user.Email = form["Email"];            // Email
            user.Username = form["Username"];      // Tên đăng nhập
            user.Password = form["Password"];      // Mật khẩu
            user.CreatedAt = DateTime.Now;         // Ngày tạo tài khoản (thời gian hiện tại)
            user.UpdatedAt = DateTime.Now;         // Ngày cập nhật tài khoản (thời gian hiện tại)

            // Thêm đối tượng user vào cơ sở dữ liệu
            // db.Users.Add(user); sẽ thêm người dùng vào bảng Users trong cơ sở dữ liệu
            db.Users.Add(user);

            // Lưu thay đổi vào cơ sở dữ liệu
            // db.SaveChanges() sẽ lưu tất cả thay đổi (trong trường hợp này là thêm mới người dùng) vào cơ sở dữ liệu
            db.SaveChanges();

            // Sau khi đăng ký thành công, chuyển hướng đến trang đăng nhập
            // return RedirectToAction("Login"); sẽ chuyển người dùng đến phương thức Login trong controller Home
            return RedirectToAction("Login");
        }



        // HTTP GET: /Home/LogIn
        public ActionResult LogIn()
        {
            // Trả về View cho trang đăng nhập
            // Phương thức GET sẽ hiển thị trang Login để người dùng nhập thông tin đăng nhập
            return View();
        }

        // HTTP POST /Home/LogIn
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Kiểm tra trong bảng Users (người dùng thông thường)
            var users = db.Users.Where(u => u.Username == username && u.Password == password).ToList();
            // Dòng này thực hiện truy vấn tìm kiếm tất cả người dùng trong bảng `Users` với điều kiện tên đăng nhập và mật khẩu trùng khớp.
            // Kết quả trả về là một danh sách người dùng (dù chỉ có một người dùng hợp lệ, vì vậy dùng `ToList()` để lấy tất cả).

            // Kiểm tra trong bảng Admins (quản trị viên)
            var admin = db.Admin.FirstOrDefault(a => a.UserName == username && a.Password == password);
            // Dòng này kiểm tra trong bảng `Admin` xem có tài khoản quản trị viên nào có tên đăng nhập và mật khẩu khớp với yêu cầu không.
            // `FirstOrDefault` sẽ trả về đối tượng quản trị viên đầu tiên tìm thấy hoặc `null` nếu không có.

            // Kiểm tra nếu tìm thấy người dùng thông thường
            if (users != null && users.Any()) // Nếu là người dùng thông thường
            {
                var user = users.First(); // Giả sử chỉ có 1 người dùng duy nhất trong trường hợp hợp lệ
                if (user != null)
                {
                    // Đăng nhập thành công cho người dùng
                    Session["UserID"] = user.UserID;        // Lưu ID người dùng vào session
                    Session["FullName"] = user.FullName;    // Lưu họ và tên vào session
                    Session["Sex"] = user.Sex;              // Lưu giới tính vào session
                    Session["Phone"] = user.Phone;          // Lưu số điện thoại vào session
                    Session["Address"] = user.Address;      // Lưu địa chỉ vào session
                    Session["Email"] = user.Email;          // Lưu email vào session
                    Session["Username"] = user.Username;    // Lưu tên đăng nhập vào session
                    Session["Password"] = user.Password;    // Lưu mật khẩu vào session

                    // Kiểm tra nếu có RedirectUrl trong Session
                    var redirectUrl = Session["RedirectUrl"];
                    if (redirectUrl != null)
                    {
                        // Reset lại RedirectUrl sau khi sử dụng
                        Session["RedirectUrl"] = null;    // Xóa giá trị RedirectUrl trong session
                        return Redirect(redirectUrl.ToString()); // Chuyển hướng đến trang giỏ hàng (nếu có)
                    }

                    // Nếu không có RedirectUrl, chuyển về trang chủ
                    return RedirectToAction("Index", new { id = user.UserID }); // Chuyển hướng đến trang chủ và truyền ID người dùng
                }
            }

            // Nếu người dùng không phải là người dùng thông thường, kiểm tra là quản trị viên
            else if (admin != null) // Nếu là quản trị viên
            {
                // Đăng nhập thành công cho quản trị viên
                Session["AdminID"] = admin.AdminID;          // Lưu ID quản trị viên vào session
                Session["FullName"] = admin.FullName;        // Lưu họ và tên của quản trị viên vào session
                Session["Username"] = admin.UserName;        // Lưu tên đăng nhập của quản trị viên vào session
                Session["Password"] = admin.Password;        // Lưu mật khẩu của quản trị viên vào session

                // Chuyển hướng đến trang quản trị của admin
                return RedirectToAction("DashBoard", "Admin"); // Chuyển hướng đến trang quản lý của admin và truyền ID của admin
            }
            else
            {
                // Đăng nhập thất bại, hiển thị lỗi
                ViewBag.Message = "Tên đăng nhập hoặc mật khẩu không đúng hoặc không tồn tại."; // Gửi thông báo lỗi về View
            }

            return View(); // Trả về View Login nếu đăng nhập thất bại
        }



        // Trang chủ - Hiển thị danh sách sản phẩm nổi bật
        public ActionResult Index()
        {
            // Lấy danh sách sản phẩm nổi bật - Bán Chạy
            // Truy vấn cơ sở dữ liệu để lấy những sản phẩm có thuộc tính HighlightType là "Bán Chạy"
            // Kết hợp bảng HighlightedProducts và Products dựa trên ProductID
            // Lọc ra những sản phẩm có loại "Bán Chạy" và lấy tối đa 20 sản phẩm
            var topSellingProducts = (from hp in db.HighlightedProducts
                                      join p in db.Products on hp.ProductID equals p.ProductID
                                      where hp.HighlightType == "Bán Chạy"
                                      select p).Take(10).ToList();

            // Lấy danh sách sản phẩm mới
            // Truy vấn cơ sở dữ liệu để lấy những sản phẩm có thuộc tính HighlightType là "Sản Phẩm Mới"
            // Kết hợp bảng HighlightedProducts và Products dựa trên ProductID
            // Lọc ra những sản phẩm có loại "Sản Phẩm Mới" và lấy tối đa 20 sản phẩm
            var newProducts = (from hp in db.HighlightedProducts
                               join p in db.Products on hp.ProductID equals p.ProductID
                               where hp.HighlightType == "Sản Phẩm Mới"
                               select p).Take(10).ToList();

            // Lấy danh sách sản phẩm hot nhất tháng
            // Truy vấn cơ sở dữ liệu để lấy những sản phẩm có thuộc tính HighlightType là "Hot Nhất Tháng"
            // Kết hợp bảng HighlightedProducts và Products dựa trên ProductID
            // Lọc ra những sản phẩm có loại "Hot Nhất Tháng" và lấy tối đa 20 sản phẩm
            var hotProducts = (from hp in db.HighlightedProducts
                               join p in db.Products on hp.ProductID equals p.ProductID
                               where hp.HighlightType == "Hot Nhất Tháng"
                               select p).Take(10).ToList();

            // Đưa dữ liệu vào ViewBag để sử dụng trong View (Index.cshtml)
            // Các sản phẩm nổi bật theo từng loại sẽ được truyền qua ViewBag để hiển thị trên giao diện
            ViewBag.TopSellingProducts = topSellingProducts;
            ViewBag.NewProducts = newProducts;
            ViewBag.HotProducts = hotProducts;

            // Trả về View tương ứng với trang chủ (Index.cshtml)
            return View();
        }



        // Trang Thông Tin Cá Nhân
        // Phương thức này xử lý yêu cầu GET từ trang thông tin cá nhân của người dùng.
        public ActionResult My_Account()
        {
            // Lấy UserID từ Session
            // Kiểm tra nếu người dùng chưa đăng nhập (Session["UserID"] là null)
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Home"); // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            }

            // Lấy UserID từ session để tìm thông tin người dùng trong cơ sở dữ liệu
            int userID = (int)Session["UserID"];
            var user = db.Users.Find(userID); // Tìm người dùng trong cơ sở dữ liệu

            // Nếu không tìm thấy người dùng trong database
            if (user == null)
            {
                TempData["Message"] = "Không tìm thấy người dùng."; // Thông báo lỗi cho người dùng
                return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chủ
            }

            return View(user); // Truyền thông tin người dùng (model) sang view để hiển thị
        }

        // Phương thức này xử lý yêu cầu POST từ form cập nhật thông tin người dùng.
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống tấn công CSRF (Cross-Site Request Forgery)
        public ActionResult My_Account(string Sex, string Phone, string Address, string Email, string Password, string ConfirmPassword)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Home"); // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            }

            // Lấy UserID từ session
            int userID = (int)Session["UserID"];
            var user = db.Users.Find(userID); // Tìm người dùng trong cơ sở dữ liệu

            // Nếu không tìm thấy người dùng trong database
            if (user == null)
            {
                TempData["Message"] = "Không tìm thấy người dùng."; // Thông báo lỗi cho người dùng
                return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chủ
            }

            // Cập nhật thông tin người dùng
            user.Sex = Sex; // Cập nhật giới tính
            user.Phone = Phone; // Cập nhật số điện thoại
            user.Address = Address; // Cập nhật địa chỉ
            user.Email = Email; // Cập nhật email


            // Kiểm tra và cập nhật mật khẩu nếu được cung cấp
            // Trường hợp người dùng nhập mật khẩu xác nhận (ConfirmPassword) nhưng không nhập mật khẩu mới (Password)
            if (!string.IsNullOrEmpty(ConfirmPassword) && string.IsNullOrEmpty(Password))
            {
                // Gửi thông báo lỗi cho người dùng rằng họ cần nhập mật khẩu mới trước khi xác nhận
                TempData["Message"] = "Bạn chưa nhập mật khẩu mới để xác nhận, vui lòng nhập mật khẩu mới trước khi xác nhận.";
                return View(user); // Trả về lại trang thông tin người dùng với thông báo lỗi
            }

            // Trường hợp người dùng nhập mật khẩu mới (Password)
            if (!string.IsNullOrEmpty(Password))
            {
                // Kiểm tra xem mật khẩu mới và mật khẩu xác nhận có trùng khớp không
                if (Password == ConfirmPassword)
                {
                    user.Password = Password; // Cập nhật mật khẩu mới cho người dùng (Lưu ý: cần mã hóa mật khẩu trước khi lưu trong thực tế)
                }
                else
                {
                    // Nếu mật khẩu xác nhận không khớp, gửi thông báo lỗi cho người dùng
                    TempData["Message"] = "Mật khẩu xác nhận không khớp.";
                    return View(user); // Trả về lại trang thông tin người dùng với thông báo lỗi
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges(); // Lưu thay đổi thông tin người dùng vào database

            // Cập nhật session với thông tin mới (Cập nhật thông tin trong session để dùng sau)
            Session["Sex"] = Sex;
            Session["Phone"] = Phone;
            Session["Address"] = Address;
            Session["Email"] = Email;

            // Gửi thông báo thành công
            TempData["Message"] = "Thông tin tài khoản được cập nhật thành công."; // Hiển thị thông báo thành công
            return RedirectToAction("My_Account"); // Chuyển hướng người dùng lại trang thông tin cá nhân
        }



        // Action tìm kiếm món ăn
        public ActionResult Search(string query)
        {
            // Kiểm tra nếu từ khóa tìm kiếm rỗng hoặc null
            if (string.IsNullOrEmpty(query))
            {
                // Nếu từ khóa tìm kiếm trống, trả về danh sách rỗng (JSON)
                return Json(new List<FoodItem>(), JsonRequestBehavior.AllowGet);
            }

            // Tìm kiếm trong bảng Products theo tên món ăn, với điều kiện tên món ăn chứa từ khóa tìm kiếm
            var result = db.Products
                           .Where(p => p.ProductName.Contains(query)) // Lọc các món ăn có tên chứa từ khóa tìm kiếm
                           .Select(p => new FoodItem // Chọn các trường cần thiết để trả về kết quả
                           {
                               FooditemID = p.ProductID, // Lấy mã món ăn từ bảng Products
                               FoodName = p.ProductName, // Lấy tên món ăn từ bảng Products
                               ImageUrl = p.ProductImage, // Lấy hình ảnh của món ăn từ bảng Products
                               Price = p.Price // Lấy giá món ăn từ bảng Products
                           })
                           .ToList(); // Chuyển đổi kết quả truy vấn thành danh sách

            // Trả về kết quả dưới dạng JSON để giao tiếp với client (dễ dàng hiển thị trong giao diện người dùng)
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        // Action lấy danh sách sản phẩm
        public ActionResult Product()
        {
            // Sử dụng cú pháp 'using' để đảm bảo rằng đối tượng db sẽ được dispose (giải phóng tài nguyên) khi không còn sử dụng
            using (var db = new huefood())
            {
                // Lấy danh sách món ăn được nhóm theo danh mục
                var groupedProducts = db.Products
                                        // Nhóm các sản phẩm theo tên danh mục của mỗi sản phẩm (CategoryName)
                                        .GroupBy(p => p.Category.CategoryName)
                                        // Chuyển đổi kết quả nhóm thành một danh sách (List)
                                        .ToList();

                // Truyền dữ liệu vào View: trả về View với biến groupedProducts
                // groupedProducts là một danh sách các nhóm sản phẩm, trong đó mỗi nhóm có một tên danh mục và một danh sách sản phẩm thuộc danh mục đó
                return View(groupedProducts);
            }
        }



        // Action hiện chi tiết sản phẩm
        public ActionResult Product_Details(int id)
        {
            // Sử dụng câu lệnh using để mở kết nối với cơ sở dữ liệu thông qua model 'huehousemodel'
            using (var db = new huefood())
            {
                // Lấy chi tiết sản phẩm từ bảng ProductDetails và bảng Product
                // Sử dụng phương thức Include để lấy thông tin liên kết với bảng Products (liên kết giữa bảng ProductDetails và bảng Products)
                var productDetails = db.ProductDetails
                                       .Include(pd => pd.Products) // Sử dụng đúng navigation property để lấy thông tin sản phẩm liên quan
                                       .FirstOrDefault(pd => pd.ProductID == id);  // Tìm sản phẩm theo ID (dựa trên tham số 'id' truyền vào)

                // Kiểm tra nếu không tìm thấy sản phẩm trong cơ sở dữ liệu
                if (productDetails == null)
                {
                    // Nếu không tìm thấy sản phẩm, trả về lỗi 404 với thông báo "Không tìm thấy sản phẩm."
                    return HttpNotFound("Không tìm thấy sản phẩm.");
                }

                // Trả về view (giao diện) với thông tin chi tiết sản phẩm, thông qua đối tượng 'productDetails'
                // 'productDetails' sẽ được truyền vào view để hiển thị các thông tin chi tiết sản phẩm
                return View(productDetails);
            }
        }



        // Action thêm sản phẩm vào giỏ hàng
        [HttpPost] // Đánh dấu phương thức này xử lý các yêu cầu HTTP POST từ form
        public ActionResult AddToCart(int productId, int quantity = 1) // Nhận productId (ID sản phẩm) và quantity (số lượng) từ form
        {
            using (var db = new huefood()) // Sử dụng context "huehousemodel" để thao tác với cơ sở dữ liệu
            {
                // Kiểm tra trạng thái đăng nhập (nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập)
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Login", "Home"); // Điều hướng người dùng đến trang đăng nhập
                }

                // Lấy thông tin người dùng từ Session
                var userId = (int)Session["UserID"]; // Lưu ID người dùng đã đăng nhập để xác định giỏ hàng của họ

                // Lấy sản phẩm từ cơ sở dữ liệu theo productId
                var product = db.Products.FirstOrDefault(p => p.ProductID == productId);
                if (product == null) // Nếu không tìm thấy sản phẩm trong cơ sở dữ liệu
                {
                    return HttpNotFound("Không tìm thấy sản phẩm."); // Trả về lỗi 404 nếu sản phẩm không tồn tại
                }

                // Lấy mô tả của sản phẩm từ bảng ProductDetails
                var productDetail = db.ProductDetails.SingleOrDefault(pd => pd.ProductID == productId);

                // Kiểm tra số lượng khả dụng trong bảng ProductDetails
                if (productDetail.Quantity < quantity) // Nếu số lượng yêu cầu lớn hơn số lượng có sẵn
                {
                    TempData["ErrorMessage"] = "Không đủ số lượng sản phẩm trong kho."; // Lưu thông báo lỗi tạm thời
                    return RedirectToAction("Product_Details", new { id = productId }); // Truyền tham số id để quay lại trang chi tiết sản phẩm
                }

                // Giảm số lượng khả dụng trong ProductDetails khi thêm vào giỏ hàng
                productDetail.Quantity -= quantity; // Cập nhật số lượng còn lại sau khi giảm

                // Lấy mô tả của sản phẩm nếu có, nếu không thì để trống
                string description = productDetail != null ? productDetail.Description : string.Empty;

                // Kiểm tra xem sản phẩm đã có trong giỏ hàng của người dùng chưa
                var cartItem = db.Cart.FirstOrDefault(c => c.ProductID == productId && c.UserID == userId);
                if (cartItem != null) // Nếu sản phẩm đã có trong giỏ hàng
                {
                    cartItem.Quantity += quantity; // Tăng số lượng sản phẩm trong giỏ
                    cartItem.UpdatedAt = DateTime.Now; // Cập nhật thời gian sửa đổi giỏ hàng
                }
                else // Nếu sản phẩm chưa có trong giỏ hàng
                {
                    // Thêm sản phẩm mới vào giỏ hàng
                    db.Cart.Add(new Cart
                    {
                        UserID = userId, // ID người dùng
                        ProductID = productId, // ID sản phẩm
                        ProductName = product.ProductName, // Tên sản phẩm từ bảng Products
                        ProductImage = product.ProductImage, // Hình ảnh sản phẩm từ bảng Products
                        Description = description, // Mô tả sản phẩm từ bảng ProductDetails
                        Price = product.Price, // Giá sản phẩm từ bảng Products
                        Quantity = quantity, // Số lượng thêm vào giỏ hàng
                        CreatedAt = DateTime.Now, // Thời gian tạo mục giỏ hàng
                        UpdatedAt = DateTime.Now  // Thời gian cập nhật mục giỏ hàng
                    });
                }

                db.SaveChanges(); // Lưu tất cả thay đổi vào cơ sở dữ liệu

                // Chuyển hướng về trang giỏ hàng sau khi thêm sản phẩm thành công
                return RedirectToAction("Shopping_Cart", "Home");
            }
        }



        // Action hiển thị giỏ hàng
        public ActionResult Shopping_Cart()
        {
            // Kiểm tra trạng thái đăng nhập (nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập)
            // Nếu Session["UserID"] là null, có nghĩa là người dùng chưa đăng nhập
            if (Session["UserID"] == null)
            {
                // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
                return RedirectToAction("Login", "Home");
            }

            // Lấy UserID từ session để biết người dùng hiện tại
            var userId = (int)Session["UserID"];

            // Sử dụng đối tượng huehousemodel để truy vấn cơ sở dữ liệu
            using (var db = new huefood())
            {
                // Lấy danh sách các sản phẩm trong giỏ hàng của người dùng từ cơ sở dữ liệu
                // Điều kiện lọc theo UserID để chỉ lấy các sản phẩm của người dùng hiện tại
                var cartItems = db.Cart.Where(c => c.UserID == userId).ToList();

                // Trả về view hiển thị giỏ hàng và truyền dữ liệu cartItems (danh sách các sản phẩm trong giỏ hàng) đến view
                return View(cartItems);
            }
        }



        // Action xử lý yêu cầu tăng/giảm số lượng sản phẩm trong giỏ hàng
        [HttpPost]  // Chỉ định đây là hành động POST, được gọi khi có yêu cầu gửi dữ liệu đến server
        public ActionResult UpdateQuantity(int cartId, string actionType)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Home");  // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            }

            // Sử dụng đối tượng db để kết nối với cơ sở dữ liệu
            using (var db = new huefood())
            {
                // Tìm sản phẩm trong giỏ hàng theo cartId
                var cartItem = db.Cart.SingleOrDefault(c => c.CartID == cartId);

                if (cartItem != null)  // Nếu sản phẩm trong giỏ hàng tồn tại
                {
                    // Tìm chi tiết sản phẩm trong bảng ProductDetails để cập nhật số lượng
                    var productDetail = db.ProductDetails.SingleOrDefault(p => p.ProductID == cartItem.ProductID);

                    // Kiểm tra loại hành động (tăng hay giảm số lượng)
                    if (actionType == "increase")
                    {
                        // Nếu hành động là "increase" (tăng số lượng)
                        if (productDetail.Quantity > 0)  // Chỉ tăng số lượng nếu còn hàng trong kho
                        {
                            cartItem.Quantity++;  // Tăng số lượng sản phẩm trong giỏ hàng
                            productDetail.Quantity--;  // Giảm số lượng sản phẩm có sẵn trong kho
                        }
                    }
                    else if (actionType == "decrease" && cartItem.Quantity > 1)
                    {
                        // Nếu hành động là "decrease" (giảm số lượng) và số lượng trong giỏ hàng còn lớn hơn 1
                        cartItem.Quantity--;  // Giảm số lượng sản phẩm trong giỏ hàng
                        productDetail.Quantity++;  // Tăng số lượng sản phẩm có sẵn trong kho
                    }

                    // Cập nhật thời gian sửa đổi của mục giỏ hàng
                    cartItem.UpdatedAt = DateTime.Now;
                    db.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu
                }
            }

            // Sau khi thực hiện cập nhật, chuyển hướng đến trang giỏ hàng
            return RedirectToAction("Shopping_Cart");
        }



        // Action xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public ActionResult RemoveFromCart(int cartItemId)
        {
            // Khởi tạo context để thao tác với cơ sở dữ liệu
            using (var db = new huefood())
            {
                // Lấy mục giỏ hàng từ cơ sở dữ liệu theo cartItemId
                var cartItem = db.Cart.SingleOrDefault(c => c.CartID == cartItemId);

                // Kiểm tra nếu không tìm thấy mục giỏ hàng với cartItemId đã cho
                if (cartItem == null)
                {
                    // Trả về lỗi 404 nếu không tìm thấy sản phẩm trong giỏ hàng
                    return HttpNotFound("Không tìm thấy sản phẩm trong giỏ hàng.");
                }

                // Lấy thông tin chi tiết sản phẩm từ bảng ProductDetails
                var productDetail = db.ProductDetails.SingleOrDefault(pd => pd.ProductID == cartItem.ProductID);

                // Nếu tìm thấy sản phẩm trong bảng ProductDetails
                if (productDetail != null)
                {
                    // Khôi phục lại số lượng sản phẩm trong kho bằng cách cộng thêm số lượng sản phẩm đã được thêm vào giỏ hàng
                    productDetail.Quantity += cartItem.Quantity;
                }

                // Xóa mục giỏ hàng từ cơ sở dữ liệu
                db.Cart.Remove(cartItem);

                // Lưu các thay đổi vào cơ sở dữ liệu (xóa mục giỏ hàng và cập nhật số lượng sản phẩm)
                db.SaveChanges();
            }

            // Sau khi xóa sản phẩm khỏi giỏ hàng, chuyển hướng người dùng về trang giỏ hàng
            return RedirectToAction("Shopping_Cart", "Home");
        }



        // GET: Checkout
        public ActionResult Checkout()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa, nếu chưa thì chuyển hướng đến trang đăng nhập
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Lấy ID người dùng từ Session
            int userId = (int)Session["UserID"];

            // Truy vấn dữ liệu người dùng từ cơ sở dữ liệu
            var user = db.Users.SingleOrDefault(u => u.UserID == userId);

            // Lấy danh sách các sản phẩm trong giỏ hàng của người dùng
            var cartItems = db.Cart.Where(c => c.UserID == userId).ToList();

            // Tính tổng tiền giỏ hàng dựa trên số lượng và giá của từng sản phẩm
            var totalAmount = cartItems.Sum(c => c.Quantity * c.Price);

            // Tạo đối tượng Checkout và gán các giá trị cần thiết vào mô hình
            var model = new Checkout
            {
                User = user, // Dữ liệu người dùng
                CartItems = cartItems, // Dữ liệu giỏ hàng
                TotalAmount = totalAmount, // Tổng số tiền
                SelectedPaymentMethod = "COD" // Phương thức thanh toán mặc định (Thanh toán khi nhận hàng)
            };

            // Trả về view với mô hình Checkout
            return View(model);
        }

        // POST: SubmitOrder
        [HttpPost]
        public ActionResult SubmitOrder(string FullName, string Phone, string Address, string PaymentMethod)
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa, nếu chưa thì chuyển hướng đến trang đăng nhập
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Kiểm tra xem người dùng đã chọn phương thức thanh toán chưa
            if (string.IsNullOrEmpty(PaymentMethod))
            {
                TempData["ErrorMessage"] = "Vui lòng chọn phương thức thanh toán.";
                return RedirectToAction("Checkout");
            }

            // Lấy ID người dùng từ Session
            int userId = (int)Session["UserID"];

            // Bắt đầu một giao dịch để đảm bảo tính toàn vẹn dữ liệu (rollback nếu có lỗi)
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Tạo đơn hàng mới
                    var order = new Orders
                    {
                        UserID = userId,  // Lưu ID người dùng
                        OrderDate = DateTime.Now,  // Ngày đặt hàng
                        Status = "Đang Xử Lý",  // Trạng thái đơn hàng
                        TotalAmount = db.Cart.Where(c => c.UserID == userId).Sum(c => c.Quantity * c.Price),  // Tổng tiền đơn hàng
                        CreatedAt = DateTime.Now,  // Ngày tạo đơn hàng
                        UpdatedAt = DateTime.Now  // Ngày cập nhật đơn hàng
                    };

                    // Thêm đơn hàng vào cơ sở dữ liệu
                    db.Orders.Add(order);
                    db.SaveChanges();

                    // Lấy danh sách các sản phẩm trong giỏ hàng
                    var cartItems = db.Cart.Where(c => c.UserID == userId).ToList();

                    // Thêm chi tiết đơn hàng cho từng sản phẩm trong giỏ
                    foreach (var item in cartItems)
                    {
                        db.OrderDetails.Add(new OrderDetails
                        {
                            OrderID = order.OrderID,  // ID đơn hàng
                            ProductID = item.ProductID,  // ID sản phẩm
                            Quantity = item.Quantity,  // Số lượng sản phẩm
                            Price = item.Price,  // Giá sản phẩm
                            CreatedAt = DateTime.Now,  // Ngày tạo chi tiết đơn hàng
                            UpdatedAt = DateTime.Now  // Ngày cập nhật chi tiết đơn hàng
                        });

                        // Xóa sản phẩm khỏi giỏ hàng sau khi đã đặt
                        db.Cart.Remove(item);
                    }

                    // Thêm bản ghi thanh toán cho đơn hàng
                    db.Payments.Add(new Payments
                    {
                        OrderID = order.OrderID,  // ID đơn hàng
                        PaymentMethod = PaymentMethod,  // Phương thức thanh toán người dùng chọn
                        Status = PaymentMethod == "COD" ? "Thành Công" : "Chờ Xử Lý",  // Trạng thái thanh toán
                        CreatedAt = DateTime.Now,  // Ngày tạo thanh toán
                        UpdatedAt = DateTime.Now  // Ngày cập nhật thanh toán
                    });

                    // Cập nhật thông tin người dùng với thông tin mới (họ tên, điện thoại, địa chỉ)
                    var user = db.Users.SingleOrDefault(u => u.UserID == userId);
                    if (user != null)
                    {
                        user.FullName = FullName;  // Cập nhật họ tên
                        user.Phone = Phone;  // Cập nhật số điện thoại
                        user.Address = Address;  // Cập nhật địa chỉ giao hàng
                    }

                    // Lưu tất cả thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();

                    // Xác nhận giao dịch (commit)
                    transaction.Commit();

                    // Hiển thị thông báo thành công và chuyển hướng đến trang xác nhận đơn hàng
                    TempData["SuccessMessage"] = "Đặt hàng thành công!";
                    return RedirectToAction("Order_Confirmation", "Home", new { orderId = order.OrderID });
                }
                catch (Exception)
                {
                    // Nếu có lỗi xảy ra trong quá trình thực hiện, rollback giao dịch và hiển thị thông báo lỗi
                    transaction.Rollback();
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình thanh toán ";
                    return RedirectToAction("Checkout", "Home");
                }
            }
        }



        // Action hiển thị trang Xác nhận đặt hàng
        public ActionResult Order_Confirmation(int orderId)
        {
            // Lấy thông tin đơn hàng từ db
            var order = db.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order == null)
            {
                // Nếu không tìm thấy đơn hàng, trả về lỗi 404
                return HttpNotFound("Không tìm thấy đơn hàng.");
            }

            // Lấy thông tin người dùng từ UserID trong bảng Orders
            var user = db.Users.FirstOrDefault(u => u.UserID == order.UserID);
            if (user == null)
            {
                // Nếu không tìm thấy người dùng, trả về lỗi 404 hoặc xử lý khác
                return HttpNotFound("Không tìm thấy người dùng.");
            }

            // Lấy chi tiết đơn hàng từ bảng OrderDetails
            var orderDetails = db.OrderDetails.Where(od => od.OrderID == orderId).ToList();

            // Lấy thông tin thanh toán từ bảng Payments
            var payment = db.Payments.FirstOrDefault(p => p.OrderID == orderId);
            if (payment == null)
            {
                // Nếu không tìm thấy thông tin thanh toán, trả về lỗi 404
                return HttpNotFound("Không tìm thấy thông tin thanh toán.");
            }

            // Lấy thông tin sản phẩm từ bảng Products, sử dụng danh sách ProductID có trong OrderDetails
            var productIds = orderDetails.Select(od => od.ProductID).ToList();
            var products = db.Products.Where(p => productIds.Contains(p.ProductID)).ToList();

            // Gửi dữ liệu đến View thông qua ViewData để hiển thị trên giao diện người dùng
            ViewData["User"] = user;               // Gửi thông tin người dùng đến view
            ViewData["Order"] = order;             // Gửi thông tin đơn hàng đến view
            ViewData["OrderDetails"] = orderDetails; // Gửi chi tiết đơn hàng (sản phẩm và số lượng) đến view
            ViewData["Payment"] = payment;         // Gửi thông tin thanh toán đến view
            ViewData["Products"] = products;       // Gửi thông tin sản phẩm (danh sách sản phẩm) đến view

            // Trả về View hiển thị thông tin xác nhận đơn hàng
            return View();
        }



        // Action hiển thị trang Lịch sử mua hàng
        public ActionResult Order_History()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa (kiểm tra giá trị Session["UserID"]).
            if (Session["UserID"] == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang Login để người dùng đăng nhập.
                return RedirectToAction("Login", "Home");
            }

            // Lấy ID của người dùng từ Session, giả định rằng ID người dùng đã được lưu trong Session sau khi đăng nhập.
            int userId = (int)Session["UserID"];

            // Truy vấn từ bảng Orders để lấy danh sách đơn hàng của người dùng dựa trên UserID.
            // Sử dụng LINQ (Language Integrated Query) để lọc, bao gồm các bảng liên quan và sắp xếp kết quả.
            var orders = db.Orders
                .Where(o => o.UserID == userId) // Lọc các đơn hàng chỉ thuộc về người dùng có UserID tương ứng.
                .Include(o => o.OrderDetails)   // Bao gồm các chi tiết đơn hàng (OrderDetails) liên kết với đơn hàng.
                .Include(o => o.OrderDetails.Select(od => od.Products)) // Bao gồm thông tin các sản phẩm (Products) trong OrderDetails.
                .OrderByDescending(o => o.OrderDate) // Sắp xếp các đơn hàng theo thứ tự giảm dần dựa trên ngày đặt hàng (OrderDate).
                .ToList(); // Thực thi truy vấn và trả về danh sách các đơn hàng.

            // Truyền danh sách đơn hàng vào ViewData để có thể sử dụng trong View.
            // Dữ liệu này sẽ được dùng để hiển thị danh sách đơn hàng trong trang "Order History".
            ViewData["Orders"] = orders;

            // Trả về View hiển thị kết quả.
            return View();
        }



        // Action Đăng Xuất
        [HttpGet]
        public ActionResult Logout()
        {
            // Xoá thông tin người dùng
            Session["UserID"] = null;    // Xoá ID của người dùng
            Session["FullName"] = null;      // Xoá Họ và Tên của người dùng
            Session["Sex"] = null;           // Xoá Giới tính của người dùng
            Session["Phone"] = null;         // Xoá SĐT của người dùng
            Session["Address"] = null;       // Xoá Địa chỉ của người dùng
            Session["Email"] = null;         // Xoá Email của người dùng
            Session["Username"] = null;      // Xoá Tên đăng nhập của người dùng
            Session["Password"] = null;      // Xoá Mật khẩu của người dùng

            // Xoá thông tin admin
            Session["AdminID"] = null;       // Xoá ID của admin
            Session["FullName"] = null; // Xoá Họ và Tên của admin
            Session["Username"] = null; // Xoá Tên đăng nhập của admin
            Session["Password"] = null; // Xoá Mật khẩu của admin

            // Sau khi đăng xuất, chuyển hướng về trang chủ
            return RedirectToAction("Index");
        }
    }
}