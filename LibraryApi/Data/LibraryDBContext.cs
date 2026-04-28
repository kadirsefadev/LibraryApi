using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data
{
    public class LibraryDBContext:DbContext

    {
        public LibraryDBContext(DbContextOptions<LibraryDBContext> options) : base(options) { }

        public DbSet<Book> Book => Set<Book>();
            public DbSet<Author> Authors => Set<Author>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FullName = "Halis Gökçe", Country = "TR" },
                new Author { Id = 2, FullName = "Victor Hugo", Country = "FR" },
                new Author { Id = 3, FullName = "J.R.R. Tolkien", Country = "ZA" },
                new Author { Id = 4, FullName = "Fyodor Mihayloviç Dostoyevski", Country = "RU" },
                new Author { Id = 5, FullName = "Ahmet Keleş", Country = "TR" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Halis ve Harikalar Diyarı", Year = 1965, Category = "Roman", AuthorId = 1 },


                new Book { Id = 2, Title = "Suç ve Ceza", Year = 1975, Category = "Roman", AuthorId = 4 },


                new Book { Id = 3, Title = "Sefiller ", Year = 1982, Category = "Roman", AuthorId = 2 },


                new Book { Id = 4, Title = "Yüzüklerin Efendisi", Year = 1937, Category = "Roman", AuthorId = 3 }
            );
        }
    }
}
