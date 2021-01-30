using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UpFlux.Library.API.Db
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Loan> Loans { get; set; }

        public LibraryDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Book>(entity =>
            {                        
                entity.Property(p => p.Id).ValueGeneratedOnAdd();                                            
                entity.HasData(
                    new Book { Id = 100, Title = "A Alma do Mundo", Author = "Roger Scruton" },
                    new Book { Id = 101, Title = "Beleza", Author = "Roger Scruton" },
                    new Book { Id = 102, Title = "O Homem Eterno", Author = "G. K. Chesterton" }
                );
            });

            builder.Entity<Loan>(entity =>
            {
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.HasData(
                    new Loan { Id = 1, BookId = 102, User = "Jaison", Borrowed = DateTime.Now }
                );
            });
        }
    }
}
