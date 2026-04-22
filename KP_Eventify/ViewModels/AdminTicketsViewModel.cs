using KP_Eventify.Models;

namespace KP_Eventify.ViewModels;

public class AdminTicketsViewModel
{
    public required IEnumerable<Ticket> Tickets { get; set; }
    public required IEnumerable<ApplicationUser> Users { get; set; }
}
