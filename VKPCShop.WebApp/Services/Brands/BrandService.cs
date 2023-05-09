using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using VKPCShop.Data.EF;
using VKPCShop.Data.Entities;
using VKPCShop.WebApp.Dtos;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace VKPCShop.WebApp.Services.Brands
{
    public class BrandService : IBrandService
    {
        private readonly IMapper _mapper;
        private readonly VKPCShopDbContext _context;
        public BrandService(VKPCShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> Create(BrandDto dto)
        {
            try
            {
                Brand brand = _mapper.Map<Brand>(dto);
                _context.Brands.Add(brand);
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
                var delete = await _context.Brands.Where(u => u.Id == id).SingleOrDefaultAsync();
                if (delete != null)
                {
                    _context.Brands.Remove(delete);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<BrandDto>> GetAll(string? keyword)
        {
            List<Brand> list = new List<Brand>();
            if (!string.IsNullOrEmpty(keyword))
            {
                list = await _context.Brands.Where(x => x.Name.Contains(keyword)).ToListAsync();
            }
            else
            {
                list = await _context.Brands.ToListAsync();
            }
            var listDto = new List<BrandDto>();
            foreach(var item in list)
            {
                BrandDto dto = _mapper.Map<BrandDto>(item);
                listDto.Add(dto);
            }
            return listDto;
        }

        public async Task<List<BrandDto>> GetBrands(List<string> brands)
        {
            var list = await _context.Brands.Where(x => brands.Contains(x.Name)).ToListAsync();
            var listDto = _mapper.Map<List<BrandDto>>(list);
            return listDto; ;
        }

        public async Task<BrandDto> GetById(int id)
        {
            var get = await _context.Brands.Where(x => x.Id == id).SingleOrDefaultAsync();
            var map = _mapper.Map<BrandDto>(get);
            return map;
        }

        public async Task<bool> Update(BrandDto dto)
        {
            try
            {
                var update = await _context.Brands.Where(x => x.Id == dto.Id).SingleOrDefaultAsync();
                update.Name = dto.Name;
                if(dto.Image != null)
                {
                    update.Image = dto.Image;
                }
                _context.Brands.UpdateRange(update);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }
    }
}
