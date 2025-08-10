using OTPProject.Domain.Entities;

namespace OTPProject.Domain.Abstractions
{
    public interface IOtpCodeRepository
    {
        Task AddAsync(OtpCode code, CancellationToken ct);
        Task<OtpCode?> GetLatestActiveAsync(string phoneNumber, CancellationToken ct);
        Task UpdateAsync(OtpCode code, CancellationToken ct);
    }
}