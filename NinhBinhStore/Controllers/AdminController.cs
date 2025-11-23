using Microsoft.AspNetCore.Mvc;
using NinhBinhStore.DAO;
using NinhBinhStore.Models;
using OfficeOpenXml; // Thêm namespace này
using OfficeOpenXml.Style; // Thêm namespace này
namespace NinhBinhStore.Controllers
{
    // Yêu cầu đăng nhập và phải là admin (bạn cần cấu hình Authentication/Authorization sau)
    // Tạm thời ta kiểm tra session thủ công trong code giống như Servlet cũ
    public class AdminController : Controller
    {
        private readonly ProductDAO _productDAO;
        private readonly OrderDAO _orderDAO;
        private readonly UserDAO _userDAO;
        private readonly ContactDAO _contactDAO;
        private readonly IWebHostEnvironment _webHostEnvironment; // Để xử lý upload file

        public AdminController(ProductDAO productDAO, OrderDAO orderDAO, UserDAO userDAO, ContactDAO contactDAO, IWebHostEnvironment webHostEnvironment)
        {
            _productDAO = productDAO;
            _orderDAO = orderDAO;
            _userDAO = userDAO;
            _contactDAO = contactDAO;
            _webHostEnvironment = webHostEnvironment;
        }

        // Helper để kiểm tra quyền Admin (thay cho Filter)
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("vaitro");
            return role == "admin";
        }

        // 1. DASHBOARD
        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            return View();
        }

        // 2. QUẢN LÝ SẢN PHẨM (Manage Products)
        public IActionResult ManageProducts()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            var list = _productDAO.GetAllProducts();
            return View(list);
        }

        public IActionResult CreateProduct()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            return View("ProductForm", new Product()); // Dùng chung form cho Add/Edit
        }

        public IActionResult EditProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            var product = _productDAO.GetProductById(id);
            if (product == null) return NotFound();
            return View("ProductForm", product);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProduct(Product p, IFormFile? hinhanhFile, string hinhanhCu)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            // Xử lý upload ảnh
            if (hinhanhFile != null && hinhanhFile.Length > 0)
            {
                // Đường dẫn: wwwroot/images/products
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = DateTime.Now.Ticks + "_" + hinhanhFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await hinhanhFile.CopyToAsync(fileStream);
                }
                p.Hinhanh = "images/products/" + uniqueFileName;
            }
            else
            {
                p.Hinhanh = hinhanhCu ?? "";
            }

            if (p.Id == 0) // Thêm mới
            {
                _productDAO.InsertProduct(p);
                TempData["SuccessMessage"] = "Đã thêm sản phẩm thành công!";
            }
            else // Cập nhật
            {
                _productDAO.UpdateProduct(p);
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
            }

            return RedirectToAction("ManageProducts");
        }

        public IActionResult DeleteProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            _productDAO.DeleteProduct(id);
            return RedirectToAction("ManageProducts");
        }

        // 3. QUẢN LÝ ĐƠN HÀNG (Manage Orders)
        public IActionResult ManageOrders()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            var list = _orderDAO.GetAllOrders();
            return View(list);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string newStatus)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            _orderDAO.UpdateOrderStatus(orderId, newStatus);
            TempData["SuccessMessage"] = $"Cập nhật đơn hàng #{orderId} thành công!";
            return RedirectToAction("ManageOrders");
        }

        // 4. QUẢN LÝ LIÊN HỆ (Manage Contacts)
        public IActionResult ManageContacts()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            var list = _contactDAO.GetAllContacts();
            return View(list);
        }

        public IActionResult DeleteContact(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            _contactDAO.DeleteContact(id);
            TempData["SuccessMessage"] = "Đã xóa tin nhắn thành công!";
            return RedirectToAction("ManageContacts");
        }

        // 5. QUẢN LÝ USER (Manage Users)
        public IActionResult ManageUsers()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            var list = _userDAO.GetAllUsers();
            return View(list);
        }

        // Trả về JSON để hiển thị lên Modal
        public IActionResult GetOrderDetails(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            // Lấy chi tiết sản phẩm
            var items = _orderDAO.GetOrderDetails(id);

            // Lấy thông tin user để lấy SĐT (nếu trong Order chưa có)
            // Ở đây giả sử OrderDAO.GetAllOrders đã join lấy SĐT rồi, 
            // nhưng ta cần lấy lại info cụ thể của đơn này để hiển thị Modal
            // Bạn có thể viết thêm hàm GetOrderById trong OrderDAO nếu cần

            return Ok(items); // Trả về danh sách sản phẩm JSON
        }

        // 1. Action Xem Báo Cáo
        public IActionResult RevenueReport()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            var stats = _orderDAO.GetRevenueStats();

            // Tính tổng doanh thu toàn bộ để hiển thị con số tổng
            ViewBag.TotalRevenue = stats.Sum(x => x.DoanhThu);
            ViewBag.TotalOrders = stats.Sum(x => x.SoDonHang);

            return View(stats);
        }

        // 2. Action Xuất Báo Cáo ra Excel (CSV)
        public IActionResult ExportRevenue()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            var stats = _orderDAO.GetRevenueStats();

            // Cấu hình EPPlus (Bắt buộc cho bản Free)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Báo Cáo Doanh Thu");

                // 1. Tạo Tiêu đề cột
                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Số Đơn Hàng";
                worksheet.Cells[1, 3].Value = "Doanh Thu (VNĐ)";

                // Format Tiêu đề (In đậm, Màu nền xanh, Căn giữa)
                using (var range = worksheet.Cells[1, 1, 1, 3])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                // 2. Đổ dữ liệu
                int row = 2;
                foreach (var item in stats)
                {
                    worksheet.Cells[row, 1].Value = item.Ngay;
                    worksheet.Cells[row, 2].Value = item.SoDonHang;
                    worksheet.Cells[row, 3].Value = item.DoanhThu;

                    // Format tiền tệ cho cột Doanh Thu (Ví dụ: 10,000,000)
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0";

                    row++;
                }

                // 3. Tự động chỉnh độ rộng cột
                worksheet.Cells.AutoFitColumns();

                // Xuất file
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"BaoCaoDoanhThu_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        public IActionResult PrintInvoice(int id)
        {
            // Gọi hàm vừa thêm trong DAO
            var order = _orderDAO.GetOrderById(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
