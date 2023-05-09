using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Net.WebSockets;
using VKPCShop.Data.EF;
using VKPCShop.Data.Entities;
using VKPCShop.WebApp.Constant;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.ViewModel;

namespace VKPCShop.WebApp.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly VKPCShopDbContext _context;
        private readonly IMapper _mapper;
        public ProductService(VKPCShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> Create(ProductDto dto)
        {
            try
            {
                var product = new Product()
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    SalePrice = dto.SalePrice,
                    Quantity = dto.Quantity,
                    Description = dto.Description,
                    Config = dto.Config,
                    ViewCount = 0,
                    Sold = 0,
                    Image = dto.Image,
                    BrandId = dto.BrandId,
                    CategoryId = dto.CategoryId,
                };
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                var productId = product.Id;
                var images = new List<Image>();
                foreach(var item in dto.Images)
                {
                    images.Add(new Image()
                    {
                        Name = item,
                        ProductId = productId,
                    });
                }
                foreach(var item in images)
                {
                    _context.Images.Add(item);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                var images = await _context.Images.Where(x => x.ProductId == id).ToListAsync();
                foreach (var image in images)
                {
                    _context.Images.Remove(image);
                }
                var product = await _context.Products.Where(x => x.Id == id).SingleOrDefaultAsync();
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }

        }

        public async Task<PagingResult<ProductDto>> GetAllAdmin(PagingDto dto)
        {
            var products = new List<Product>();
            products = await _context.Products.ToListAsync();
            if (!string.IsNullOrEmpty(dto.Keyword))
            {
                products = await _context.Products.Where(x => x.Name.Contains(dto.Keyword)).ToListAsync();
            }
            // tổng item
            int count = products.Count();
            // phân trang
            var data = products.Skip((dto.PageIndex - 1) * dto.PageSize).Take(dto.PageSize).ToList();
            List<ProductDto> productsDto = _mapper.Map<List<ProductDto>>(data);
            var pagedResult = new PagingResult<ProductDto>()
            {
                Items = productsDto,
                Count = count,
                PageIndex = dto.PageIndex,
                PageSize = dto.PageSize,
            };
            return pagedResult;
        }

        public async Task<ProductDto> GetById(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if(product != null)
            {
                var productVm = _mapper.Map<ProductDto>(product);
                return productVm;
            }
            return null;
             
        }

        public async Task<bool> Update(ProductUpdateDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if(product != null)
            {
                product.SalePrice = dto.SalePrice;
                product.Price = dto.Price;
                product.Quantity = dto.Quantity;
                product.BrandId = dto.BrandId;
                product.CategoryId = dto.CategoryId;
                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Config = dto.Config;
                if(dto.Image != null)
                {
                    product.Image = dto.Image;
                }
                _context.Products.UpdateRange(product);
                if(dto.Images != null)
                {
                    var images = await _context.Images.Where(x => x.ProductId == product.Id).ToListAsync();
                    foreach(var item in images)
                    {
                        _context.Images.Remove(item);
                    }
                    foreach(var item in dto.Images)
                    {
                        var image = new Image() {
                            Name = item,
                            ProductId = product.Id,
                        };
                        _context.Images.Add(image);
                    }
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task<List<ProductVm>> GetAll(bool bestSell, bool sale, bool casePC, int? categoryId, bool ascending, bool descending, int? price, List<string> brands, int? brandId, int? productId, string? keyword)
        {
            var products = from p in _context.Products
                        join b in _context.Brands on p.BrandId equals b.Id
                        join c in _context.Categories on p.CategoryId equals c.Id
                        select new {p,b, c};
            if (bestSell)
                products = products.OrderByDescending(x => x.p.Sold);              // lấy 50 sp bán chạy nhất giảm dần
            if (sale)
                products = products.Where(x=>x.p.SalePrice != 0);                           // lấy những sp khuyến mãi
            if (casePC)
                products = products.Where(x => x.c.Name == FilterProductConstant.PcGaming);// lấy những sp là case lắp sẵn
            if (categoryId > 0) // lấy sp theo danh mục
            {
                // Lấy sp theo danh mục mặc định
                if (!ascending && !descending)
                {
                    var productsByCategory = products.Where(x => x.c.Id == categoryId).Count();
                    if (productsByCategory == 0)
                    {
                        products = products.Where(x => x.c.ParentCategoryId == categoryId);      // Nếu categoryId truyền vào là danh mục cha thì lấy danh sách những sản phẩm có parentId = categoryId truyền vào
                    }
                    else
                    {
                        products = products.Where(x => x.c.Id == categoryId);                   // Còn không thì lấy bth
                    }
                }
                // Lấy sp theo danh mục sắp xếp giá tăng giần
                else if (ascending)
                {
                    var productsByCategory = products.Where(x => x.c.Id == categoryId).Count();
                    if (productsByCategory == 0)
                    {
                        products = products.Where(x => x.c.ParentCategoryId == categoryId).OrderBy(x => x.p.Price);      // Nếu categoryId truyền vào là danh mục cha thì lấy danh sách những sản phẩm có parentId = categoryId truyền vào
                    }
                    else
                    {
                        products = products.Where(x => x.c.Id == categoryId).OrderBy(x => x.p.Price);                   // Còn không thì lấy bth
                    }
                }
                // Lấy sp theo danh mục sắp xếp giá giảm dần
                else if (descending)
                {
                    var productsByCategory = products.Where(x => x.c.Id == categoryId).Count();
                    if (productsByCategory == 0)
                    {
                        products = products.Where(x => x.c.ParentCategoryId == categoryId).OrderByDescending(x => x.p.Price);      // Nếu categoryId truyền vào là danh mục cha thì lấy danh sách những sản phẩm có parentId = categoryId truyền vào
                    }
                    else
                    {
                        products = products.Where(x => x.c.Id == categoryId).OrderByDescending(x => x.p.Price);                   // Còn không thì lấy bth
                    }
                }

                // Lấy sp theo giá
                if (price != null)
                    products = products.Where(x => x.p.Price < price * 1000000 || (x.p.SalePrice > 0 && x.p.SalePrice < price * 1000000));
                if (brands.Count > 0)
                {
                    products = products.Where(x => brands.Contains(x.b.Name));
                }
            }
            else // Lấy tất cả sp
            {
                // Tăng dần

                if (ascending)
                {
                    products = products.OrderBy(x => x.p.Price);
                }
                // Giảm dần
                else if (descending)
                {
                    products = products.OrderByDescending(x => x.p.Price);
                }

                // Lấy sp theo giá
                if (price != null)
                    products = products.Where(x => x.p.Price < price * 1000000 || (x.p.SalePrice > 0 && x.p.SalePrice < price * 1000000));
                if(brands != null)
                {
                    if (brands.Count > 0)
                    {
                        products = products.Where(x => brands.Contains(x.b.Name));
                    }
                }
            }
            if(brandId > 0)
            {
                products = products.Where(x => x.p.BrandId == brandId && x.p.Id != productId);
            }
            if(keyword != null)
            {
                // Lấy sp theo danh mục mặc định
                if (!ascending && !descending)
                {
                    products = products.Where(x=>x.p.Name.Contains(keyword));
                }
                // Lấy sp theo danh mục sắp xếp giá tăng giần
                else if (ascending)
                {
                    products = products.Where(x => x.p.Name.Contains(keyword)).OrderBy(x => x.p.Price);
                }
                // Lấy sp theo danh mục sắp xếp giá giảm dần
                else if (descending)
                {
                    products = products.Where(x => x.p.Name.Contains(keyword)).OrderByDescending(x => x.p.Price);
                }

                // Lấy sp theo giá
                if (price != null)
                    products = products.Where(x => x.p.Price < price * 1000000 || (x.p.SalePrice > 0 && x.p.SalePrice < price * 1000000) && x.p.Name.Contains(keyword));
                if(brands != null)
                {
                    if (brands.Count > 0)
                    {
                        products = products.Where(x => brands.Contains(x.b.Name) && x.p.Name.Contains(keyword));
                    }
                }
            }
            var productVm = new List<ProductVm>();
            foreach (var item in products)
            {
                productVm.Add(new ProductVm()
                {
                    Id = item.p.Id,
                    Name = item.p.Name,
                    Price = item.p.Price,
                    SalePrice = item.p.SalePrice,
                    Quantity = item.p.Quantity,
                    ViewCount = item.p.ViewCount,
                    Config = item.p.Config,
                    Description = item.p.Description,
                    BrandId = item.p.BrandId,
                    CategoryId = item.p.CategoryId,
                    BrandName = item.b.Name,
                    CategoryName = item.c.Name,
                    Image = item.p.Image,
                    ParentCategoryId = item.c.ParentCategoryId
                });
            }
            return productVm;
        }

        public async Task<List<ProductVm>> GetProductsVmById(int id)
        {
            var products =  from p in _context.Products
                           join b in _context.Brands on p.BrandId equals b.Id
                           join c in _context.Categories on p.CategoryId equals c.Id
                           join od in _context.OrderDetails on p.Id equals od.ProductId
                           select new { p, b, c, od };
            products = products.Where(x => x.od.OrderId == id);
            var productVm = new List<ProductVm>();
            foreach (var item in products)
            {
                productVm.Add(new ProductVm()
                {
                    Id = item.p.Id,
                    Name = item.p.Name,
                    Price = item.p.Price,
                    SalePrice = item.p.SalePrice,
                    Quantity = item.p.Quantity,
                    ViewCount = item.p.ViewCount,
                    Config = item.p.Config,
                    Description = item.p.Description,
                    BrandId = item.p.BrandId,
                    CategoryId = item.p.CategoryId,
                    BrandName = item.b.Name,
                    CategoryName = item.c.Name,
                    Image = item.p.Image,
                    ParentCategoryId = item.c.ParentCategoryId,
                    OrderId = item.od.OrderId
                });
            }
            return productVm;
        }

        public async Task<int> CountSold()
        {
            var list = await _context.Products.Select(x => x.Sold).ToListAsync();
            int count = 0;
            foreach(var sold in list)
            {
                count += sold;
            }
            return count;
        }
        public async Task<int> CountProduct()
        {
            return await _context.Products.CountAsync();
        }
    }
}
