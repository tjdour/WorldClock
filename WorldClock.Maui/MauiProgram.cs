using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WorldClock.Maui.Data;

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

            //ef core sqlite database path
            string databasePath = Path.Combine(FileSystem.AppDataDirectory,"worldclock.db");

            builder.Services.AddDbContext<WorldClockDbContext>(options =>
                options.UseSqlite($"Data Source={databasePath}"));

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
