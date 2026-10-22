using Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Data;

public class ApplicationDbContext : DbContext
{
    // Constructor que recibe las opciones de configuracion (como la configuracion de mysql)
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    // Cada DbSet representa una tabla en nuestra base de datos
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            // Largo fijo: MySQL lo necesita para poder indexar la columna
            entity.Property(c => c.Name)
                .HasMaxLength(100);

            // Garantiza a nivel de base de datos que no haya dos nombres iguales
            entity.HasIndex(c => c.Name)
                .IsUnique();
        });
    }
}