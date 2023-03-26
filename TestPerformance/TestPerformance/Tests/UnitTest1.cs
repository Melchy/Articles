using System.Diagnostics;
using System.Net.Http.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using MvcApplication.Database;
using PerformanceTests;
using Respawn;
using Respawn.Graph;
using TestSupport.EfHelpers;

namespace Tests;

[SimpleJob(RunStrategy.ColdStart)]
public class Tests
{
    [Test]
    [Benchmark]
    public async Task CallMinimalEndpoint()
    {
        var webApplication = new WebApplicationFactory<WebApp.Program>();
        var httpClient = webApplication.CreateClient();
        var response = await httpClient.GetAsync("/");
        var result = await response.Content.ReadAsStringAsync();
    }

    [Test]
    [Benchmark]
    public async Task CallMvcEndpoint()
    {
        var webApplication = new WebApplicationFactory<MvcApplication.Program>();
        var httpClient = webApplication.CreateClient();
        var response = await httpClient.GetAsync("/");
        var result = await response.Content.ReadAsStringAsync();
    }

    [Test]
    [Benchmark]
    public async Task CreateAndDeleteDatabaseUsingEf()
    {
        var dbContextOptions = DbToolsMssql.GetOptions<MvcDbContext>(Guid.NewGuid().ToString());
        var dbContext = new MvcDbContext(dbContextOptions);
        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.Database.EnsureDeletedAsync();
    }
    
    [Test]
    [Benchmark]
    public async Task CreateAndDeleteDatabaseUsingEfTestSupport()
    {
        var dbContextOptions = DbToolsMssql.GetOptions<MvcDbContext>(Guid.NewGuid().ToString());
        var dbContext = new MvcDbContext(dbContextOptions);
        dbContext.Database.EnsureClean();
        await dbContext.Database.EnsureDeletedAsync();
    }

    [Test]
    [Benchmark]
    public async Task CreateDatabaseUsingAdoMigrateAndDelete()
    {
        var databaseName = nameof(CreateDatabaseUsingAdoMigrateAndDelete);
        var connectionStringWithoutDb = DbToolsMssql.GetConnectionStringWithoutDatabaseSelected();
        var connectionString = DbToolsMssql.GetConnectionString(databaseName);

        
        await using var connection = new SqlConnection(connectionStringWithoutDb);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE {databaseName}";
        await command.ExecuteNonQueryAsync();

        await using (var connectionWithDb = new SqlConnection(connectionString))
        {
            await connectionWithDb.OpenAsync();
            var migrationCommand = $"""
                                    create table Carts
                                    (
                                        Id     bigint not null
                                            constraint PK_Carts
                                                primary key,
                                        Amount int
                                    );

                                    create table Items
                                    (
                                        Id                bigint not null
                                            constraint PK_Items
                                                primary key,
                                        Name              nvarchar(255),
                                        Description       nvarchar(255),
                                        ShortDescription  nvarchar(255),
                                        AmountInWereHouse bigint
                                    );

                                    create table CartItems
                                    (
                                        Id      bigint not null
                                            constraint PK_CartItems
                                                primary key,
                                        ItemsId bigint
                                            constraint FK_CartItems_ItemsId_Items_Id
                                                references Items,
                                        CartsId bigint
                                            constraint FK_CartItems_CartsId_Carts_Id
                                                references Carts
                                    );

                                    create table Users
                                    (
                                        Id        bigint not null
                                            constraint PK_Users
                                                primary key,
                                        Name      nvarchar(255),
                                        Surname   nvarchar(255),
                                        Email     nvarchar(255),
                                        Password  nvarchar(255),
                                        BirthDate datetimeoffset,
                                        CartId    bigint
                                            constraint FK_Users_CartId_Carts_Id
                                                references Carts
                                    );
                                    """;
            var migrateSchemaCommand = connectionWithDb.CreateCommand();
            migrateSchemaCommand.CommandText = migrationCommand;
            await migrateSchemaCommand.ExecuteNonQueryAsync();
        }


        var deleteCommand = connection.CreateCommand();
        SqlConnection.ClearAllPools();
        deleteCommand.CommandText = $"DROP DATABASE {databaseName}";
        await deleteCommand.ExecuteNonQueryAsync();
    }
    
