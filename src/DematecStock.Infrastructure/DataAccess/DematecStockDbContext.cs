using DematecStock.Domain.DTOs;
using DematecStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DematecStock.Infrastructure.DataAccess
{
    public class DematecStockDbContext : DbContext
    {
        public DematecStockDbContext(DbContextOptions options) : base(options) { }

        public DbSet<WarehouseLocations> WarehouseLocations { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ProductSearchQueryResult> ProductSearch => Set<ProductSearchQueryResult>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FlatLocationWithProductsQueryResult>(entity =>
            {
                entity.HasNoKey()
                    .ToView("vw_LocationProducts", "dbo");

                entity.Property(p => p.GrossWeight).HasPrecision(18, 6);
                entity.Property(p => p.NetWeight).HasPrecision(18, 6);
                entity.Property(p => p.MinQuantity).HasPrecision(18, 4);
                entity.Property(p => p.MaxQuantity).HasPrecision(18, 4);
                entity.Property(p => p.OnHandQuantity).HasPrecision(18, 4);
                entity.Property(p => p.PackQuantity).HasPrecision(18, 4);
            });


            modelBuilder.Entity<ProductSearchQueryResult>(entityProductSearch =>
            {
                entityProductSearch.HasNoKey();
                entityProductSearch.ToView(null);
            });

            modelBuilder.Entity<InventoryLocation>(entity =>
            {
                entity.HasKey(e => new { e.IdLocation, e.IdProduct });
                entity.Property(e => e.OnHandQuantity).HasPrecision(18, 4);
            });
        }

        public DbSet<InventoryLocation> InventoryLocation { get; set; }
    }
}
