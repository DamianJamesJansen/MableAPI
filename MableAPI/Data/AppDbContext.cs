using Microsoft.EntityFrameworkCore;
namespace MableAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}
