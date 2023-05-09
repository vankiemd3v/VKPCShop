using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKPCShop.Data.EF
{
	public class VKPCShopDbContextFactory : IDesignTimeDbContextFactory<VKPCShopDbContext>
	{
		public VKPCShopDbContext CreateDbContext(string[] args)
		{
			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			var connectionString = configuration.GetConnectionString("VKPCShopDb");

			var optionsBuilder = new DbContextOptionsBuilder<VKPCShopDbContext>();
			optionsBuilder.UseSqlServer(connectionString);

			return new VKPCShopDbContext(optionsBuilder.Options);
		}
	}
}
