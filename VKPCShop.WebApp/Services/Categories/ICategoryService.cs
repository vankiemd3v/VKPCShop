using VKPCShop.WebApp.Dtos;

namespace VKPCShop.WebApp.Services.Categories
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAll(string? keyword);
        Task<CategoryDto> GetById(int id);
        Task<bool> Create(CategoryDto dto);
        Task<bool> Update(CategoryDto dto);
        Task<bool> Delete(int id);
        Task<List<CategoryDto>> GetParents();
        Task<List<CategoryDto>> GetChildrens();
    }
}
