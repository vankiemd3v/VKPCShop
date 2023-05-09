using Microsoft.Build.Framework;
using VKPCShop.Data.Entities;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace VKPCShop.WebApp.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalPayment { get; set; }
        public string Status { get; set; }
        public string ShipName { get; set; }
        public string ShipEmail { get; set; }
        public string ShipPhoneNumber { get; set; }
        public string ShipAddress { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}
