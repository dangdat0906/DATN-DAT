using Microsoft.EntityFrameworkCore;
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
        public DbSet<Contents> Contents { get; set; }
        public DbSet<Authors> Authors { get; set; }
        public DbSet<NewsSources> NewsSources { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<Types> Types { get; set; }
        public DbSet<Content_Categories> Content_Categories { get; set; }
        public DbSet<ContentTypes> ContentTypes { get; set; }
        public DbSet<Content_Groups> Content_Groups { get; set; }
        public DbSet<TitleImages> TitleImages { get; set; }
        public DbSet<Medias> Medias { get; set; }
        public DbSet<QuanLyDonGiaNhuanBut> QuanLyDonGiaNhuanBut { get; set; }
        public DbSet<NhuanBut> NhuanBut { get; set; }
        public DbSet<TheLoai_HeSo> TheLoai_HeSo { get; set; }
    }
}
