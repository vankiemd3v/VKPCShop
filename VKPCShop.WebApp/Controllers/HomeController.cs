using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using VKPCShop.WebApp.Models;
using VKPCShop.WebApp.Services.Brands;
using VKPCShop.WebApp.Services.Categories;
using VKPCShop.WebApp.Services.Products;
using VKPCShop.WebApp.ViewModel;

namespace VKPCShop.WebApp.Controllers
{
	public class HomeController : Controller
	{
        private readonly IProductService _productService;
		private readonly ICategoryService _categoryService;
		private readonly IBrandService _brandService;
        public HomeController(IProductService productService, ICategoryService categoryService, IBrandService brandService)
		{
			_productService = productService;
			_categoryService = categoryService;
			_brandService = brandService;
		}

		public async Task<IActionResult> Index()
		{
			var cartSession = HttpContext.Session.GetString("cartSession");
			if(cartSession != null)
			{
				ViewBag.CartNumber = JsonConvert.DeserializeObject<List<CartVm>>(cartSession).Count();
			}
			else { ViewBag.CartNumber = 0; }
			ViewBag.ParentCategory = await _categoryService.GetParents();			// Danh mục cha
			ViewBag.BrandHot = await _brandService.GetAll(null);					// Thương hiệu nổi tiếng
			ViewBag.CategoryName = await _categoryService.GetAll(null);				// Danh mục sản phẩm
			foreach(var item in ViewBag.CategoryName)
			{
				if(item.Name == "Màn hình máy tính")
				{
					ViewBag.DisplayPC = item;
				}
			}
            ViewBag.PC = await _productService.GetAll(false, false, true, null, false, false, null, null, null, null, null);            // PC Gaming, PC Văn phòng
			foreach (var item in ViewBag.PC) 
			{
                ViewBag.PcId = item.CategoryId;
				break;
            }
			
            ViewBag.Sale = await _productService.GetAll(false, true, false, null, false, false, null, null, null, null, null);		// Sản phẩm khuyến mãi
            ViewBag.BestSell = await _productService.GetAll(true, false, false, null, false, false, null, null, null, null, null);	// Sản phẩm bán chạy nhất
			var products = await _productService.GetAll(false, false, false	, null, false, false, null, null, null, null, null);      // Gợi ý dành cho bạn
			return View(products);
		}

	}
}