    [Test]
    [Benchmark]
    public async Task CreateDatabaseUsingAdoMigrateUsingFluentMigratorAndDelete()
    {
        var databaseName = nameof(CreateDatabaseUsingAdoMigrateUsingFluentMigratorAndDelete);
        var connectionStringWithoutDb = DbToolsMssql.GetConnectionStringWithoutDatabaseSelected();
        var connectionString = DbToolsMssql.GetConnectionString(databaseName);

        
        await using var connection = new SqlConnection(connectionStringWithoutDb);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE {databaseName}";
        await command.ExecuteNonQueryAsync();
        
        MigrationsRunner.Run(connectionString);
        
        var deleteCommand = connection.CreateCommand();
        SqlConnection.ClearAllPools();
        deleteCommand.CommandText = $"DROP DATABASE {databaseName}";
        await deleteCommand.ExecuteNonQueryAsync();
    }

    [Test]
    [Benchmark]
    public async Task MigrateExistingDatabaseAndResetWithRespawn()
    {
        var databaseName = nameof(MigrateExistingDatabaseAndResetWithRespawn);
        var connectionString = DbToolsMssql.GetConnectionString(databaseName);
        var connectionStringWithoutDb = DbToolsMssql.GetConnectionStringWithoutDatabaseSelected();
        
        try
        {
            MigrationsRunner.Run(connectionString);
        }
        catch (Exception e)
        {
            await using var connection = new SqlConnection(connectionStringWithoutDb);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE {databaseName}";
            await command.ExecuteNonQueryAsync();
            SqlConnection.ClearAllPools();

            MigrationsRunner.Run(connectionString);
        }

        var respawner = await Respawner.CreateAsync(connectionString, new RespawnerOptions()
        {
            TablesToIgnore = new Table[]{"VersionInfo"},
        });
        await respawner.ResetAsync(connectionString);
    }
    
    [Test]
    [Benchmark]
    public async Task RestartDatabaseUsingTestSupportWithoutDelete()
    {
        var databaseName = nameof(RestartDatabaseUsingTestSupportWithoutDelete);
        var connectionString = DbToolsMssql.GetConnectionString(databaseName);
        var dbContextOptions = DbToolsMssql.GetOptions<MvcDbContext>(connectionString);
        var dbContext = new MvcDbContext(dbContextOptions);
        
        dbContext.Database.EnsureClean();
    }
    
    [Test]
    [Benchmark]
    public async Task CreatedExecuteQueryAndDelete()
    {
        var dbContextOptions = DbToolsMssql.GetOptions<MvcDbContext>(Guid.NewGuid().ToString());
        var dbContext = new MvcDbContext(dbContextOptions);
        await dbContext.Database.EnsureCreatedAsync();

        var user = new User();
        dbContext.Users.Add(user);
        dbContext.Carts.Add(new Cart()
        {
            User = user,
        });
        await dbContext.SaveChangesAsync();
        _ = await dbContext.Carts.ToListAsync();

        await dbContext.Database.EnsureDeletedAsync();
    }
    
    [Test]
    [Benchmark]
    public async Task WebApplicationFactoryWithMigrationAndCall()
    {
        var databaseName = Guid.NewGuid().ToString();
        var dbContextOptions = DbToolsMssql.GetOptions<MvcDbContext>(databaseName);
        var dbContext = new MvcDbContext(dbContextOptions);
        await dbContext.Database.EnsureCreatedAsync();

        var user = new User();
        dbContext.Users.Add(user);
        dbContext.Carts.Add(new Cart()
        {
            User = user,
        });
        await dbContext.SaveChangesAsync();
        
        
        var webApplication =
            new WebApplicationFactory<MvcApplication.Program>()
               .WithWebHostBuilder(x => x.ConfigureServices(services =>
                {
                    services.AddDbContext<MvcDbContext>(x => x.UseSqlServer(DbToolsMssql.GetConnectionString(databaseName)));
                }));
        
        var httpClient = webApplication.CreateClient();
        var response = await httpClient.GetFromJsonAsync<IEnumerable<Cart>>("/carts");
        
        await dbContext.Database.EnsureDeletedAsync();
    }
    
