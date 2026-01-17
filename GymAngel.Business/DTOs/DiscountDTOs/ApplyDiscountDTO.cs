using System.ComponentModel.DataAnnotations;

namespace GymAngel.Business.DTOs.DiscountDTOs
{
    public class ApplyDiscountDTO
    {
        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
