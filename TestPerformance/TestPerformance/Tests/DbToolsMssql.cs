using Microsoft.EntityFrameworkCore;

namespace PerformanceTests;

public static class DbToolsMssql
{
    public static DbContextOptions<T> GetOptions<T>(
        string databaseName)
        where T : DbContext
    {
        var options = new DbContextOptionsBuilder<T>()
           .UseSqlServer(GetConnectionString(databaseName))
           .Options;
        return options;
    }

    public static string GetConnectionString(
        string databaseName)
    {
        return $"{GetConnectionStringWithoutDatabaseSelected()};Database={databaseName}";
    }

    public static string GetConnectionStringWithoutDatabaseSelected()
    {
        return "Server=(localdb)\\mssqllocaldb;Trusted_Connection=True;MultipleActiveResultSets=true";
    }
}