using AspNetCore.Authentication.Basic;
using Lab8.Data;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IFoxesRepository, FoxesRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddAuthentication(BasicDefaults.AuthenticationScheme)
    .AddBasic(options =>
    {
        options.Realm = "Fox API";
        options.Events = new BasicEvents
        {
            OnValidateCredentials = async (context) =>
            {
                var user = context.Username;

                var isValid = user == "user" && context.Password == "password"; ;
                if (isValid)
                {
                    context.Response.Headers.Add(
                        "ValidationCustomHeader",
                        "From OnValidateCredentials"
                    );
                    var claims = new[]
                    {
                        new Claim(
                          ClaimTypes.NameIdentifier,
                          context.Username,
                          ClaimValueTypes.String,
                          context.Options.ClaimsIssuer
                        ),
                        new Claim(
                            ClaimTypes.Name,
                            context.Username,
                            ClaimValueTypes.String,
                            context.Options.ClaimsIssuer
                        )
                    };
                    context.Principal = new ClaimsPrincipal(
                        new ClaimsIdentity(claims, context.Scheme.Name));
                    context.Success();
                }
                else
                {
                    context.NoResult();
                }
            }
        };
    });
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Fox api", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseFileServer();

app.MapControllers();

app.Run();
