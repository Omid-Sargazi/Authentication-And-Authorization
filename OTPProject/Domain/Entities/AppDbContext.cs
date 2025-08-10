using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OTPProject.Domain.Entities
{
   public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options) // 👈 حتماً این
{
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<OtpCode>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.PhoneNumber).IsRequired();
            e.Property(x => x.CodeHash).IsRequired();
            e.HasIndex(x => new { x.PhoneNumber, x.Used, x.ExpiresAtUtc });
        });
    }
}
}