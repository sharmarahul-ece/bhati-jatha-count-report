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
  }
}