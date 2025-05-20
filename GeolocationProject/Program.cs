
using AutoMapper;
using Geolocation.Core.GeoLocationConfig;
using Geolocation.Core.IRepo;
using Geolocation.Core.Models;
using Geolocation.Services;
using Geolocation.Services.Services;
using Geolocation.Services.Services.Interface;
using GeolocationProject.Profiles;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

namespace GeolocationProject
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Suppress noisy logs
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);
            //Cponfigure Hangfire for Background jobs
            builder.Services.AddHangfire(config =>
            config.UseMemoryStorage());
            builder.Services.AddHangfireServer();

            builder.Host.UseSerilog();


            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(typeof(MapProfile).Assembly);
            builder.Services.AddHttpClient<IGeoLocationService, GeoLocationService>((provider, client) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["GeoLocationApi:BaseUrl"]);
            });
            builder.Services.AddSingleton<IBlockedCountryRepo, BlockedCountryRepo>();
            builder.Services.AddSingleton<IGeoLocationService, GeoLocationService>();
            builder.Services.AddSingleton<IBlockedCountryService, BlockedCountryService>();
            builder.Services.AddSingleton<ILoggingService, LoggingService>();
            builder.Services.AddScoped<IIPCheckService , IPCheckService>();
            builder.Services.AddSingleton<ITemporalBlockService, TemporalBlockService>();
            builder.Services.Configure<CountryCodeOptions>(builder.Configuration);
            builder.Services.Configure<GeoLocationApiConfig>(builder.Configuration.GetSection("GeoLocationApi"));
            builder.Services.AddHttpClient<IGeoLocationService, GeoLocationService>((provider, client) =>
            {
                var config = provider.GetRequiredService<IOptions<GeoLocationApiConfig>>().Value;
                client.BaseAddress = new Uri(config.BaseUrl);
            });

            


            // Swagger configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Geolocation API", Version = "v1" });
            });



            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHangfireDashboard("/HangFireDash");
            RecurringJob.AddOrUpdate<IBlockedCountryRepo>(
                "RemoveExpiredTemporalBlocksJob",
                repo => repo.RemoveeExpiredTemporalBlocks(),
                "*/5 * * * *"); //=>> this Way in Write Time Called Cron Expression (first astrek for min and secound one for hours and third for days)
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
