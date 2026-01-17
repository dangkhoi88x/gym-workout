using System.ComponentModel.DataAnnotations;

namespace GymAngel.Business.DTOs.CartDTOs
{
    public class AddToCartDTO
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
    }
}
