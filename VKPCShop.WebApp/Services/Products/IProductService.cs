using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.ViewModel;

namespace VKPCShop.WebApp.Services.Products
{
    public interface IProductService
    {
        Task<bool> Create(ProductDto dto);
        Task<bool> Update(ProductUpdateDto dto);
        Task<bool> Delete(int id);
        Task<ProductDto> GetById(int id);
        Task<List<ProductVm>> GetProductsVmById(int id);
        Task<PagingResult<ProductDto>> GetAllAdmin(PagingDto dto);
        Task<List<ProductVm>> GetAll(bool bestSell, bool sale, bool casePC, int? categoryId, bool ascending, bool descending, int? price, List<string> brands, int? brandId, int? productId, string? keyworrd);
        Task<int> CountSold();
        Task<int> CountProduct();
    }
}
