namespace GymAngel.Business.DTOs.CartDTOs
{
    public class SyncCartDTO
    {
        public List<SyncCartItemDTO> Items { get; set; } = new();
    }

    public class SyncCartItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
