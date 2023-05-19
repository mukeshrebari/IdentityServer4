using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework;

namespace ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=localhost,1433;Database=IdentityServer4;Persist Security Info=True;User ID=sa;Password=Mukesh@123;";

            var services = new ServiceCollection();
            services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
            services.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);

                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = false;
                options.TokenCleanupInterval = 5; // interval in seconds, short for testing
            });

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var svc = scope.ServiceProvider.GetRequiredService<TokenCleanupService>();
                svc.RemoveExpiredGrantsAsync().GetAwaiter().GetResult();
            }
        }
    }
}
