using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKPCShop.Data.Entities
{
	[Table("Products")]
	public class Product
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
		public int CategoryId { get; set; }
		[ForeignKey("BrandId")]
		public Brand Brand { get; set; }
		[ForeignKey("CategoryId")]
		public Category Category { get; set; }
		public List<Image> Images { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
