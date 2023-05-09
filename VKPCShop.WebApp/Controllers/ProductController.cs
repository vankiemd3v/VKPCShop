using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.WebSockets;
using VKPCShop.Data.Entities;
using VKPCShop.WebApp.Constant;
using VKPCShop.WebApp.Services.Brands;
using VKPCShop.WebApp.Services.Images;
using VKPCShop.WebApp.Services.Products;
using VKPCShop.WebApp.ViewModel;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace VKPCShop.WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly IImageService _imageService;
        public ProductController(IProductService productService, IBrandService brandService, IImageService imageService)
        {
            _productService = productService;
            _brandService = brandService;
            _imageService = imageService;
        }
        public async Task<IActionResult> Index(int? categoryId, bool ascending, bool descending, bool bestSell, int? price, List<string>? brands, string? keyword, bool sale, bool casepc)
        {
            var cartSession = HttpContext.Session.GetString("cartSession");
            if (cartSession != null)
            {
                ViewBag.CartNumber = JsonConvert.DeserializeObject<List<CartVm>>(cartSession).Count();
            }
            else { ViewBag.CartNumber = 0; }
            if (keyword != null)
            {
                ViewBag.Keyword = keyword;
            }
            ViewBag.CategoryId = categoryId;
            if (ascending)
            {
                ViewBag.FilterName = FilterProductConstant.Ascending;
            }
            else if (descending)
            {
                ViewBag.FilterName = FilterProductConstant.Descending;
            }
            else if (bestSell)
            {
                ViewBag.FilterName = FilterProductConstant.BestSell;
            }
            else
            {
                ViewBag.FilterName = FilterProductConstant.Default;
            }
            if(brands.Count > 0)
            {
                ViewBag.BrandChecked = await _brandService.GetBrands(brands);
                ViewBag.ListBrand = brands;
            }
            else
            {
                ViewBag.BrandChecked = null;
            }
            if(price != null)
            {
                ViewBag.PriceCheck = price;
            }
            else
            {
                ViewBag.PriceCheck = 30;
            }
            ViewBag.Brand = await _brandService.GetAll(null);
            var products = await _productService.GetAll(bestSell,sale, casepc, categoryId, ascending, descending, price, brands, null, null, keyword);
            return View(products);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var cartSession = HttpContext.Session.GetString("cartSession");
            if (cartSession != null)
            {
                ViewBag.CartNumber = JsonConvert.DeserializeObject<List<CartVm>>(cartSession).Count();
            }
            else { ViewBag.CartNumber = 0; }
            var detail = await _productService.GetById(id);
            if (detail != null)
            {
                ViewBag.Images = await _imageService.GetImagesByProductId(detail.Id);
                var brand = await _brandService.GetById(detail.BrandId);
                ViewBag.Brand = brand.Name;
                ViewBag.ProductsByBrand = await _productService.GetAll(false, false, false, null, false, false, null, null, detail.BrandId, detail.Id, null);
            }
            return View(detail);
        }
    }
}
