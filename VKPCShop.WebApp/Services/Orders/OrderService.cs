using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using VKPCShop.Data.EF;
using VKPCShop.Data.Entities;
using VKPCShop.WebApp.Dtos;
using VKPCShop.WebApp.ViewModel;

namespace VKPCShop.WebApp.Services.Orders
{
    public class OrderService: IOrderService
    {
        private readonly VKPCShopDbContext _context;
        private readonly IMapper _mapper;
        public OrderService(VKPCShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> Create(OrderDto dto)
        {
            try
            {
                bool continueOrder = true;
                foreach (var item in dto.OrderDetails)
                {
                    var product = await _context.Products.Where(x => x.Id == item.ProductId).SingleOrDefaultAsync();
                    if (product.Quantity < item.BuyQuantity)
                    {
                        continueOrder = false;
                    }
                }
                if (continueOrder)
                {
                    // thêm đơn hàng
                    var order = new Order();
                    order.UserId = dto.UserId;
                    order.ShipPhoneNumber = dto.ShipPhoneNumber;
                    order.ShipName = dto.ShipName;
                    order.ShipAddress = dto.ShipAddress;
                    order.ShipEmail = dto.ShipEmail;
                    order.Status = dto.Status;
                    order.TotalPayment = dto.TotalPayment;
                    order.CreatedDate = dto.CreatedDate;
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // thêm chi tiết đơn hàng
                    var orderDetails = new List<OrderDetail>();
                    foreach (var item in dto.OrderDetails)
                    {
                        orderDetails.Add(new OrderDetail()
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            BuyQuantity = item.BuyQuantity,
                            Price = item.Price,
                        });
                        var product = await _context.Products.Where(x => x.Id == item.ProductId).SingleOrDefaultAsync();
                        if (product.Quantity >= item.BuyQuantity)
                        {
                            product.Quantity = product.Quantity - item.BuyQuantity;
                            product.Sold = product.Sold + item.BuyQuantity;
                        }
                    }
                    foreach (var item in orderDetails)
                    {
                        _context.OrderDetails.Add(item);
                    }
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

        public async Task<List<OrderDto>> GetOrders(int id)
        {
            var orders = await _context.Orders.Where(x => x.UserId == id).OrderByDescending(x=>x.CreatedDate).ToListAsync();
            foreach (var item in orders)
            {
                var orderDetails = await _context.OrderDetails.Where(x => x.OrderId == item.Id).ToListAsync();
                item.OrderDetails = orderDetails;
            }
            var ordersDto = _mapper.Map<List<OrderDto>>(orders);
            return ordersDto;
        }
        public async Task<OrderDto> GetById(int id)
        {
            var order = await _context.Orders.Where(x => x.Id == id).SingleOrDefaultAsync();
            var orderDetails = await _context.OrderDetails.Where(x => x.OrderId == id).ToListAsync();
            order.OrderDetails = orderDetails;
            var orderDto = _mapper.Map<OrderDto>(order);
            return orderDto;
        }
        public async Task<List<int>> GetProductIds(int id)
        {
            var productIds = await _context.OrderDetails.Where(x => x.OrderId == id).Select(x => x.ProductId).ToListAsync();
            return productIds;
        }

        public async Task<PagingResult<OrderDto>> GetAllPaging(PagingDto dto)
        {
            var orders = await _context.Orders.OrderByDescending(x=>x.CreatedDate).ToListAsync();
            if(dto.Keyword != null)
            {
                orders = await _context.Orders.Where(x => x.ShipName.Contains(dto.Keyword)).OrderByDescending(x => x.CreatedDate).ToListAsync();
            }
            // tổng item
            int count = orders.Count();
            // phân trang
            var data = orders.Skip((dto.PageIndex - 1) * dto.PageSize).Take(dto.PageSize).ToList();
            var ordersDto = _mapper.Map<List<OrderDto>>(data);
            var pagedResult = new PagingResult<OrderDto>()
            {
                Items = ordersDto,
                Count = count,
                PageIndex = dto.PageIndex,
                PageSize = dto.PageSize,
            };
            return pagedResult;
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
                _context.Remove(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public async Task<bool> Update(int id, string status)
        {
            try
            {
                if(status != "Hủy")
                {
                    var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
                    order.Status = status;
                    _context.Orders.UpdateRange(order);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
                    order.Status = status;
                    _context.Orders.UpdateRange(order);
                    var listOrderDetail = await _context.OrderDetails.Where(x => x.OrderId == id).ToListAsync();
                    foreach(var item in listOrderDetail)
                    {
                        var product = await _context.Products.FirstOrDefaultAsync(x=>x.Id == item.ProductId);
                        product.Quantity += item.BuyQuantity;
                        product.Sold -= item.BuyQuantity;
                        _context.Products.UpdateRange(product);
                    }
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        public async Task<int> CountOrderConfirm()
        {
            var count = await _context.Orders.Where(x=>x.Status == "Chờ duyệt").CountAsync();
            return count;
        }

        public async Task<long> TotalEarnings()
        {
            var list = await _context.Orders.Where(x=>x.Status == "Hoàn thành").Select(x => x.TotalPayment).ToListAsync();
            long count = 0;
            foreach (var item in list)
            {
                count += item;
            }
            return count;
        }
    }
}
