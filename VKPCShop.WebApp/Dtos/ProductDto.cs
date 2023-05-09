using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using VKPCShop.Data.Entities;
using Xunit.Sdk;

namespace VKPCShop.WebApp.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage ="Dữ liệu không được trống!")]
        public string Name { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Nhập giá bán")]
        public int Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Nhập giá khuyến mãi")]
        public int SalePrice { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Nhập số lượng")]
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public int ViewCount { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Dữ liệu không được trống!")]
        public string Config { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Dữ liệu không được trống!")]
        public string Description { get; set; }
        public string? Image { get; set; }
        public List<string>? Images { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Chọn thương hiệu")]
        public int BrandId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Chọn danh mục")]
        public int CategoryId { get; set; }
        //public BrandDto Brand { get; set; }
        //public CategoryDto Category { get; set; }
        //public List<ImageDto> Images { get; set; }
        public IFormFile formFile { get; set; }
        public List<IFormFile> formFiles { get; set; }
    }
}
