🏠 Ninh Binh Store - Web Bán Đồ Gia Dụng (ASP.NET Core MVC)
Một hệ thống thương mại điện tử hiện đại chuyên cung cấp các thiết bị gia dụng, được xây dựng trên nền tảng ASP.NET Core MVC. Dự án tập trung vào trải nghiệm người dùng mượt mà, giao diện Responsive và hệ thống quản trị (Admin) mạnh mẽ.

🚀 Tính Năng Nổi Bật
👤 Dành cho Khách hàng (User)
Xác thực: Đăng ký, Đăng nhập, Quên mật khẩu, Cập nhật thông tin cá nhân.

Tìm kiếm & Lọc:

Tìm kiếm sản phẩm với Gợi ý thông minh (Ajax Search Suggest).

Lọc sản phẩm theo Danh mục và Khoảng giá.

Giỏ hàng thông minh:

Thêm vào giỏ hàng bằng AJAX (không load lại trang) với thông báo Popup đẹp mắt.

Cập nhật số lượng, xóa sản phẩm, tính tổng tiền tự động.

Thanh toán & Đặt hàng:

Quy trình Checkout 3 bước: Giỏ hàng -> Thông tin giao hàng -> Xác nhận.

Tích hợp mã QR thanh toán ngân hàng (VietQR) tự động điền số tiền và nội dung.

Quản lý đơn hàng:

Xem Lịch sử mua hàng (chỉ hiện đơn đã hoàn thành).

Tra cứu đơn hàng dành cho khách vãng lai (qua Email).

Gửi phản hồi/liên hệ tới Admin.

🛡️ Dành cho Quản trị viên (Admin)
Dashboard: Xem tổng quan hệ thống với các lối tắt nhanh.

Quản lý Sản phẩm: Thêm, Sửa, Xóa sản phẩm, Upload hình ảnh.

Quản lý Đơn hàng:

Xem danh sách đơn hàng, lọc theo trạng thái.

Xem chi tiết đơn hàng qua Modal (Popup).

Cập nhật trạng thái đơn (Chờ xử lý -> Đang giao -> Hoàn thành/Hủy).

In hóa đơn bán hàng: Xuất hóa đơn chuẩn khổ A4 và tải về dưới dạng ảnh (JPG).

Báo cáo thống kê:

Biểu đồ doanh thu (Chart.js) trực quan theo ngày.

Xuất báo cáo ra file Excel (.xlsx) chuẩn định dạng.

Quản lý khác: Quản lý người dùng, Quản lý tin nhắn liên hệ.

🛠️ Công Nghệ Sử Dụng
Backend
Framework: ASP.NET Core MVC (.NET 6/7/8).

Ngôn ngữ: C#.

Kiến trúc: MVC (Model-View-Controller) kết hợp mô hình DAO (Data Access Object).

Database Access: Microsoft.Data.SqlClient (Sử dụng Raw SQL thuần để tối ưu hiệu năng và kiểm soát truy vấn).

Excel Handling: Thư viện EPPlus.

Frontend
View Engine: Razor Views (.cshtml).

Framework CSS: Bootstrap 5 (Responsive & Modern UI).

Icons: FontAwesome 6.

JavaScript Libraries:

SweetAlert2: Popup thông báo đẹp mắt.

Chart.js: Vẽ biểu đồ báo cáo.

Chartjs-plugin-datalabels: Hiển thị số liệu trên biểu đồ.

Html2Canvas: Chụp ảnh hóa đơn.

Database
Hệ quản trị: SQL Server.

🗄️ Cơ Sở Dữ Liệu (Database Schema)
Dự án bao gồm 5 bảng chính:

taikhoan: Lưu thông tin người dùng (Admin/User).

id, hoten, tendangnhap, matkhau, email, sodienthoai, diachi, vaitro.

sanpham: Lưu thông tin hàng hóa.

id, tensp, mota, dongia, hinhanh, loai (Category), tonkho.

donhang: Lưu thông tin đơn đặt hàng.

id, id_taikhoan, ngaydat, tongtien, trangthai, diachi, email (cho khách vãng lai).

chitietdonhang: Lưu chi tiết sản phẩm trong từng đơn.

id, id_donhang, id_sanpham, soluong, dongia.

lienhe: Lưu tin nhắn phản hồi từ khách hàng.

id, hoten, email, noidung, ngaygui.

📂 Cấu Trúc Dự Án
NinhBinhStore/
├── Controllers/          # Xử lý logic điều hướng (Account, Cart, Admin, Product...)
├── DAO/                  # Lớp truy xuất dữ liệu (Database Access Objects)
│   ├── DBContext.cs      # Cấu hình kết nối SQL
│   ├── ProductDAO.cs     # CRUD Sản phẩm, Tìm kiếm, Lọc
│   ├── OrderDAO.cs       # Xử lý Đơn hàng, Báo cáo doanh thu
│   ├── UserDAO.cs        # Xử lý User, Đăng nhập/Đăng ký
│   └── ContactDAO.cs     # Xử lý Liên hệ
├── Models/               # Các thực thể (Product, User, Order, CartItem...)
├── Views/                # Giao diện người dùng (Razor Pages)
│   ├── Admin/            # Giao diện trang quản trị
│   ├── Account/          # Đăng nhập, Đăng ký
│   ├── Cart/             # Giỏ hàng
│   ├── Checkout/         # Thanh toán
│   ├── Home/             # Trang chủ, Liên hệ
│   ├── Order/            # Lịch sử, Tra cứu
│   ├── Product/          # Chi tiết, Danh sách sản phẩm
│   └── Shared/           # Layout chung, Components
│       └── Components/   # ViewComponent (CategoryMenu)
├── wwwroot/              # File tĩnh
│   ├── css/              # Custom Styles
│   ├── js/               # Custom Scripts
│   └── images/           # Hình ảnh sản phẩm
└── appsettings.json      # Chuỗi kết nối Database

⚙️ Cài Đặt & Chạy Dự Án
1. Yêu cầu hệ thống
Visual Studio 2022.

SQL Server.

.NET SDK (6.0 hoặc cao hơn).

2. Thiết lập Cơ sở dữ liệu
Mở SQL Server Management Studio (SSMS).

Tạo một Database mới tên là web_bandogiadung.

Chạy file script SQL (Cơ sở dữ liệu.txt) để tạo bảng và thêm dữ liệu mẫu.

3. Cấu hình kết nối
Mở file appsettings.json trong Visual Studio.
Sửa chuỗi kết nối DefaultConnection cho phù hợp với máy của bạn:
JSON
"ConnectionStrings": {
  "DefaultConnection": "Server=TEN_MAY_CUA_BAN;Database=web_bandogiadung;Trusted_Connection=True;TrustServerCertificate=True"
}

4. Chạy dự án
Nhấn F5 hoặc nút Run (https) trong Visual Studio.
Trình duyệt sẽ mở trang chủ tại https://localhost:xxxx.

📝 Tác Giả
Phát triển bởi: Nguyễn Ngọc Tú
Liên hệ: nguyentunb1@gmail.com
