using Microsoft.AspNetCore.Mvc;
using NinhBinhStore.DAO;

namespace NinhBinhStore.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ProductDAO _productDAO;

        public CategoryMenuViewComponent(ProductDAO productDAO)
        {
            _productDAO = productDAO;
           
        }

        public IViewComponentResult Invoke()
        {
            // Lấy danh sách category từ DB
            var categories = _productDAO.GetAllCategories();
            return View(categories);
        }
    }
}
