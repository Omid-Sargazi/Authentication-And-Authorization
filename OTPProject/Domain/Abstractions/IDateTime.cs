namespace OTPProject.Domain.Abstractions
{
    public interface IDateTime
    {
        DateTimeOffset UtcNow { get; }
    }
}