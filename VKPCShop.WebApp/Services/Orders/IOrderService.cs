using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Asn1.X509;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.ViewModel;

namespace VKPCShop.WebApp.Services.Orders
{
    public interface IOrderService
    {
        Task<bool> Create(OrderDto dto);
        Task<List<OrderDto>> GetOrders(int id);
        Task<OrderDto> GetById(int id);
        Task<List<int>> GetProductIds(int id);
        Task<PagingResult<OrderDto>> GetAllPaging(PagingDto dto);
        Task<bool> Delete(int id);
        Task<bool> Update(int id, string status);
        Task<int> CountOrderConfirm();
        Task<long> TotalEarnings();

    }
}
