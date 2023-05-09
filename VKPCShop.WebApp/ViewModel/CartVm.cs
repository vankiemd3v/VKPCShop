namespace VKPCShop.WebApp.ViewModel
{
    public class CartVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int SalePrice { get; set; }
        public int Quantity { get; set; }
        public int BuyQuantity { get; set; }

        public string Image { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
