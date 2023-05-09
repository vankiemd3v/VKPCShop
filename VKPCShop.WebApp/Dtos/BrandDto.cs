using Microsoft.Build.Framework;
using VKPCShop.Data.Entities;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace VKPCShop.WebApp.Dtos
{
    public class BrandDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        public string Name { get; set; }
        public string? Image { get; set; }
        [DataType(DataType.Upload)]
        public IFormFile? formFile { get; set; }
    }
}
