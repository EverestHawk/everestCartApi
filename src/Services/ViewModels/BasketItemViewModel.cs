namespace Services.ViewModels
{
    public class BasketItemViewModel
    {
        public int Id { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int CatalogItemId { get; set; }
        public string ProductName { get; set; }
        public string PictureUri { get; set; }
    }
}
