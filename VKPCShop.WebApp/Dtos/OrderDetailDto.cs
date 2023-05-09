namespace VKPCShop.WebApp.Dtos
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int BuyQuantity { set; get; }
        public int Price { set; get; }
    }
}
