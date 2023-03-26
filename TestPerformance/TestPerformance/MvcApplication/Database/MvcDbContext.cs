using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MvcApplication.Database;

public class MvcDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;

    public MvcDbContext(
        DbContextOptions<MvcDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasOne(x => x.Cart).WithOne(x => x.User);
    }
}

public class User
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public long Id { get; set; }
    public DateTimeOffset BirthDate { get; set; }
    public Cart Cart { get; set; }
    public long CartId { get; set; }
}

public class Cart
{
    public long Id { get; set; }
    public User User { get; set; }
    public int Amount { get; set; }
    public IEnumerable<CartItem> Items { get; set; }
}

public class CartItem
{
    public long Id { get; set; }
    public Item Items { get; set; }
    public Cart Carts { get; set; }
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ShortDescription { get; set; }
    public int AmountInWereHouse { get; set; }
    public IEnumerable<CartItem> Carts { get; set; }
}
