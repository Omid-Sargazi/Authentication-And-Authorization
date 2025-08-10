using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OTPProject.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=otp_simple.db"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
{
    // هنوز پسورد نداریم، ولی Identity باید مقداردهی شود
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 1;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ------------------------- Options -------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}   

app.MapPost("/auth/request-otp", async (
    string phoneNumber,
    AppDbContext db,
    ILoggerFactory lf) =>
{
    var logger = lf.CreateLogger("SMS");
    var phone = Normalize(phoneNumber);

    // تولید کد ۶ رقمی
    var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    var codeHash = Hash(code);

    var otp = new OtpCode
    {
        PhoneNumber = phone,
        CodeHash = codeHash,
        CreatedAtUtc = DateTimeOffset.UtcNow,
        ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(2),
        Used = false
    };

    db.OtpCodes.Add(otp);
    await db.SaveChangesAsync();

    // DEV: به‌جای ارسال SMS واقعی
    logger.LogInformation("DEV SMS to {Phone}: code = {Code}", phone, code);

    return Results.Accepted();
})
.WithOpenApi(op =>
{
    op.Summary = "درخواست کد ورود (OTP)";
    op.Parameters[0].Description = "شماره موبایل (ترجیحاً E.164 مثل +352...)";
    return op;
});

app.MapPost("/auth/verify-otp", async (
    string phoneNumber,
    string code,
    AppDbContext db,
    UserManager<ApplicationUser> userManager) =>
{
    var phone = Normalize(phoneNumber);
    var now = DateTimeOffset.UtcNow;

   var candidates = db.OtpCodes
    .Where(x => x.PhoneNumber == phone && !x.Used)
    .AsEnumerable() // اینجا IEnumerable برمی‌گردونه، نه IQueryable
    .OrderByDescending(x => x.CreatedAtUtc)
    .ToList(); 

        var otp = candidates.FirstOrDefault(x => x.ExpiresAtUtc >= now);

  

    if (otp is null)
        return Results.BadRequest(new { error = "کد فعال موجود نیست یا منقضی شده." });

    if (!StringEquals(otp.CodeHash, Hash(code)))
    {
        otp.AttemptCount++;
        await db.SaveChangesAsync();
        return Results.BadRequest(new { error = "کد نامعتبر است." });
    }

    otp.Used = true;
    await db.SaveChangesAsync();

    // اگر کاربر وجود نداشت، بساز
    var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
    if (user is null)
    {
        user = new ApplicationUser
        {
            UserName = phone,
            PhoneNumber = phone,
            PhoneNumberConfirmed = true
        };
        var create = await userManager.CreateAsync(user);
        if (!create.Succeeded)
            return Results.BadRequest(new { error = string.Join(";", create.Errors.Select(e => e.Description)) });
    }

    return Results.Ok(new { userId = user.Id, phoneNumber = user.PhoneNumber });
})
.WithOpenApi(op =>
{
    op.Summary = "تأیید کد ورود";
    return op;
});

app.Run();

static string Normalize(string phone)
    => phone.Replace(" ", "").Replace("-", "").Trim();

static string Hash(string input)
{
    using var sha = SHA256.Create();
    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
    return Convert.ToHexString(bytes);
}

static bool StringEquals(string a, string b)
    => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

