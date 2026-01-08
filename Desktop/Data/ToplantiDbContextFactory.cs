using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Toplanti.Data;

public class ToplantiDbContextFactory : IDesignTimeDbContextFactory<ToplantiDbContext>
{
    public ToplantiDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\mssqllocaldb;Database=ToplantiDb;Trusted_Connection=true;MultipleActiveResultSets=true";

        var optionsBuilder = new DbContextOptionsBuilder<ToplantiDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        
        return new ToplantiDbContext(optionsBuilder.Options);
    }
}

