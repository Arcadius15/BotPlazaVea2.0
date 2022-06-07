using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotPlazaVea2._0.Models
{
    public class PlazaVeaContext:DbContext
    {
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Urls> Urls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseIdentityColumns();

            modelBuilder.Entity<Urls>()
                .HasOne(a => a.Producto)
                .WithOne(b => b.Url)
                .HasForeignKey<Productos>(b => b.idUrl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_URL_1");

            modelBuilder.Entity<Urls>()
                .HasIndex(x => x.url)
                .IsUnique();

            modelBuilder.Entity<Urls>()
                .Property(p => p.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Productos>()
                .Property(p => p.id)
                .ValueGeneratedOnAdd();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

            => optionsBuilder.UseNpgsql(@"Host=localhost;Database=PlazaVeaData;Username=postgres;Password=admin");

    }

}

