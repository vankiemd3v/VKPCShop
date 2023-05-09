using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.IO;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.Services.Brands;
using VKPCShop.WebApp.Services.Categories;
using VKPCShop.WebApp.Services.Products;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VKPCShop.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductService productService, ICategoryService categoryService, IBrandService brandService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _webHostEnvironment = webHostEnvironment;

        }
        public async Task<IActionResult> Index(string? keyword, int pageIndex)
        {
            ViewBag.Result = TempData["success"];
            if (pageIndex == 0)
                pageIndex = 1;
            var dto = new PagingDto()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = 10
            };
            var paging = await _productService.GetAllAdmin(dto);
            return View(paging);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Price = 0;
            ViewBag.SalePrice = 0;
            ViewBag.Quantity = 0;
            ViewBag.Brands = await _brandService.GetAll(null);
            ViewBag.Categories = await _categoryService.GetAll(null);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            // kiểm tra validation
            if (!ModelState.IsValid)
            {
                if (dto.BrandId != null)
                {
                    ViewBag.BrandId = dto.BrandId;
                }
                if (dto.CategoryId != null)
                {
                    ViewBag.CategoryId = dto.CategoryId;
                }
                if (dto.Name != null)
                {
                    ViewBag.Name = dto.Name;
                }
                if (dto.Price > 0)
                {
                    ViewBag.Price = dto.Price;
                }
                else
                {
                    ViewBag.Price = 0;
                }
                if (dto.SalePrice > 0)
                {
                    ViewBag.SalePrice = dto.SalePrice;
                }
                else
                {
                    ViewBag.SalePrice = 0;
                }
                if (dto.Quantity > 0)
                {
                    ViewBag.Quantity = dto.Quantity;
                }
                else
                {
                    ViewBag.Quantity = 0;
                }
                if (dto.Config != null)
                {
                    ViewBag.Config = dto.Config;
                }
                if (dto.Description != null)
                {
                    ViewBag.Description = dto.Description;
                }
                ViewBag.Brands = await _brandService.GetAll(null);
                ViewBag.Categories = await _categoryService.GetAll(null);
                return View(dto);
            }
            SaveImage(dto);
            var created = await _productService.Create(dto);
            if (created)
                TempData["success"] = "success";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Brands = await _brandService.GetAll(null);
            ViewBag.Categories = await _categoryService.GetAll(null);
            var product = await _productService.GetById(id);
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateDto dto)
        {
            // kiểm tra validation
            if (!ModelState.IsValid)
            {
                if (dto.BrandId != null)
                {
                    ViewBag.BrandId = dto.BrandId;
                }
                if (dto.CategoryId != null)
                {
                    ViewBag.CategoryId = dto.CategoryId;
                }
                if (dto.Name != null)
                {
                    ViewBag.Name = dto.Name;
                }
                if (dto.Price > 0)
                {
                    ViewBag.Price = dto.Price;
                }
                else
                {
                    ViewBag.Price = 0;
                }
                if (dto.SalePrice > 0)
                {
                    ViewBag.SalePrice = dto.SalePrice;
                }
                else
                {
                    ViewBag.SalePrice = 0;
                }
                if (dto.Quantity > 0)
                {
                    ViewBag.Quantity = dto.Quantity;
                }
                else
                {
                    ViewBag.Quantity = 0;
                }
                if (dto.Config != null)
                {
                    ViewBag.Config = dto.Config;
                }
                if (dto.Description != null)
                {
                    ViewBag.Description = dto.Description;
                }
                ViewBag.Brands = await _brandService.GetAll(null);
                ViewBag.Categories = await _categoryService.GetAll(null);
                return View(dto);
            }
            if(dto.formFiles != null)
            {
                SaveImage(dto);
            }
            var updated = await _productService.Update(dto);
            if (updated)
                TempData["success"] = "success";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.Delete(id);
            if (deleted)
                TempData["success"] = "success";
            return RedirectToAction("Index");
        }
        public void SaveImage(ProductDto dto)
        {
            string fileName = Guid.NewGuid().ToString() + "-" + dto.formFile.FileName;
            string targetPath = Path.Combine(_webHostEnvironment.WebRootPath, "admin\\images", fileName);
            using (var stream = new FileStream(targetPath, FileMode.Create))
            {
                dto.formFile.CopyTo(stream);
            }
            dto.Image = fileName;


            var listImage = new List<string>();
            if (dto.formFiles != null)
            {
                foreach (var item in dto.formFiles)
                {
                    var image = Guid.NewGuid().ToString() + "-" + item.FileName;
                    string targetPaths = Path.Combine(_webHostEnvironment.WebRootPath, "admin\\images", image);
                    using (var streams = new FileStream(targetPaths, FileMode.Create))
                    {
                        item.CopyTo(streams);
                    }
                    listImage.Add(image);
                }
            }
            dto.Images = listImage;
        }
        public void SaveImage(ProductUpdateDto dto)
        {
            if(dto.formFile != null)
            {
                string fileName = Guid.NewGuid().ToString() + "-" + dto.formFile.FileName;
                string targetPath = Path.Combine(_webHostEnvironment.WebRootPath, "admin\\images", fileName);
                using (var stream = new FileStream(targetPath, FileMode.Create))
                {
                    dto.formFile.CopyTo(stream);
                }
                dto.Image = fileName;
            }
            var listImage = new List<string>();
            if(dto.formFiles != null)
            {
                foreach (var item in dto.formFiles)
                {
                    var image = Guid.NewGuid().ToString() + "-" + item.FileName;
                    string targetPaths = Path.Combine(_webHostEnvironment.WebRootPath, "admin\\images", image);
                    using (var streams = new FileStream(targetPaths, FileMode.Create))
                    {
                        item.CopyTo(streams);
                    }
                    listImage.Add(image);
                }
            }
            dto.Images = listImage;
        }
    }
}
