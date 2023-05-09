using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VKPCShop.WebApp.Services.Categories;
using VKPCShop.WebApp.ViewModel;

namespace VKPCShop.WebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            var cartSession = HttpContext.Session.GetString("cartSession");
            if (cartSession != null)
            {
                ViewBag.CartNumber = JsonConvert.DeserializeObject<List<CartVm>>(cartSession).Count();
            }
            else { ViewBag.CartNumber = 0; }
            var categories = await _categoryService.GetAll(null);
            return View(categories);
        }
    }
}
