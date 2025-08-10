using OTPProject.Domain.Entities;

namespace OTPProject.Domain.Abstractions
{
    public interface IJwtTokenGenerator
    {
         string Generate(ApplicationUser user, IEnumerable<string> roles);
    }
}