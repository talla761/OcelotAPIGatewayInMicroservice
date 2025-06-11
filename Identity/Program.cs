using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
     {
         var key = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Authentication:Key").Value!);
         string issuer = builder.Configuration["Authentication:Issuer"]!;
         string audience = builder.Configuration["Authentication:Audience"]!;

         options.RequireHttpsMetadata = false;
         options.SaveToken = true;
         options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = false,
             ValidateIssuerSigningKey = true,
             ValidIssuer = issuer,
             ValidAudience = audience,
             IssuerSigningKey = new SymmetricSecurityKey(key)
         };
     });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();
app.UseMiddleware<RestrictAccessMiddleware>();

app.MapControllers();

app.Run();
