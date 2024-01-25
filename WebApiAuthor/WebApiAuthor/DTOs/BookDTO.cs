namespace WebApiAuthor.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<CommentDTO> Comments { get; set; } //ESta linea es opcional por si queremos traer los comentarios 
    }                                                 //Tendriamos que borrar el include del AuthorsController tambien.
}

