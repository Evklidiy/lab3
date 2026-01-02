using Microsoft.EntityFrameworkCore;
using AutoPartsWarehouse.Models;

namespace AutoPartsWarehouse.Data
{
    public class AutoPartsContext : DbContext
    {
        public AutoPartsContext(DbContextOptions<AutoPartsContext> options) : base(options) { }

        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplyBatch> SupplyBatches { get; set; }
        public DbSet<SupplyItem> SupplyItems { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Указываем точность для всех полей типа decimal (18 знаков всего, 2 после запятой)

            modelBuilder.Entity<SparePart>()
                .Property(p => p.PurchasePrice).HasPrecision(18, 2);
            modelBuilder.Entity<SparePart>()
                .Property(p => p.SalePrice).HasPrecision(18, 2);

            modelBuilder.Entity<SupplyItem>()
                .Property(p => p.PurchasePrice).HasPrecision(18, 2);

            modelBuilder.Entity<SaleItem>()
                .Property(p => p.SalePrice).HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}