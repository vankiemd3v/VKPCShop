using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace VKPCShop.WebApp.ViewModel
{
    public class ProductVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int SalePrice { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public int ViewCount { get; set; }
        public string Config { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public int OrderId { get; set; }
    }
}
