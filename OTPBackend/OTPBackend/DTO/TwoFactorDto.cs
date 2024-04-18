namespace OTPBackend.DTO
{
    public class TwoFactorDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = default!;
    }
}
