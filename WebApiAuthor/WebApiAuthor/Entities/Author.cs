using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAuthor.Validations;

namespace WebApiAuthor.Entities;

public class Author : IValidatableObject
{    
    public int Id  { get; set; }     
    [Required(ErrorMessage = "The {0} field is required")]
    [StringLength(maximumLength: 120, ErrorMessage = "The {0} field have more than {1} characters")]  
    //[FirstCapitalLetter]
    public string Name { get; set; }    
    /*[Range(18, 120)]
    [NotMapped]
    public int Age { get; set; }
    [CreditCard]
    [NotMapped]
    public string CreditCard { get; set; }
    [Url] 
    [NotMapped]
    public string Url { get; set; }*/
    /*public int Min { get; set; }
    public int Max { get; set; }*/
    public List<Book> Books { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Name))
        {
            var firstLetter = Name[0].ToString();
            if (firstLetter != firstLetter.ToUpper())
            {
                yield return new ValidationResult("The first letter must be Upper",
                new string[] {nameof(Name)});
            }
        }

        /*if (Min > Max)
        {
            yield return new ValidationResult("This value cannot be more than the Max field",
                new string[] {nameof(Min)});
        }*/
    }
}