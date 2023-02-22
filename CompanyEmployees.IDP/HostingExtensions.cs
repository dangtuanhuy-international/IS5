using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection;

namespace CompanyEmployees.IDP;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        var migrationAssembly = typeof(Program).Assembly.GetName().Name;

        builder.Services.AddIdentityServer(options =>
        {
            options.EmitStaticAudienceClaim = true;
        })
        .AddTestUsers(TestUsers.Users)
        .AddConfigurationStore(opt =>
        {
                opt.ConfigureDbContext = c => c.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"),
                sql => sql.MigrationsAssembly(migrationAssembly));
        })
        .AddOperationalStore(opt =>
        {
        opt.ConfigureDbContext = o => o.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"),
            sql => sql.MigrationsAssembly(migrationAssembly));
        });

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
