using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKPCShop.Data.Entities
{
	[Table("Images")]
	public class Image
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public string Name { get; set; }
        public int ProductId { get; set; }
		[ForeignKey("ProductId")]
        public Product Product { get; set; }
	}
}
