using AutoOglasi.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoOglasi.Data
{
    public class AutoOglasiContext : DbContext
    {
        public AutoOglasiContext(DbContextOptions<AutoOglasiContext> options)
            : base(options)
        {
        }

        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Marka> Marke { get; set; }
        public DbSet<Model> Modeli { get; set; }
        public DbSet<Kategorija> Kategorije { get; set; }
        public DbSet<Oglas> Oglasi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Korisnik>().ToTable("Korisnici");
            modelBuilder.Entity<Marka>().ToTable("Marke");
            modelBuilder.Entity<Model>().ToTable("Modeli");
            modelBuilder.Entity<Kategorija>().ToTable("Kategorije");
            modelBuilder.Entity<Oglas>().ToTable("Oglasi");
        }
    }
}