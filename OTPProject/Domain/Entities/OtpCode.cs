namespace OTPProject.Domain.Entities
{
    public class OtpCode
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PhoneNumber { get; set; } = default!; // normalized E.164 preferred
        public string CodeHash { get; set; } = default!;    // store hash, not raw code
        public DateTimeOffset ExpiresAtUtc { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }
        public bool Used { get; set; }
        public int AttemptCount { get; set; }
    }
}