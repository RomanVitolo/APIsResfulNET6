using System.ComponentModel.DataAnnotations;

namespace WebApiAuthor.DTOs;

public class EditAdminDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}