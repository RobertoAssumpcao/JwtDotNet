using System.Security.Claims;
using JwtDotNet.Models;
using JwtDotNet.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<TokenService>();
builder.Services.AddAuthentication();
var app = builder.Build();
app.UseAuthentication();

app.MapGet("/", (TokenService service) =>
{
    var user = new User(
        1,
        "teste",
        "xyz@teste",
        "https://teste/",
        "xyxz",
        new[] { "student", "premium" });

    return service.Create(user);
});

app.Run();