    [Test]
    [Benchmark]
    public async Task WebApplicationFactoryWithFluentMigratorAndCall()
    {
        var databaseName = nameof(WebApplicationFactoryWithFluentMigratorAndCall);
        var connectionString = DbToolsMssql.GetConnectionString(databaseName);
        var connectionStringWithoutDb = DbToolsMssql.GetConnectionStringWithoutDatabaseSelected();
        var dbContextOptions = DbToolsMssql.GetOptions<MvcDbContext>(databaseName);
        var dbContext = new MvcDbContext(dbContextOptions);
        try
        {
            MigrationsRunner.Run(connectionString);
        }
        catch (Exception e)
        {
            await using var connection = new SqlConnection(connectionStringWithoutDb);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE {databaseName}";
            await command.ExecuteNonQueryAsync();
            SqlConnection.ClearAllPools();

            MigrationsRunner.Run(connectionString);
        }

        var respawner = await Respawner.CreateAsync(connectionString, new RespawnerOptions()
        {
            TablesToIgnore = new Table[]{"VersionInfo"},
        });

        var user = new User()
        {
            Id = 1,
        };
        dbContext.Users.Add(user);
        dbContext.Carts.Add(new Cart()
        {
            Id = 1,
            User = user,
        });
        await dbContext.SaveChangesAsync();
        
        
        var webApplication =
            new WebApplicationFactory<MvcApplication.Program>()
               .WithWebHostBuilder(x => x.ConfigureServices(services =>
                {
                    services.AddDbContext<MvcDbContext>(x =>
                    {
                        x.UseSqlServer(connectionString);
                    });
                }));
        
        var httpClient = webApplication.CreateClient();
        var response = await httpClient.GetFromJsonAsync<IEnumerable<Cart>>("/carts");
        
        await respawner.ResetAsync(connectionString);
    }
    
    [Test]
    [Benchmark]
    public async Task FluentMigratorAndDbCallWithRespawn()
    {
        var databaseName = nameof(FluentMigratorAndDbCallWithRespawn);
        var connectionString = DbToolsMssql.GetConnectionString(databaseName);
        var connectionStringWithoutDb = DbToolsMssql.GetConnectionStringWithoutDatabaseSelected();
        var dbContextOptions = DbToolsMssql.GetOptions<MvcDbContext>(databaseName);
        var dbContext = new MvcDbContext(dbContextOptions);
        try
        {
            MigrationsRunner.Run(connectionString);
        }
        catch (Exception e)
        {
            await using var connection = new SqlConnection(connectionStringWithoutDb);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE {databaseName}";
            await command.ExecuteNonQueryAsync();
            SqlConnection.ClearAllPools();

            MigrationsRunner.Run(connectionString);
        }

        var respawner = await Respawner.CreateAsync(connectionString, new RespawnerOptions()
        {
            TablesToIgnore = new Table[]{"VersionInfo"},
        });

        var user = new User()
        {
            Id = 1,
        };
        dbContext.Users.Add(user);
        dbContext.Carts.Add(new Cart()
        {
            Id = 1,
            User = user,
        });
        await dbContext.SaveChangesAsync();

        _ = await dbContext.Carts.ToListAsync();

        await respawner.ResetAsync(connectionString);
    }

    private MvcDbContext _dbContext;
    
