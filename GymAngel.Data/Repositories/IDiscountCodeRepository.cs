using GymAngel.Domain.Entities;

namespace GymAngel.Data.Repositories
{
    public interface IDiscountCodeRepository
    {
        Task<DiscountCode?> GetByCodeAsync(string code);
        Task IncrementUsageAsync(int codeId);
    }
}
