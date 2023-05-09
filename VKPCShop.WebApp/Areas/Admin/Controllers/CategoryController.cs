using Microsoft.AspNetCore.Mvc;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.Services.Brands;
using VKPCShop.WebApp.Services.Categories;

namespace VKPCShop.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : BaseController
    {

        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CategoryController(ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
        {
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(string? keyword)
        {
            // thông báo cho thêm sửa xóa
            ViewBag.Notify = TempData["result"];
            var list = await _categoryService.GetAll(keyword);
            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["result"] = "fail";
                return RedirectToAction("Index");
            }
            SaveImage(dto);
            var create = await _categoryService.Create(dto);
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
            var get = await _categoryService.GetById(id);
            if (get != null)
            {
                return Json(new { status = true, name = get.Name, image = get.Image });
            }
            return Json(new
            {
                status = false
            });
        }
        [HttpPost]
        public async Task<IActionResult> Update(CategoryDto dto)
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
            var update = await _categoryService.Update(dto);
            if (update)
            {
                TempData["result"] = "success";
                return RedirectToAction("Index");
            }
            TempData["result"] = "fail";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var delete = await _categoryService.Delete(id);
            if (delete)
            {
                return Json(new { status = true });
            }
            return Json(new { status = false });
        }
        public void SaveImage(CategoryDto dto)
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
