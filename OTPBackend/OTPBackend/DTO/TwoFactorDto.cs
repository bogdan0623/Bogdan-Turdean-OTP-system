namespace OTPBackend.DTO
{
    public class TwoFactorDto
    {
        public int Id { get; set; }

        //public string Secret { get; set; } = default!;

        public string Code { get; set; } = default!;
    }
}
