using FreeBox.Server.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using File = FreeBox.Server.DataAccess.Entities.File;

namespace FreeBox.Server.DataAccess;

public class FreeBoxContext : DbContext
{
    public FreeBoxContext(DbContextOptions<FreeBoxContext> options)
    : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<Blob> Blobs { get; set; }
}