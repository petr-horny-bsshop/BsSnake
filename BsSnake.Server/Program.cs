using Radzen;

namespace BsSnake.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor(options =>
            {
                options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(15); // výchozí jsou 3 minuty
                options.DisconnectedCircuitMaxRetained = 100; // výchozí je 100
                options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1); // výchozí je 1 minuta
                options.DetailedErrors = true;
            });
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();
            builder.WebHost.UseKestrel();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            for (var i = 0; i < 10; i++)
            {
                SnakeServer.Map(i, app);
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}