using DematecStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DematecStock.Infrastructure.DataAccess
{
    public class DematecStockDbContext : DbContext
    {
        public DematecStockDbContext(DbContextOptions options) : base(options) { }

         public DbSet<WarehouseLocations> WarehouseLocations { get; set; }

         public DbSet<User> Users { get; set; }
    }
}
