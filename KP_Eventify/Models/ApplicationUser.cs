using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KP_Eventify.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;
}
