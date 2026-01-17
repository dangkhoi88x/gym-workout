namespace GymAngel.Business.DTOs.DiscountDTOs
{
    public class DiscountCodeDTO
    {
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal DiscountAmount { get; set; } // Calculated amount for current cart
    }
}
