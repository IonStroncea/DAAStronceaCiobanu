
using AuthService.DAL;
using Microsoft.EntityFrameworkCore;
using AuthService.BL.Login;
using AuthService.BL.TokenGeneration;
using AuthService.BL;
using System.Net;

namespace DSNService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.Listen(IPAddress.Any, 5000);
                serverOptions.Listen(IPAddress.Any, 5001);
            });

            // Add services to the container.
            builder.Services.AddDbContext<AuthContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<ILogin, Login>();
            builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();
            builder.Services.AddScoped<LoginBL>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();


            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
