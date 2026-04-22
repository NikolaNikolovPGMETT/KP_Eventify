using System.ComponentModel.DataAnnotations;

namespace KP_Eventify.Models;

public class Ticket
{
    public int Id { get; set; }

    [Required]
    public int EventId { get; set; }
    public Event? Event { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public DateTime PurchasedOn { get; set; }
}