using AwsLocalStack.Models;
using Microsoft.EntityFrameworkCore;

namespace AwsLocalStack.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }

        public DbSet<UserInfo> UserInfos { get; set; }
    }
}
