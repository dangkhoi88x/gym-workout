namespace GymAngel.Business.DTOs.AuthDTOs
{
    public class LoginResultDTO
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
    }
}
