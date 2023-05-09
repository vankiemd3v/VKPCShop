using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VKPCShop.Data.EF;
using VKPCShop.Data.Entities;
using VKPCShop.WebApp.Dtos;

namespace VKPCShop.WebApp.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly VKPCShopDbContext _context;
        public CategoryService(VKPCShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> Create(CategoryDto dto)
        {
            try
            {
                var category = new Category();
                category.Name = dto.Name;
                category.ParentCategoryId = dto.ParentCategoryId;
                if(dto.Image != null)
                {
                    category.Image = dto.Image;
                }
                _context.Categories.Add(category);
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
                var delete = await _context.Categories.Where(u => u.Id == id).SingleOrDefaultAsync();
                if (delete != null)
                {
                    _context.Categories.Remove(delete);
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

        public async Task<List<CategoryDto>> GetAll(string? keyword)
        {
            List<Category> list = new List<Category>();
            if (!string.IsNullOrEmpty(keyword))
            {
                list = await _context.Categories.Where(x => x.Name.Contains(keyword)).ToListAsync();
            }
            else
            {
                list = await _context.Categories.ToListAsync();
            }
            var listDto = new List<CategoryDto>();
            foreach (var item in list)
            {
                CategoryDto dto = _mapper.Map<CategoryDto>(item);
                listDto.Add(dto);
            }
            return listDto;
        }

        public async Task<CategoryDto> GetById(int id)
        {
            var get = await _context.Categories.Where(x => x.Id == id).SingleOrDefaultAsync();
            var categoryDto = _mapper.Map<CategoryDto>(get);
            return categoryDto;
        }

        public async Task<List<CategoryDto>> GetParents()
        {
            var parens = await _context.Categories.Where(x => x.ParentCategoryId == 0).ToListAsync();
            var parensDto = _mapper.Map<List<CategoryDto>>(parens);
            return parensDto;
        }
        public async Task<List<CategoryDto>> GetChildrens()
        {
            var parens = await _context.Categories.Where(x => x.ParentCategoryId != 0).ToListAsync();
            var parensDto = _mapper.Map<List<CategoryDto>>(parens);
            return parensDto;
        }

        public async Task<bool> Update(CategoryDto dto)
        {
            try
            {
                var update = await _context.Categories.Where(x => x.Id == dto.Id).SingleOrDefaultAsync();
                update.Name = dto.Name;
                update.ParentCategoryId = dto.ParentCategoryId;
                if(dto.Image != null)
                {
                    update.Image = dto.Image;
                }
                _context.Categories.UpdateRange(update);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }
    }
}
