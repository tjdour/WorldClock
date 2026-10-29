using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using WorldClock.Core;
using WorldClock.Maui.Data;
using WorldClock.Maui.Services;

namespace WorldClock.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Configuration.AddUserSecrets(typeof(App).Assembly, optional: true);

            //ef core sqlite database path
            string databasePath = Path.Combine(FileSystem.AppDataDirectory,"worldclock.db");

            builder.Services.AddDbContext<WorldClockDbContext>(options =>
                options.UseSqlite($"Data Source={databasePath}"));

            builder.Services.AddSingleton(
                FoundryOptions.FromConfiguration(builder.Configuration));
            builder.Services.AddSingleton<FoundryChatClientFactory>();
            builder.Services.AddSingleton<WorldClockService>();
            builder.Services.AddSingleton<TimeZoneTools>();
            builder.Services.AddSingleton<AiAssistantService>();
            builder.Services.AddSingleton<AssistantPage>();
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            MauiApp app = builder.Build();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                WorldClockDbContext dbContext =
                    scope.ServiceProvider.GetRequiredService<WorldClockDbContext>();

                dbContext.Database.EnsureCreated();
            }

            return app;
        }
    }
}
