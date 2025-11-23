using Microsoft.AspNetCore.Mvc;
using NinhBinhStore.DAO;
using NinhBinhStore.Models;
namespace WebBanDoGiaDung.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductDAO _productDAO;

        public ProductController(ProductDAO productDAO)
        {
            _productDAO = productDAO;
        }

        // Trang xem tất cả sản phẩm
        // URL: /Product/Index hoặc /Product
        public IActionResult Index()
        {
            var list = _productDAO.GetAllProducts();
            return View(list); // View: AllProducts.cshtml
        }

        // Trang xem theo danh mục (gộp cả lọc giá)
        // URL: /Product/Category?loai=abc&priceRange=...
        public IActionResult Category(string loai, string priceRange)
        {
            List<Product> list;

            if (!string.IsNullOrEmpty(priceRange) && priceRange != "all")
            {
                try
                {
                    var parts = priceRange.Split('-');
                    double min = double.Parse(parts[0]);
                    double max = double.Parse(parts[1]);
                    // Lưu ý: DAO bên C# đã đổi double -> decimal, bạn ép kiểu cho đúng
                    list = _productDAO.GetProductsByCategoryAndPrice(loai, (decimal)min, (decimal)max);
                }
                catch
                {
                    list = _productDAO.GetProductsByCategory(loai);
                }
            }
            else
            {
                list = _productDAO.GetProductsByCategory(loai);
            }

            ViewBag.CategoryName = loai;
            ViewBag.CurrentPriceRange = priceRange;
            return View(list); // View: Category.cshtml
        }

        // --- CHI TIẾT SẢN PHẨM (ProductDetailServlet) ---
        // URL: /Product/Detail/5
        public IActionResult Detail(int id)
        {
            if (id <= 0) return RedirectToAction("Index", "Home");

            var product = _productDAO.GetProductById(id);
            if (product == null) return RedirectToAction("Index", "Home");

            return View(product); // View: Product/Detail.cshtml
        }

        // --- MỚI: TÌM KIẾM SẢN PHẨM (SearchServlet) ---
        // URL: /Product/Search?query=abc
        public IActionResult Search(string query)
        {
            var resultList = _productDAO.SearchProducts(query);
            ViewBag.SearchQuery = query; // Để hiển thị lại từ khóa ở ô input
            return View("SearchResults", resultList); // Cần tạo view SearchResults.cshtml
        }

        // --- MỚI: GỢI Ý TÌM KIẾM (SearchSuggestServlet) ---
        // URL: /Product/SearchSuggest?query=abc
        // Trả về JSON cho AJAX
        public IActionResult SearchSuggest(string query)
        {
            if (string.IsNullOrEmpty(query) || query.Length < 2)
            {
                return Json(new List<object>());
            }

            var suggestions = _productDAO.SearchProducts(query);

            // Trả về JSON
            return Json(suggestions);
        }
    }
}