    [GlobalSetup(Targets = new[] {nameof(EfQueryPerformance), nameof(EfMultipleQueryPerformance), nameof(EfMultipleQueryPerformanceWithTransaction)})]
    public async Task GlobalSetup()
    {
        var dbContextOptions = DbToolsMssql.GetOptions<MvcDbContext>(nameof(EfQueryPerformance));
        _dbContext = new MvcDbContext(dbContextOptions);
        await _dbContext.Database.EnsureCreatedAsync();
    }
    
    [Test]
    [Benchmark]
    public async Task EfQueryPerformance()
    {
        var user = new User()
        {
        };
        _dbContext.Users.Add(user);
        _dbContext.Carts.Add(new Cart()
        {
            User = user,
        });
        await _dbContext.SaveChangesAsync();
    }
    
    [Test]
    [Benchmark]
    public async Task EfMultipleQueryPerformance()
    {
        var user = new User()
        {
        };
        _dbContext.Users.Add(user);
        _dbContext.Carts.Add(new Cart()
        {
            User = user,
        });
        await _dbContext.SaveChangesAsync();

        var user1 = new User()
        {
        };
        _dbContext.Users.Add(user1);
        _dbContext.Carts.Add(new Cart()
        {
            User = user1,
        });
        await _dbContext.SaveChangesAsync();

        var user2 = new User()
        {
        };
        _dbContext.Users.Add(user2);
        _dbContext.Carts.Add(new Cart()
        {
            User = user2,
        });
        await _dbContext.SaveChangesAsync();

        var user3 = new User()
        {
        };
        _dbContext.Users.Add(user3);
        _dbContext.Carts.Add(new Cart()
        {
            User = user3,
        });
        await _dbContext.SaveChangesAsync();

        var user4 = new User()
        {
        };
        _dbContext.Users.Add(user4);
        _dbContext.Carts.Add(new Cart()
        {
            User = user4,
        });
        await _dbContext.SaveChangesAsync();
    }
    
    [Test]
    [Benchmark]
    public async Task EfMultipleQueryPerformanceWithTransaction()
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        var user = new User()
        {
        };
        _dbContext.Users.Add(user);
        _dbContext.Carts.Add(new Cart()
        {
            User = user,
        });
        await _dbContext.SaveChangesAsync();

        var user1 = new User()
        {
        };
        _dbContext.Users.Add(user1);
        _dbContext.Carts.Add(new Cart()
        {
            User = user1,
        });
        await _dbContext.SaveChangesAsync();

        var user2 = new User()
        {
        };
        _dbContext.Users.Add(user2);
        _dbContext.Carts.Add(new Cart()
        {
            User = user2,
        });
        await _dbContext.SaveChangesAsync();

        var user3 = new User()
        {
        };
        _dbContext.Users.Add(user3);
        _dbContext.Carts.Add(new Cart()
        {
            User = user3,
        });
        await _dbContext.SaveChangesAsync();

        var user4 = new User()
        {
        };
        _dbContext.Users.Add(user4);
        _dbContext.Carts.Add(new Cart()
        {
            User = user4,
        });
        await _dbContext.SaveChangesAsync();
        
        await transaction.RollbackAsync();
    }
    
    [GlobalCleanup(Targets = new[] {nameof(EfQueryPerformance), nameof(EfMultipleQueryPerformance), nameof(EfMultipleQueryPerformanceWithTransaction)})]
    public async Task GlobalCleanup()
    {
        await _dbContext.Database.EnsureDeletedAsync();
    }
}
// TODO https://stackoverflow.com/a/19388450/5324847
// TODO test ef core migrace
// TODO test query
// TODO test 

// TODO
// Async s db pomoci migraci
// 104 testu
//
//
//     LevelOfParalelism 20
//
// 16267
// 10992
// 10992
// 12088
// 12366
//
//
// Level of paralelism 8 (nenastaveno)
// 13676
// 15000
// 16017
// 13548
//
//
// Level of paralelism 1000
// 11289
// 11066
//
//
//
//
// Seriove
//
// 32592
// 36167
// 34000
//
//
//
// Jeden test - 150ms - 200sec