using System.Security.Claims;
using System.Text;
using JwtDotNet;
using JwtDotNet.Models;
using JwtDotNet.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<TokenService>();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", p => p.RequireRole("admin"));

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/login", (TokenService service)
    =>
{
    var user = new User(
        1,
        "teste teste",
        "xyz@teste",
        "https://teste/",
        "xyxz",
        new[] { "student", "premium" });

    return service.Create(user);
});

app.MapGet("/restrito", (ClaimsPrincipal user) => new
    {
        id = user.Id(),
        name = user.Name(),
        email = user.Email(),
        givenName = user.GivenName(),
        image = user.Image(),
    })
    .RequireAuthorization();

app.MapGet("/admin", () => "Você tem acesso!")
    .RequireAuthorization("admin");

app.Run();