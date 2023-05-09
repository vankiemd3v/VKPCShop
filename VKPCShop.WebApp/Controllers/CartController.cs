using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using System.Net.WebSockets;
using VKPCShop.Data.Entities;
using VKPCShop.WebApp.Constant;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.Services.Brands;
using VKPCShop.WebApp.Services.Categories;
using VKPCShop.WebApp.Services.Orders;
using VKPCShop.WebApp.Services.Products;
using VKPCShop.WebApp.Services.Users;
using VKPCShop.WebApp.ViewModel;

namespace VKPCShop.WebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        public CartController(IProductService productService, IBrandService brandService, ICategoryService categoryService, IUserService userService, IOrderService orderService, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _productService = productService;
            _brandService = brandService;
            _categoryService = categoryService;
            _userService = userService;
            _orderService = orderService;
            _env = env;

        }
        public IActionResult Index()
        {
            ViewBag.DeleteAll = TempData["deleteAll"];

            var cartSession = HttpContext.Session.GetString("cartSession");
            var cart = new List<CartVm>();
            if (cartSession != null)
            {
                cart = JsonConvert.DeserializeObject<List<CartVm>>(cartSession);
                ViewBag.CartNumber = JsonConvert.DeserializeObject<List<CartVm>>(cartSession).Count();
            }
            else { ViewBag.CartNumber = 0; }
            int totalPayment = 0;
            if(cart.Count > 0)
            {
                foreach (var item in cart)
                {
                    if (item.SalePrice > 0)
                        totalPayment = totalPayment + item.SalePrice * item.BuyQuantity;
                    else
                        totalPayment = totalPayment + item.Price * item.BuyQuantity;
                }
            }
            else
            {
                ViewBag.NoCart = "Không có sản phẩm trong giỏ hàng.";
            }
            ViewBag.TotalPayment = totalPayment;
            return View(cart);
        }
        public async Task<IActionResult> AddToCart(int id)
        {
            var cartSession = HttpContext.Session.GetString("cartSession");
            var cart = new List<CartVm>();
            if(cartSession != null)
                cart = JsonConvert.DeserializeObject<List<CartVm>>(cartSession);              // nếu đã có sp trong giỏ thì convert json cart sang model cartVm
            if(cart.Any(x=>x.Id == id))
            {
                var item = cart.Where(x => x.Id == id).SingleOrDefault();
                if(item.BuyQuantity < item.Quantity)
                    item.BuyQuantity++;
            }
            else
            {
                var product = await _productService.GetById(id);
                var brand = await _brandService.GetById(product.BrandId);
                if(product != null)
                {
                    var item = new CartVm()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        SalePrice = product.SalePrice,
                        Quantity = product.Quantity,
                        BuyQuantity = 1,
                        Image = product.Image,
                        BrandId = product.BrandId,
                        BrandName = brand.Name,
                    };
                    cart.Add(item);
                }
            }
            HttpContext.Session.SetString("cartSession", JsonConvert.SerializeObject(cart));  // convert sang json rồi gán cart vào session
            return Json(new { cartNumber = cart.Count, status = true});
        }
        public async Task<IActionResult> Update(int id, string active)
        {
            var cartSession = HttpContext.Session.GetString("cartSession");
            var cart = new List<CartVm>();
            if (cartSession != null)
                cart = JsonConvert.DeserializeObject<List<CartVm>>(cartSession);
            var item = cart.Where(x => x.Id == id).SingleOrDefault();
            if(active == "plus" && item.BuyQuantity < item.Quantity)
                item.BuyQuantity++;
            if(active == "minus")
            {
                item.BuyQuantity--;
                if (item.BuyQuantity < 1)
                    cart.Remove(item);
            }
            HttpContext.Session.SetString("cartSession", JsonConvert.SerializeObject(cart));
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteAllCart()
        {
            HttpContext.Session.Remove("cartSession");
            TempData["deleteAll"] = "deleteAll";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Build()
        {
            ViewBag.ReBuild = TempData["rebuild"];
            var cartSession = HttpContext.Session.GetString("cartSession");
            if (cartSession != null)
            {
                ViewBag.CartNumber = JsonConvert.DeserializeObject<List<CartVm>>(cartSession).Count();
            }
            else { ViewBag.CartNumber = 0; }



            var buildSession = HttpContext.Session.GetString("buildSession");
            var buildObj = new List<CartVm>();
            if (buildSession != null)
                buildObj = JsonConvert.DeserializeObject<List<CartVm>>(buildSession);
            ViewBag.ProductsSelected = buildObj;
            int totalPayment = 0;
            if (buildObj.Count > 0)
            {
                foreach (var item in buildObj)
                {
                    if (item.SalePrice > 0)
                        totalPayment = totalPayment + item.SalePrice * item.BuyQuantity;
                    else
                        totalPayment = totalPayment + item.Price * item.BuyQuantity;
                }
            }
            ViewBag.TotalPayment = totalPayment;

            ViewBag.SelectList = await _productService.GetAll(false, false, false, null, false, false, null, null, null, null, null);
            var categoriesChildren = await _categoryService.GetChildrens();
            return View(categoriesChildren);
        }

        public async Task<IActionResult> AddToBuild(int id)
        {
            var buildSession = HttpContext.Session.GetString("buildSession");
            var buildObj = new List<CartVm>();
            if (buildSession != null)
                buildObj = JsonConvert.DeserializeObject<List<CartVm>>(buildSession);
            var product = await _productService.GetById(id);
            if (product != null)
            {
                var category = await _categoryService.GetById(product.CategoryId);
                var brand = await _brandService.GetById(product.BrandId);
                var item = new CartVm()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    SalePrice = product.SalePrice,
                    Quantity = product.Quantity,
                    BuyQuantity = 1,
                    Image = product.Image,
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    BrandId = product.BrandId,
                    BrandName =brand.Name
                };
                buildObj.Add(item);
            }
            HttpContext.Session.SetString("buildSession", JsonConvert.SerializeObject(buildObj));
            return RedirectToAction("Build");
        }
        public async Task<IActionResult> UpdateBuild(int id, string active)
        {
            var buildSession = HttpContext.Session.GetString("buildSession");
            var buildObj = new List<CartVm>();
            if (buildSession != null)
                buildObj = JsonConvert.DeserializeObject<List<CartVm>>(buildSession);
            var item = buildObj.Where(x => x.Id == id).SingleOrDefault();
            if (active == "plus" && item.BuyQuantity < item.Quantity)
                item.BuyQuantity++;
            if (active == "minus")
            {
                if(item.BuyQuantity > 1)
                    item.BuyQuantity--;
            }
            HttpContext.Session.SetString("buildSession", JsonConvert.SerializeObject(buildObj));
            return RedirectToAction("Build");
        }
        public async Task<IActionResult> DeleteBuild(int id) //id category
        {
            var buildSession = HttpContext.Session.GetString("buildSession");
            var buildObj = new List<CartVm>();
            if (buildSession != null)
                buildObj = JsonConvert.DeserializeObject<List<CartVm>>(buildSession);
            var delete = buildObj.Where(x => x.CategoryId == id).SingleOrDefault();
            buildObj.Remove(delete);
            HttpContext.Session.SetString("buildSession", JsonConvert.SerializeObject(buildObj));
            return RedirectToAction("Build");
        }
        public async Task<IActionResult> DeleteAllBuild()
        {
            HttpContext.Session.Remove("buildSession");
            TempData["rebuild"] = "rebuild";
            return RedirectToAction("Build");
        }
        public async Task<IActionResult> CheckOut(string? isBuy, string? isBuild)
        {
            ViewBag.Updated = TempData["updated"];
            var userSession = HttpContext.Session.GetString("userSession");
            if(userSession == null)
            {
                return RedirectToAction("Login","User");
            }
            else
            {
                var user = JsonConvert.DeserializeObject<UserSession>(userSession);
                var findByEmail = await _userService.FindByEmail(user.Email);
                ViewBag.User = findByEmail;
            }
            var cart = new List<CartVm>();
            if(isBuild != null)
            {
                ViewBag.isBuild = isBuild;
                var buildSession = HttpContext.Session.GetString("buildSession");
                if (buildSession != null)
                {
                    cart = JsonConvert.DeserializeObject<List<CartVm>>(buildSession);
                }
                var cartSession = HttpContext.Session.GetString("cartSession");
                if (cartSession != null)
                {
                    ViewBag.CartNumber = JsonConvert.DeserializeObject<List<CartVm>>(cartSession).Count();
                }
                else { ViewBag.CartNumber = 0; }

            }
            else
            {
                if(isBuild != null)
                     ViewBag.isBuy = isBuy;
                else
                    ViewBag.isBuy = "buy";
                var cartSession = HttpContext.Session.GetString("cartSession");
                if (cartSession != null)
                {
                    cart = JsonConvert.DeserializeObject<List<CartVm>>(cartSession);
                    ViewBag.CartNumber = JsonConvert.DeserializeObject<List<CartVm>>(cartSession).Count();
                }

                else { ViewBag.CartNumber = 0; }
            }
            int totalPayment = 0;
            if (cart.Count > 0)
            {
                foreach (var item in cart)
                {
                    if (item.SalePrice > 0)
                        totalPayment = totalPayment + item.SalePrice * item.BuyQuantity;
                    else
                        totalPayment = totalPayment + item.Price * item.BuyQuantity;
                }
            }
            else
            {
                ViewBag.NoCart = "Không có sản phẩm trong giỏ hàng.";
            }
            ViewBag.TotalPayment = totalPayment;
            return View(cart);
        }
        [HttpPost]
        public async Task<IActionResult> CheckOut(int userId, string? isBuild)
        {
            var cart = new List<CartVm>();
            if (isBuild == null) // order = cart
            {
                var cartSession = HttpContext.Session.GetString("cartSession");
                if (cartSession != null)
                {
                    cart = JsonConvert.DeserializeObject<List<CartVm>>(cartSession);
                }
            }
            else  // order =  build
            {
                var buildSession = HttpContext.Session.GetString("buildSession");
                if (buildSession != null)
                {
                    cart = JsonConvert.DeserializeObject<List<CartVm>>(buildSession);
                }
            }
            var orderDetailDto = new List<OrderDetailDto>();
            int totalPayment = 0;
            foreach(var item in cart)
            {
                if(item.SalePrice > 0)
                {
                    totalPayment += item.SalePrice * item.BuyQuantity;
                    orderDetailDto.Add(new OrderDetailDto()
                    {
                        ProductId = item.Id,
                        BuyQuantity = item.BuyQuantity,
                        Price = item.SalePrice,
                    });
                }
                else
                {
                    totalPayment += item.Price * item.BuyQuantity;
                    orderDetailDto.Add(new OrderDetailDto()
                    {
                        ProductId = item.Id,
                        BuyQuantity = item.BuyQuantity,
                        Price = item.Price,
                    });
                }
            }
            var user = await _userService.GetById(userId);
            var orderDto = new OrderDto();
            orderDto.UserId = userId;
            orderDto.ShipName = user.FullName;
            orderDto.ShipPhoneNumber = user.PhoneNumber;
            orderDto.ShipEmail = user.Email;
            orderDto.ShipAddress = user.Address;
            orderDto.TotalPayment = totalPayment;
            orderDto.OrderDetails = orderDetailDto;
            orderDto.Status = "Chờ duyệt";
            orderDto.CreatedDate = DateTime.Now;
            var order = await _orderService.Create(orderDto);
            if (order)
            {
                await SendEmail(orderDto);
                if(isBuild == null)
                    HttpContext.Session.Remove("cartSession");
                else

                    HttpContext.Session.Remove("buildSession");
                TempData["order"] = "success";
            }
            else
            {
                TempData["order"] = "error";
            }
            HttpContext.Session.Remove("cartSession");
            return RedirectToAction("Orders","User", new {id = userId});
        }

        public async Task<bool> ConfigEmail(string mess, string emailCustomer, string subject)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(EmailConstant.Email));
            email.To.Add(MailboxAddress.Parse(emailCustomer));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = mess
            };
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(EmailConstant.Email, EmailConstant.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
            return true;
        }
        public async Task<bool> SendEmail(OrderDto dto)
        {
            // Gửi mail cho khách hàng
            var mess = await System.IO.File.ReadAllTextAsync(_env.WebRootPath + "/html/order.html");
            mess = mess.Replace("{{Name}}", dto.ShipName);
            mess = mess.Replace("{{PhoneNumber}}", dto.ShipPhoneNumber);
            mess = mess.Replace("{{Email}}", dto.ShipEmail);
            mess = mess.Replace("{{Address}}", dto.ShipAddress);
            mess = mess.Replace("{{TotalPayment}}", dto.TotalPayment.ToString("N0"));
            mess = mess.Replace("{{TotalProduct}}", dto.OrderDetails.Count().ToString("N0"));
            await ConfigEmail(mess, dto.ShipEmail, EmailConstant.SubjectCustomer);
            await ConfigEmail(mess, EmailConstant.EmailShop, EmailConstant.SubjectShop);

            return true;
        }


    }
}
