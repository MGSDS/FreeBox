using FreeBox.Server.DataAccess.DatabaseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using File = FreeBox.Server.DataAccess.DatabaseEntities.File;

namespace FreeBox.Server.DataAccess;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options)
    : base(options)
    {
    }
    
    public DbSet<Client> Clients { get; set; }
    public DbSet<File> Files { get; set; }
}