using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using ControllerCaller;
using ControllerCaller.FluentAssertionActionResultExtensions;
using FluentAssertions;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TestingExample;
using TestingExample.Controllers;
using TestingExample.Controllers.Course;

namespace Tests
{
    public class Tests
    {
        private Task<WebApp> CreateDefaultApplication()
        {
            return new ApplicationBuilder().Start();
        }
    }
    
    public sealed class WebApp : IDisposable
    {
        private readonly Func<ControllerBuilder> _controllerBuilderFactory;
        private readonly WebAppFactory _webAppFactory;

        public CourseController MenuAdminController => CreateControllerCaller<CourseController>();

        public IServiceProvider ServiceProvider => _webAppFactory.Services;

        public WebApp(
            Func<ControllerBuilder> controllerBuilderFactory,
            WebAppFactory webAppFactory)
        {
            _controllerBuilderFactory = controllerBuilderFactory;
            _webAppFactory = webAppFactory;
        }

        private TController CreateControllerCaller<TController>() where TController : ControllerBase
        {
            return _controllerBuilderFactory().Build<TController>();
        }

        public void Dispose()
        {
            _webAppFactory.Dispose();
        }
    }
    
    public class ApplicationBuilder
    {
        private readonly List<Action<IServiceCollection>> _serviceBuilders = new List<Action<IServiceCollection>>();

        public ApplicationBuilder WithServices(Action<IServiceCollection> serviceBuilder)
        {
            _serviceBuilders.Add(serviceBuilder);

            return this;
        }

        public async Task<WebApp> Start()
        {
            void ConfigurationBuilder(IConfigurationBuilder builder)
            {
                // builder.AddJsonStream(new FileStream("shopSettings.json", FileMode.Open, FileAccess.Read));
                // builder.AddJsonStream(new FileStream("configuration.json", FileMode.Open, FileAccess.Read));
                // builder.AddInMemoryCollection(new Dictionary<string, string> {
                //     { "Custom:Setting", "--empty--" },
                // });
                builder.AddEnvironmentVariables();
            }

            void ServiceRegistration(
                IServiceCollection services)
            {
                foreach (var serviceBuilder in _serviceBuilders)
                {
                    serviceBuilder(services);
                }
            }

            var dbContextProvider = await DatabaseTools.CreateContextProvider();

            var webAppFactory = new WebAppFactory(
                ConfigurationBuilder, ServiceRegistration, dbContextProvider);

            var client = webAppFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var linkGenerator = webAppFactory.Services.GetService<LinkGenerator>();
            var configuration = webAppFactory.Services.GetService<IConfiguration>();

            return new WebApp(
                () => new ControllerBuilder(client, linkGenerator),
                webAppFactory);
        }
    }
    
    
    public static class DatabaseTools
    {
        public static async Task<DbContextProvider> CreateContextProvider()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            await connection.OpenAsync();

            var contextProvider = new DbContextProvider(connection);

            using var context = contextProvider.CreateContext();
            await CreateDatabase(context);

            return contextProvider;
        }

        private static async Task CreateDatabase(
            ContosContext menuContext)
        {
            try
            {
                var menuDatabaseCreator = menuContext.Database.GetService<IRelationalDatabaseCreator>();

                if (await menuDatabaseCreator.ExistsAsync())
                {
                    await menuDatabaseCreator.DeleteAsync();
                }

                await menuDatabaseCreator.CreateAsync();
                await menuDatabaseCreator.CreateTablesAsync();
            }
            catch (PlatformNotSupportedException)
            {
                throw new PlatformNotSupportedException("It looks like your platform does not support MSSQL LocalDB.");
            }
        }
    }
    
    
    public class WebAppFactory : WebApplicationFactory<Startup>
    {
        private bool _disposed;
        private readonly Action<IConfigurationBuilder> _configurationBuilder;
        private readonly Action<IServiceCollection> _serviceRegistration;
        private readonly DbContextProvider _dbContextProvider;

        public WebAppFactory(
            Action<IConfigurationBuilder> configurationBuilder,
            Action<IServiceCollection> serviceRegistration,
            DbContextProvider dbContextProvider)
        {
            _configurationBuilder = configurationBuilder;
            _serviceRegistration = serviceRegistration;
            _dbContextProvider = dbContextProvider;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = base.CreateHostBuilder();
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders(); // Remove loggers
            });

            builder.UseEnvironment(Environments.Production);
            builder.ConfigureAppConfiguration((hostBuilderContext, conf) => _configurationBuilder(conf));

            builder.ConfigureServices(services =>
            {
                services.AddTransient(_ => _dbContextProvider.CreateContext());
                _serviceRegistration(services);
            });

            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Don't run IHostedServices when running as a test
            builder.ConfigureTestServices((services) =>
            {
                services.RemoveAll(typeof(IHostedService));
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _dbContextProvider.Dispose();
                base.Dispose(disposing);
            }

            _disposed = true;
        }
    }
    
    public class DbContextProvider : IDisposable 
    {
        private readonly DbConnection _connection;

        public DbContextProvider(DbConnection connection)
        {
            _connection = connection;
        }

        public ContosContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ContosContext>()
                .UseSqlite(_connection)
                .Options;

            return new ContosContext(options);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _connection.Dispose();
        }
    }
}
