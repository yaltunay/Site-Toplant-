using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Toplanti.Data;

public class ToplantiDbContextFactory : IDesignTimeDbContextFactory<ToplantiDbContext>
{
    public ToplantiDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ToplantiDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ToplantiDb;Trusted_Connection=true;MultipleActiveResultSets=true");
        
        return new ToplantiDbContext(optionsBuilder.Options);
    }
}

