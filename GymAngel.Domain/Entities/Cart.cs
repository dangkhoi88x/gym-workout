using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymAngel.Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Discount
        public int? DiscountCodeId { get; set; }

        // Navigation
        public User User { get; set; } = null!;
        public ICollection<CartItem>? CartItems { get; set; }
        public DiscountCode? DiscountCode { get; set; }
    }
}
