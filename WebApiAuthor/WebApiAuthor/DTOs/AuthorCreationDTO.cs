using System.ComponentModel.DataAnnotations;
using WebApiAuthor.Validations;

namespace WebApiAuthor.DTOs
{
    public class AuthorCreationDTO
    {
        [Required(ErrorMessage = "The {0} field is required")]
        [StringLength(maximumLength: 120, ErrorMessage = "The {0} field have more than {1} characters")]
        [FirstCapitalLetter]
        public string Name { get; set; }
    }
}

