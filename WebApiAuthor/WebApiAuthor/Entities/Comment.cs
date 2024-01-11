namespace WebApiAuthor.Entities;

public class Comment        //El comentario depende completamente del Libro (no al reves) 
{ 
      public int Id { get; set; }
      public string Content { get; set; }
      public int BookId { get; set; }
      public Book Book { get; set; }
}