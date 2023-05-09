using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.Services.Orders;
using VKPCShop.WebApp.Services.Products;
using VKPCShop.WebApp.Services.Users;

namespace VKPCShop.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public HomeController(IProductService productService, IOrderService orderService, IUserService userService) 
        {
            _productService = productService;
            _orderService = orderService;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Sold = await _productService.CountSold();                   // sp đã bán
            ViewBag.Earnings = await _orderService.TotalEarnings();             // doanh thu
            ViewBag.Users = await _userService.CountUser();                     // số thành viên
            ViewBag.Products = await _productService.CountProduct();            // số sản phẩm
            return View();
        }
        public IActionResult User()
        {
            var userSession = HttpContext.Session.GetString("userSession");
            if (userSession != null)
            {
                UserSession user = JsonConvert.DeserializeObject<UserSession>(userSession);
                return Json(new
                {
                    image = user.Picture,
                    name = user.Name,
                });
            }
            return View();
        }
    }
}
