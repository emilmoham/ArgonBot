using ArgonBot.HostedServices;
using ArgonBot.Services;
using Microsoft.EntityFrameworkCore;
using TwitchLib.EventSub.Websockets.Extensions;

namespace ArgonBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            string? connectionString = builder.Configuration.GetConnectionString("ApplicationDbContext");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            builder.Services.AddLogging();
            builder.Services.AddTwitchLibEventSubWebsockets();

            builder.Services.AddHostedService<WebsocketHostedService>();

            builder.Services.AddSingleton<TwitchApiService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
