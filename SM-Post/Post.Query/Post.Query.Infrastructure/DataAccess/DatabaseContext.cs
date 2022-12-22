using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.DataAccess;

public class DatabaseContext:DbContext
{
    public DatabaseContext(DbContextOptions options):base(options)
    {
        
    }

    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
}


public class DatabaseContextFactory
{
    private readonly Action<DbContextOptionsBuilder> _configureDbContext;

    public DatabaseContextFactory(Action<DbContextOptionsBuilder>configureDbContext)
    {
        _configureDbContext = configureDbContext;
    }

    public DatabaseContext CreateDbContext()
    {
        DbContextOptionsBuilder<DatabaseContext> optionsBuilder = new();
        _configureDbContext(optionsBuilder);
        return new DatabaseContext(optionsBuilder.Options);
    }
}