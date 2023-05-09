using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.Services.Users;

namespace VKPCShop.WebApp.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        public async override void OnActionExecuted(ActionExecutedContext context)
        {
            // check sessions nếu null thì đưa về trang login
            var userSession = context.HttpContext.Session.GetString("userSession");
            if (userSession == null)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
            else
            {
                UserSession user = JsonConvert.DeserializeObject<UserSession>(userSession);
                if(user.Role != "ADMIN")
                {
                    context.Result = new RedirectToActionResult("Index", "Login", null);
                }
            }
            base.OnActionExecuted(context);
        }
    }
}
