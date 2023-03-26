using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System.Diagnostics.CodeAnalysis;

namespace Migrations;

public static class MigrationsRunner
{
    public static void Run(
        string connectionString)
    {
        using var serviceProvider = CreateServices(connectionString);
        using var scope = serviceProvider.CreateScope();
        UpdateDatabase(scope.ServiceProvider);
    }

    private static ServiceProvider CreateServices(
        string connectionString)
    {
        return new ServiceCollection()
            // Add common FluentMigrator services
           .AddFluentMigratorCore()
           .ConfigureRunner(rb => rb
                // Add SQLite support to FluentMigrator
               .AddSqlServer()
                // Set the connection string
               .WithGlobalConnectionString(connectionString)
                // Define the assembly containing the migrations
               .ScanIn(typeof(InitialMigrations).Assembly).For.Migrations())
            // Build the service provider
           .BuildServiceProvider(false);
    }
    
    private static void UpdateDatabase(
        IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateUp();
    }
}
