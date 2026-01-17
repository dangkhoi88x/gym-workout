using GymAngel.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GymAngel.Data;

public class ApplicationDbContext : IdentityDbContext<User, Role, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<DiscountCode> DiscountCodes { get; set; }
    public DbSet<MembershipPlan> MembershipPlans { get; set; }
    public DbSet<MembershipTransaction> MembershipTransactions { get; set; }
    public DbSet<MembershipNotification> MembershipNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // quan hệ CartItem (Cart - Product)
        builder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId);

        builder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId);

        // quan hệ OrderItem (Order - Product)
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId);

        // Product
        builder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        // CartItem
        builder.Entity<CartItem>()
            .Property(c => c.UnitPrice)
            .HasPrecision(18, 2);

        // Order
        builder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);
        
        builder.Entity<Order>()
            .Property(o => o.SubtotalAmount)
            .HasPrecision(18, 2);
        
        builder.Entity<Order>()
            .Property(o => o.DiscountAmount)
            .HasPrecision(18, 2);

        // OrderItem
        builder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);
        
        builder.Entity<DiscountCode>()
            .Property(dc => dc.DiscountValue)
            .HasPrecision(18, 2);
        
        builder.Entity<DiscountCode>()
            .Property(dc => dc.MinimumOrderAmount)
            .HasPrecision(18, 2);
        
        // MembershipPlan
        builder.Entity<MembershipPlan>()
            .Property(mp => mp.Price)
            .HasPrecision(18, 2);
        
        builder.Entity<MembershipPlan>()
            .Property(mp => mp.OriginalPrice)
            .HasPrecision(18, 2);
        
        // MembershipTransaction
        builder.Entity<MembershipTransaction>()
            .Property(mt => mt.Amount)
            .HasPrecision(18, 2);
        
        builder.Entity<MembershipTransaction>()
            .HasOne(mt => mt.User)
            .WithMany()
            .HasForeignKey(mt => mt.UserId);
        
        builder.Entity<MembershipTransaction>()
            .HasOne(mt => mt.MembershipPlan)
            .WithMany(mp => mp.MembershipTransactions)
            .HasForeignKey(mt => mt.MembershipPlanId);
    }
}
