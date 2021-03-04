﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace APP.MODELS
{
    public class APPDbContext : DbContext
    {
        private static readonly MethodInfo _propertyMethod = typeof(EF).GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(typeof(bool));
        public APPDbContext(DbContextOptions<APPDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // modelBuilder.HasDefaultSchema("orcl");
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Menus> Menus { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Role_Permissions> Role_Permissions { get; set; }
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<AccountRoles> AccountRoles { get; set; }
        public DbSet<Categories> Categories { get; set; }
    }
}
