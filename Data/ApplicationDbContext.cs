using Microsoft.EntityFrameworkCore;
using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
  {
  }

  public DbSet<Center> Centers { get; set; }
  public DbSet<SewaType> SewaTypes { get; set; }
  public DbSet<SewaNominalRoll> SewaNominalRolls { get; set; }
  public DbSet<DailyActualCount> DailyActualCounts { get; set; }
  public DbSet<AllotedCount> AllotedCounts { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Center>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).ValueGeneratedOnAdd();
      entity.Property(e => e.CenterName).IsRequired();
      entity.Property(e => e.CenterType).IsRequired();
    });

    modelBuilder.Entity<SewaNominalRoll>(entity =>
    {
      entity.HasKey(e => new { e.NominalRollToken, e.SewaDate });
      entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    });

    modelBuilder.Entity<DailyActualCount>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).ValueGeneratedOnAdd();
      entity.Property(e => e.Date).IsRequired();
      entity.Property(e => e.CenterId).IsRequired();
      entity.Property(e => e.SewaTypeId).IsRequired();
      entity.Property(e => e.Count).IsRequired();
      entity.Property(e => e.NominalRollToken).IsRequired().HasMaxLength(100);

      entity.HasOne(e => e.Center)
        .WithMany()
        .HasForeignKey(e => e.CenterId)
        .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.SewaType)
        .WithMany()
        .HasForeignKey(e => e.SewaTypeId)
        .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<AllotedCount>(entity =>
    {
      entity.HasKey(e => new { e.WeekDay, e.CenterId, e.SewaTypeId });
      entity.Property(e => e.Count).IsRequired();

      entity.HasOne(e => e.Center)
        .WithMany()
        .HasForeignKey(e => e.CenterId)
        .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.SewaType)
        .WithMany()
        .HasForeignKey(e => e.SewaTypeId)
        .OnDelete(DeleteBehavior.Restrict);
    });
  }
}