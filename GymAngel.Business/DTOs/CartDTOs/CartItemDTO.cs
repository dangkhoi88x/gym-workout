namespace GymAngel.Business.DTOs.CartDTOs
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
        
        // Stock information
        public int StockAvailable { get; set; }
        public bool IsInStock => StockAvailable > 0;
        public bool IsLowStock => StockAvailable > 0 && StockAvailable <= 5;
    }
}
