using GymAngel.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAngel.Data.Repositories
{
    public class DiscountCodeRepository : IDiscountCodeRepository
    {
        private readonly ApplicationDbContext _context;

        public DiscountCodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DiscountCode?> GetByCodeAsync(string code)
        {
            return await _context.DiscountCodes
                .FirstOrDefaultAsync(dc => dc.Code == code && dc.IsActive);
        }

        public async Task IncrementUsageAsync(int codeId)
        {
            var discountCode = await _context.DiscountCodes.FindAsync(codeId);
            if (discountCode != null)
            {
                discountCode.UsedCount++;
                await _context.SaveChangesAsync();
            }
        }
    }
}
