using System.ComponentModel.DataAnnotations;

namespace VKPCShop.WebApp.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string? Image { get; set; }
        public int? ParentCategoryId { get; set; }
        [DataType(DataType.Upload)]
        public IFormFile? formFile { get; set; }
        //public List<ProductDto> Products { get; set; }
    }
}
