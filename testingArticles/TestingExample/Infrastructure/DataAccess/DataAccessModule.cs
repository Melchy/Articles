using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataAccess
{
    public class DataAccessModule
    {
        public static IServiceCollection Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                EnsureDatabaseCreated(configuration);
            }
            services.AddDbContext<ContosContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(serviceProvider.GetRequiredService<IOptions<SqlDatabaseOptions>>().Value.ConnectionString);
            });
            services.Configure<SqlDatabaseOptions>(configuration.GetSection("SqlDatabase"));
            return services;
        }


        private static void EnsureDatabaseCreated(IConfiguration configuration)
        {
            var options = new DbContextOptionsBuilder<ContosContext>()
                .UseSqlServer(configuration.GetSection("SqlDatabase")["connectionString"])
                .Options;
            var sensorsContext = new ContosContext(options);
            sensorsContext.Database.EnsureCreated();
        }

    }
}
