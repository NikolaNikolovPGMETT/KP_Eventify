using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace KP_Eventify.Models;

public class Event
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime Date { get; set; }

    [Range(0, 100000)]
    public decimal Price { get; set; }

    [Range(0, 500000)]
    public int TotalTickets { get; set; }

    [Range(0, 500000)]
    public int AvailableTickets { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
