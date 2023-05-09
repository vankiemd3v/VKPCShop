using Microsoft.AspNetCore.Mvc;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.Services.Orders;
using VKPCShop.WebApp.Services.Products;

namespace VKPCShop.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrderController(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }
        public async Task<IActionResult> Index(string? keyword, int pageIndex)
        {
            ViewBag.Active = TempData["deleteOrder"];
            ViewBag.Keyword = keyword;
            if(pageIndex == 0)
                pageIndex = 1;
            var dto = new PagingDto()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = 10
            };
            var paging = await _orderService.GetAllPaging(dto);
            return View(paging);
        }
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.ListStatus = new List<string>()
            {
                "Chờ duyệt",
                "Đang chuẩn bị hàng",
                "Đang giao",
                "Hoàn thành",
                "Hủy",
            };
            var order = await _orderService.GetById(id);
            var products = new List<ProductDto>();
            foreach (var item in order.OrderDetails)
            {
                var product = await _productService.GetById(item.ProductId);
                products.Add(product);
            }
            ViewBag.Product = products;
            return View(order);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, string status)
        {
            var update = await _orderService.Update(id, status);
            if (update)
                return Json(new {result = true});
            return Json(new {result = false});
        }
        public async Task<IActionResult> Delete(int id)
        {
            var delete = await _orderService.Delete(id);
            if (delete)
                TempData["deleteOrder"] = "deleteSuccess";
            else
                TempData["deleteOrder"] = "deleteError";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CountOrderConfirm()
        {
            var count = await _orderService.CountOrderConfirm();
            return Json(new {count = count});
        }
    }
}
