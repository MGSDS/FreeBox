using FreeBox.Server.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreeBox.Server.DataAccess;

public class BusinessContext : DbContext
{
    public BusinessContext(DbContextOptions<BusinessContext> options)
    : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }
    
    public DbSet<ClientModel> Clients { get; set; }
    public DbSet<FileModel> Files { get; set; }
    public DbSet<Blob> Blobs { get; set; }
}