using Microsoft.EntityFrameworkCore;
using Rpg.Models;

namespace Rpg.DbContexts
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> opts) : base(opts)
        {
        }

        public async Task<User?> FindUserAsync(string inName)
        {
            return await UserSet.Where(each => each.Name == inName).FirstOrDefaultAsync();
        }

        public async Task<User?> CreateUserAsync(string inName)
        {
            var newGuid = Guid.NewGuid();
            var newUser = new User() { Name = inName, Guid = newGuid.ToString() };
            Add(newUser);
            await SaveChangesAsync();
            return newUser;
        }

        public DbSet<User> UserSet { get; set; }
    }
}
