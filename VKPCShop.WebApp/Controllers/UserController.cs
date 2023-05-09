using Facebook;
using GoogleAuthentication.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VKPCShop.Data.Entities;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.Services.Orders;
using VKPCShop.WebApp.Services.Products;
using VKPCShop.WebApp.Services.Users;
using VKPCShop.WebApp.ViewModel;

namespace VKPCShop.WebApp.Controllers
{
    public class UserController : Controller
	{
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        public UserController(IConfiguration config, IUserService userService, IOrderService orderService, IProductService productService)
        {
            _config = config;
            _userService = userService;
            _orderService = orderService;
            _productService = productService;
        }
        public async Task<IActionResult> Index()
		{
            ViewBag.Updated = TempData["updated"];
            var cartSession = HttpContext.Session.GetString("cartSession");
            if (cartSession != null)
            {
                ViewBag.CartNumber = JsonConvert.DeserializeObject<List<CartVm>>(cartSession).Count();
            }
            else { ViewBag.CartNumber = 0; }

            var userSession = HttpContext.Session.GetString("userSession");
            if (userSession != null)
            {
                UserSession user = JsonConvert.DeserializeObject<UserSession>(userSession);
                ViewBag.User = await _userService.FindByEmail(user.Email);
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
		}
		public async Task<IActionResult> Login()
		{
            // google
            var clientId = _config.GetValue<string>("ClientId");
            var url = _config.GetValue<string>("UrlGG");
            var getUrlGoogle = GoogleAuth.GetAuthUrl(clientId, url);
            ViewBag.getUrlGoogle = getUrlGoogle;

            // facebook
            var fb = new FacebookClient();
            var getUrlFacebook = fb.GetLoginUrl(new
            {
                client_id = "782928379828275",
                redirect_uri = "https://localhost:44341/User/LoginWithFacebook",
                scope = "public_profile, email"
            });
            ViewBag.getUrlFacebook = getUrlFacebook;
            return View();
		}
        public async Task<IActionResult> LoginWithFacebook(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Get("/oauth/access_token", new
            {
                client_id = "782928379828275",
                client_secret = "cdba5287a054c96c85e8a4af44f1138c",
                redirect_uri = "https://localhost:44341/User/LoginWithFacebook",
                code = code
            });
            fb.AccessToken = result.access_token;
            dynamic me = fb.Get("/me?fields=name,email,picture");

            UserDto dto = new UserDto()
            {
                FullName = me.name,
                Email = me.email,
                Image = me.picture.data.url
            };
            var userSession = new UserSession()
            {
                Email = me.email,
                Name = me.name,
                Picture = me.picture.data.url,
            };
            HttpContext.Session.SetString("userSession", JsonConvert.SerializeObject(userSession)); // convert model sang json, gán vào session
            var userDto = await _userService.Create(dto);
            if (userDto.RoleId == 1)
            {
                userSession.Role = "ADMIN";
                var jsonString = JsonConvert.SerializeObject(userSession);
                HttpContext.Session.SetString("userSession", jsonString);
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            if (userDto.PhoneNumber == null && userDto.Address == null)
            {
                return RedirectToAction("Update", new { id = userDto.Id });
            }
            else
            {
                return RedirectToAction("Index", "User");

            }
        }
        public async Task<IActionResult> LoginWithGoogle(string code)
        {
            var clientId = _config.GetValue<string>("ClientId");
            var url = _config.GetValue<string>("UrlGG");
            var clientSecret = _config.GetValue<string>("ClientSecret");
            var token = await GoogleAuth.GetAuthAccessToken(code, clientId, clientSecret, url);        // trả về token
            var userProfile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken.ToString());  // lấy thông tin người dùng

            
            InfoGoogle userGoogle = JsonConvert.DeserializeObject<InfoGoogle>(userProfile);        // convert json sang model
            var userSession = new UserSession()
            {
                Email = userGoogle.Email,
                Name = userGoogle.Name,
                Picture = userGoogle.Picture,
            };

            // kiểm tra xem user đã từng đăng nhập chưa, nếu chưa thì lưu user vào DB
            UserDto dto = new UserDto()
            {
                FullName = userSession.Name,
                Email = userSession.Email,
                Image = userSession.Picture
            };
            var userDto = await _userService.Create(dto);
            if (userDto.RoleId == 1)
            {
                userSession.Role = "ADMIN";
                var jsonString = JsonConvert.SerializeObject(userSession);
                HttpContext.Session.SetString("userSession", jsonString);
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }


            var session = JsonConvert.SerializeObject(userSession);
            HttpContext.Session.SetString("userSession", session);
            if (userDto.PhoneNumber == null && userDto.Address == null)
            {
                return RedirectToAction("Update", new { id = userDto.Id });
            }
            else
            {
                return RedirectToAction("Index","User");

            }
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("userSession");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id, string isBuy, string isBuild)
        {
            if (isBuy != null)
                ViewBag.isBuy = isBuy;
            else if (isBuild != null)
                ViewBag.isBuild = isBuild;
            else
                ViewBag.isBuy = TempData["isBuy"];
            ViewBag.Updated = TempData["updated"];
            var user = await _userService.GetById(id);
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UserDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.isBuy = dto.IsBuy;
                ViewBag.isBuild = dto.IsBuild;
                return View(dto);
            }
            var update = await _userService.Update(dto);
            if (update)
            {
                TempData["updated"] = "updateSuccess";
                if (dto.IsBuy == "notBuy") // không phải mua hàng thì trả về trang user
                {
                    TempData["isBuy"] = dto.IsBuy;
                    return RedirectToAction("Index");
                }
                if (dto.IsBuild == "build")
                    return RedirectToAction("CheckOut", "Cart", new { isBuild = dto.IsBuild });
                if (dto.IsBuy == "buy")
                    return RedirectToAction("CheckOut", "Cart", new { isBuy = dto.IsBuy });
            }    
            else
                TempData["updated"] = "updateFaild";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Orders(int id)
        {
            var cartSession = HttpContext.Session.GetString("cartSession");
            if (cartSession != null)
            {
                ViewBag.CartNumber = JsonConvert.DeserializeObject<List<CartVm>>(cartSession).Count();
            }
            else { ViewBag.CartNumber = 0; }



            var orders = await _orderService.GetOrders(id);
            var productsInOrder = new List<ProductVm>();
            foreach(var order in orders)
            {
                var products = await _productService.GetProductsVmById(order.Id);
                foreach(var product in products)
                {
                    productsInOrder.Add(product);
                }
            }
            ViewBag.Result = TempData["order"];
            ViewBag.ProductsInOrder = productsInOrder;
            return View(orders);
        }
        public async Task<IActionResult> OrderDetail(int id)
        {
            var userSession = HttpContext.Session.GetString("userSession");
            if (userSession == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                var user = JsonConvert.DeserializeObject<UserSession>(userSession);
                var findByEmail = await _userService.FindByEmail(user.Email);
                ViewBag.User = findByEmail;
            }
            var cartSession = HttpContext.Session.GetString("cartSession");
            var cart = new List<CartVm>();
            if (cartSession != null)
            {
                ViewBag.CartNumber = JsonConvert.DeserializeObject<List<CartVm>>(cartSession).Count();
            }
            else { ViewBag.CartNumber = 0; }
            var order = await _orderService.GetById(id);
            var productsInOrder = new List<ProductVm>();
            var products = await _productService.GetProductsVmById(order.Id);
            foreach (var product in products)
            {
                productsInOrder.Add(product);
            }
            ViewBag.ProductsInOrder = productsInOrder;
            return View(order);
        }
    }
}
