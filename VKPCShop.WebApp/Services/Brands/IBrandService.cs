using VKPCShop.WebApp.Dtos;

namespace VKPCShop.WebApp.Services.Brands
{
    public interface IBrandService
    {
        Task<bool> Create(BrandDto dto);
        Task<bool> Update(BrandDto dto);
        Task<bool> Delete(int id);
        Task<BrandDto> GetById(int id);
        Task<List<BrandDto>> GetAll(string? keyword);
        Task<List<BrandDto>> GetBrands(List<string> brands);
    }
}
