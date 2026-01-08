using Microsoft.EntityFrameworkCore;
using Toplanti.Models;

namespace Toplanti.Data;

public class ToplantiDbContext(DbContextOptions<ToplantiDbContext> options) : DbContext(options)
{
    public DbSet<Site> Sites { get; set; }
    public DbSet<UnitType> UnitTypes { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<MeetingAttendance> MeetingAttendances { get; set; }
    public DbSet<Proxy> Proxies { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Decision> Decisions { get; set; }
    public DbSet<AgendaItem> AgendaItems { get; set; }
    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasIndex(e => e.Number).IsUnique();
            entity.HasOne(e => e.UnitType)
                  .WithMany(ut => ut.Units)
                  .HasForeignKey(e => e.UnitTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Site)
                  .WithMany(s => s.Units)
                  .HasForeignKey(e => e.SiteId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MeetingAttendance>(entity =>
        {
            entity.HasOne(e => e.Meeting)
                  .WithMany(m => m.Attendances)
                  .HasForeignKey(e => e.MeetingId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Unit)
                  .WithMany(u => u.Attendances)
                  .HasForeignKey(e => e.UnitId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Proxy>(entity =>
        {
            entity.HasOne(e => e.GiverUnit)
                  .WithMany(u => u.ProxiesGiven)
                  .HasForeignKey(e => e.GiverUnitId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.ReceiverUnit)
                  .WithMany(u => u.ProxiesReceived)
                  .HasForeignKey(e => e.ReceiverUnitId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);
        });

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasOne(e => e.Unit)
                  .WithMany()
                  .HasForeignKey(e => e.UnitId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Meeting)
                  .WithMany()
                  .HasForeignKey(e => e.MeetingId)
                  .OnDelete(DeleteBehavior.NoAction);
            
            entity.HasOne(e => e.Decision)
                  .WithMany(d => d.Votes)
                  .HasForeignKey(e => e.DecisionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Decision>(entity =>
        {
            entity.HasOne(e => e.Meeting)
                  .WithMany(m => m.Decisions)
                  .HasForeignKey(e => e.MeetingId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<AgendaItem>(entity =>
        {
            entity.HasOne(e => e.Meeting)
                  .WithMany(m => m.AgendaItems)
                  .HasForeignKey(e => e.MeetingId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasOne(e => e.Meeting)
                  .WithMany(m => m.Documents)
                  .HasForeignKey(e => e.MeetingId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

