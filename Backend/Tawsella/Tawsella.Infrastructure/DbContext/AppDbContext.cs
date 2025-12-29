using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Entities;

namespace Tawsella.Infrastructure.DbContext
{
    public class AppDbContext : IdentityDbContext<AppUser>
    { 
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }


        public DbSet<Customer> Customers { get; set; }
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Merchant> Merchant { get; set; }
        public DbSet<Order> orders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasOne<Admin>()
                .WithOne(a => a.User)
                .HasForeignKey<Admin>(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
                .HasOne<Customer>()
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
                .HasOne<Courier>()
                .WithOne(c => c.User)
                .HasForeignKey<Courier>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
                .HasOne<Merchant>()
                .WithOne(m => m.User)
                .HasForeignKey<Merchant>(m => m.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
