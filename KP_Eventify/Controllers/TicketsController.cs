using KP_Eventify.Constants;
using KP_Eventify.Data;
using KP_Eventify.Models;
using KP_Eventify.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Sockets;

namespace KP_Eventify.Controllers;

[Authorize]
public class TicketsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> MyTickets()
    {
        var userId = _userManager.GetUserId(User);
        var tickets = await _context.Tickets
            .Include(t => t.Event)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.PurchasedOn)
            .ToListAsync();

        return View(tickets);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Buy(int eventId)
    {
        var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        if (eventItem == null)
        {
            return NotFound();
        }

        if (eventItem.AvailableTickets <= 0)
        {
            TempData["ErrorMessage"] = "Няма налични билети за това събитие.";
            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        eventItem.AvailableTickets -= 1;

        var ticket = new Ticket
        {
            EventId = eventId,
            UserId = userId,
            PurchasedOn = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Билетът е купен успешно!";
        return RedirectToAction(nameof(MyTickets));
    }

    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> AdminTickets()
    {
        var model = new AdminTicketsViewModel
        {
            Tickets = await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.User)
                .OrderByDescending(t => t.PurchasedOn)
                .ToListAsync(),
            Users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync()
        };

        return View(model);
    }
}
