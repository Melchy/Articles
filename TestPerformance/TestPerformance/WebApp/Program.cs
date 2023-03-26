using Microsoft.OpenApi.Models;

namespace WebApp;

public class Program
{
    public static void Main(
        string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebApp", Version = "v1"});
        });

        
        var app = builder.Build();
        
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MvcApplication v1"));
        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}