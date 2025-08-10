namespace OTPProject.Domain.Abstractions
{
    public interface ISmsSender
    {
         Task SendAsync(string phoneNumber, string message, CancellationToken ct = default);
    }
}