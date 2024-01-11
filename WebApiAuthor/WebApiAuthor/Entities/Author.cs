using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAuthor.Validations;

namespace WebApiAuthor.Entities;

public class Author
{    
    public int Id  { get; set; }     
    [Required(ErrorMessage = "The {0} field is required")]
    [StringLength(maximumLength: 120, ErrorMessage = "The {0} field have more than {1} characters")]  
    [FirstCapitalLetter]
    public string Name { get; set; }    
    public List<AuthorBook> AuthorsBooks { get; set; }
}