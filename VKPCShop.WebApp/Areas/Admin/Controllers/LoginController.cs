using Facebook;
using GoogleAuthentication.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.Services.Users;

namespace VKPCShop.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        public LoginController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }
        public IActionResult Index()
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
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("userSession");
            return RedirectToAction("Index");
        }
    }
}
