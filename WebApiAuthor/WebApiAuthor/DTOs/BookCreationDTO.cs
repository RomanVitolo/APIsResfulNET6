using System.ComponentModel.DataAnnotations;
using WebApiAuthor.Validations;

namespace WebApiAuthor.DTOs;

public class BookCreationDTO
{
    [FirstCapitalLetter]
    [StringLength(maximumLength: 250)]
    public string Title { get; set; }
}