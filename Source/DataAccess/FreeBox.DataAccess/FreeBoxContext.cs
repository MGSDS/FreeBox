using FreeBox.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreeBox.DataAccess;

public class FreeBoxContext : DbContext
{
    public FreeBoxContext(DbContextOptions<FreeBoxContext> options)
    : base(options)
    {
        //Database.EnsureDeleted();
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