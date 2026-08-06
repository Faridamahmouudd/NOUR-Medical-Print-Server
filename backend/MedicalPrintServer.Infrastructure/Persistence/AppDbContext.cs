using MedicalPrintServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalPrintServer.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Printer> Printers => Set<Printer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Printer>(entity =>
        {
            entity.ToTable("Printers");

            entity.HasKey(printer => printer.Id);

            entity.Property(printer => printer.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(printer => printer.WindowsPrinterName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(printer => printer.IpAddress)
                .HasMaxLength(50);

            entity.Property(printer => printer.AeTitle)
                .HasMaxLength(16);

            entity.Property(printer => printer.DefaultPaperSize)
                .IsRequired()
                .HasMaxLength(30);
        });
    }
}