using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using pool_game_web.Models;
using Microsoft.AspNetCore.Identity;

namespace pool_game_web.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser,IdentityRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUser>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);    
        }
        public DbSet<Reservation> Reservations { get; set;}
        public DbSet<PoolTable> PoolTables { get; set;}
        public DbSet<IdentityUser> IdentityUsers { get; set;}
        // public DbSet<IdentityRole> IdentityRoles { get; set;}
    }
}
