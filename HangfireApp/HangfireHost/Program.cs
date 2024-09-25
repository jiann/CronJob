using System.Reflection;
using Hangfire;
using Hangfire.Console;
using HangfireApp.HangfireService;
using Microsoft.OpenApi.Models;

namespace HangfireHost
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
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Hangfire API",
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            });


            builder.Services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireStorage"));
                x.UseConsole();
            });

            builder.Services.AddHangfireServer(options =>
            {
                // 檢查job的時間 (預設 15秒)
                options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
            });

            builder.Services.AddScoped<IHangfireTestJobService, HangfireTestJobService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseHangfireDashboard();

            app.MapControllers();

            app.Run();
        }
    }
}