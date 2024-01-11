using Microsoft.EntityFrameworkCore;
using WebApiAuthor.Entities;

namespace WebApiAuthor;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }

    public DbSet<Author> Authors { get; set; }    //Create a table from this Scheme
    public DbSet<Book> Books { get; set; } //Para poder crear Querys a la tabla de Libros
    public DbSet<Comment> Comments { get; set; }
}