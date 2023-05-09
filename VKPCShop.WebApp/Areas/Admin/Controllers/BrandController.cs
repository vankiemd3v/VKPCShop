using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.Services.Brands;

namespace VKPCShop.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : BaseController
    {
        private readonly IBrandService _brandService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BrandController(IBrandService brandService, IWebHostEnvironment webHostEnvironment)
        {
            _brandService = brandService;
            _webHostEnvironment = webHostEnvironment;

        }
        public async Task<IActionResult> Index(string? keyword)
        {
            ViewBag.NotifyBrand = TempData["result"];
            var list = await _brandService.GetAll(keyword);
            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> Create(BrandDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["result"] = "fail";
                return RedirectToAction("Index");
            }
            SaveImage(dto);
            var create = await _brandService.Create(dto);
            if (create)
            {
                TempData["result"] = "success";
                return RedirectToAction("Index");
            }
            TempData["result"] = "fail";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var get = await _brandService.GetById(id);
            if (get != null)
            {
                return Json(new { status = true, name = get.Name });
            }
            return Json(new
            {
                status = false
            });
        }
        [HttpPost]
        public async Task<IActionResult> Update(BrandDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["result"] = "fail";
                return RedirectToAction("Index");
            }
            if(dto.formFile != null)
            {
                SaveImage(dto);
            }
            var create = await _brandService.Update(dto);
            if (create)
            {
                TempData["result"] = "success";
                return RedirectToAction("Index");
            }
            TempData["result"] = "fail";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var delete = await _brandService.Delete(id);
            if (delete)
            {
                return Json(new {status = true });
            }
            return Json(new { status = false });
        }
        public void SaveImage(BrandDto dto)
        {
            string fileName = Guid.NewGuid().ToString() + "-" + dto.formFile.FileName;
            string targetPath = Path.Combine(_webHostEnvironment.WebRootPath, "admin\\images", fileName);
            using (var stream = new FileStream(targetPath, FileMode.Create))
            {
                dto.formFile.CopyTo(stream);
            }
            dto.Image = fileName;
        }
    }
}
