
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.Enums;

namespace OnlineShop.Models
{
    public class SqlContext(DbContextOptions<SqlContext> options)
        : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Token> Tokens => Set<Token>();

        // Cart
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        // Order
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<Image>().ToTable("images");
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Token>().ToTable("tokens");

            // =========================
            // Cart
            // =========================

            modelBuilder.Entity<Cart>(e =>
            {
                e.ToTable("carts");

                e.HasKey(x => x.Id);

                e.HasOne(x => x.User)
                    .WithOne(x => x.Cart)
                    .HasForeignKey<Cart>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => x.UserId)
                    .IsUnique();
            });

            // =========================
            // CartItem
            // =========================

            modelBuilder.Entity<CartItem>(e =>
            {
                e.ToTable("cartitems");

                e.HasKey(x => x.Id);

                e.HasOne(x => x.Cart)
                    .WithMany(x => x.Items)
                    .HasForeignKey(x => x.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Product)
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.CartId, x.ProductId })
                    .IsUnique();

                e.HasIndex(x => x.ProductId);
            });

            // =========================
            // Order
            // =========================

            modelBuilder.Entity<Order>(e =>
            {
                e.ToTable("orders");

                e.HasKey(x => x.Id);

                // Order Number
                e.Property(x => x.OrderNumber)
                    .HasMaxLength(50)
                    .IsRequired();

                e.HasIndex(x => x.OrderNumber)
                    .IsUnique();

                // User
                e.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Order Items
                e.HasMany(x => x.Items)
                    .WithOne(x => x.Order)
                    .HasForeignKey(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Payments
                e.HasMany(x => x.Payments)
                    .WithOne(x => x.Order)
                    .HasForeignKey(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Enum conversions
                e.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(30);

                e.Property(x => x.PaymentStatus)
                    .HasConversion<string>()
                    .HasMaxLength(30);

                e.Property(x => x.PaymentMethod)
                    .HasConversion<string>()
                    .HasMaxLength(30);

                e.Property(x => x.ShippingMethod)
                    .HasConversion<string>()
                    .HasMaxLength(30);

                // Money precision
                e.Property(x => x.SubTotal)
                    .HasPrecision(18, 2);

                e.Property(x => x.DiscountTotal)
                    .HasPrecision(18, 2);

                e.Property(x => x.ShippingCost)
                    .HasPrecision(18, 2);

                e.Property(x => x.TotalPrice)
                    .HasPrecision(18, 2);

                // Useful indexes
                e.HasIndex(x => x.UserId);

                e.HasIndex(x => new { x.UserId, x.CreateAt });

                e.HasIndex(x => x.Status);

                e.HasIndex(x => x.PaymentStatus);
            });

            // =========================
            // OrderItem
            // =========================

            modelBuilder.Entity<OrderItem>(e =>
            {
                e.ToTable("orderitems");

                e.HasKey(x => x.Id);

                e.Property(x => x.ProductName)
                    .HasMaxLength(200)
                    .IsRequired();

                // Historical product relationship
                e.HasOne(x => x.Product)
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Money precision
                e.Property(x => x.UnitPrice)
                    .HasPrecision(18, 2);

                e.Property(x => x.FinalUnitPrice)
                    .HasPrecision(18, 2);

                e.Property(x => x.DiscountAmount)
                    .HasPrecision(18, 2);

                e.Property(x => x.TotalPrice)
                    .HasPrecision(18, 2);

                e.HasIndex(x => x.OrderId);

                e.HasIndex(x => x.ProductId);
            });

            // =========================
            // Payment
            // =========================

            modelBuilder.Entity<Payment>(e =>
            {
                e.ToTable("payments");

                e.HasKey(x => x.Id);

                e.HasOne(x => x.Order)
                    .WithMany(x => x.Payments)
                    .HasForeignKey(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.Amount)
                    .HasPrecision(18, 2);

                e.Property(x => x.Method)
                    .HasConversion<string>()
                    .HasMaxLength(30);

                e.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(30);

                // Gateway identifiers
                e.HasIndex(x => new { x.Gateway, x.Authority })
                    .IsUnique();

                e.HasIndex(x => new { x.Gateway, x.TransactionId })
                    .IsUnique();

                e.HasIndex(x => x.OrderId);

                e.HasIndex(x => x.Status);
            });
        }
    }
}
