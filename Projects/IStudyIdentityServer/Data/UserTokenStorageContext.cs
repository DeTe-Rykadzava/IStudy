using Microsoft.EntityFrameworkCore;

namespace IStudyIdentityServer.Data;

public class UserTokenStorageContext : DbContext
{
    public UserTokenStorageContext(DbContextOptions<UserTokenStorageContext> options) : base(options)
    {
    }

    public DbSet<UserToken> UserTokens { get; set; }
}