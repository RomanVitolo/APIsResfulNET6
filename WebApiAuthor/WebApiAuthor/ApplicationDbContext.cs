using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAuthor.Entities;

namespace WebApiAuthor;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       base.OnModelCreating(modelBuilder);

       modelBuilder.Entity<AuthorBook>().HasKey(al => new
       {
           al.AuthorId, al.BookId
       });
    }

    public DbSet<Author> Authors { get; set; }    //Create a table from this Scheme
    public DbSet<Book> Books { get; set; } //Para poder crear Querys a la tabla de Libros
    public DbSet<Comment> Comments { get; set; }
    public DbSet<AuthorBook> AuthorsBooks { get; set; }
}