using System.ComponentModel.DataAnnotations;
using WebApiAuthor.Validations;

namespace WebApiAuthor.DTOs
{
    public class BookCreationDTO
    {
        [FirstCapitalLetter]
        [StringLength(maximumLength: 250)]
        [Required]
        public string Title { get; set; }

        public DateTime PublicationDate { get; set; }
        public List<int> AuthorsIds { get; set; }
    }
}

