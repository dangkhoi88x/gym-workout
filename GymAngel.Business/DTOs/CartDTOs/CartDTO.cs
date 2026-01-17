using GymAngel.Business.DTOs.DiscountDTOs;

namespace GymAngel.Business.DTOs.CartDTOs
{
    public class CartDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDTO> Items { get; set; } = new();
        public DiscountCodeDTO? AppliedDiscount { get; set; }
        
        public decimal SubtotalBeforeDiscount => Items.Sum(i => i.Subtotal);
        public decimal DiscountAmount => AppliedDiscount?.DiscountAmount ?? 0;
        public decimal FinalTotal => SubtotalBeforeDiscount - DiscountAmount;
        public decimal TotalAmount => FinalTotal; // Alias for backwards compatibility
        
        public int TotalItems => Items.Sum(i => i.Quantity);
        public DateTime UpdatedAt { get; set; }
    }
}
