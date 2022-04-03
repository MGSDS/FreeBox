using FreeBox.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreeBox.DataAccess;

public class FreeBoxContext : DbContext
{
#pragma warning disable CS8618
    public FreeBoxContext(DbContextOptions<FreeBoxContext> options)
#pragma warning restore CS8618
        : base(options)
    {
        // Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<FileContainer> Files { get; set; }
    public DbSet<ContainerData> Blobs { get; set; }

    public DbSet<ContainerInfo> FileInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .Entity<ContainerData>()
            .Property(x => x.Content)
            .HasConversion(
                x => x.ToArray(),
                x => new MemoryStream(x));
}