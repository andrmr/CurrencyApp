using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace UserApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasIndex(u => u.Name).IsUnique();

            builder.Entity<User>()
                .Property(u => u.Id)
                .UseIdentityColumn();

            builder.Entity<User>()
            .Property(u => u.Data)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                v => JsonSerializer.Deserialize<UserData>(v, new JsonSerializerOptions()));
        }
    }
}
