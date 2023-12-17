using B1.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace B1.DataLayer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<DataModel> Data { get; set; }
        public DbSet<TurnoverSheetModel> TurnoverSheets { get; set; }
        public DbSet<TurnoverLineModel> TurnoverLines { get; set; }
        public DbSet<TurnoverClassModel> TurnoverClasses { get; set; }
        public DbSet<TurnoverModel> Turnovers { get; set; }
        public DbSet<BalanceModel> Balances { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TurnoverLineModel>()
                .HasOne(t => t.OutputBalance)
                .WithMany(b => b.OutputLines)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TurnoverLineModel>()
                .HasOne(t => t.InputBalance)
                .WithMany(b => b.InputLines)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
