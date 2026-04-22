using KP_Eventify.Constants;
using KP_Eventify.Data;
using KP_Eventify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace KP_Eventify.Controllers;

public class EventsController : Controller
{
    private readonly ApplicationDbContext _context;

    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _context.Events.OrderBy(e => e.Date).ToListAsync();
        return View(events);
    }

    public async Task<IActionResult> Details(int id)
    {
        var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
        if (eventItem == null)
        {
            return NotFound();
        }

        return View(eventItem);
    }

    [Authorize(Roles = Roles.Admin)]
    public IActionResult Create()
    {
        return View(new Event { Date = DateTime.UtcNow.AddDays(7) });
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Event eventItem)
    {
        if (eventItem.TotalTickets < 0)
        {
            ModelState.AddModelError(nameof(Event.TotalTickets), "Общият брой билети трябва да е положително число.");
        }

        if (eventItem.AvailableTickets != eventItem.TotalTickets)
        {
            ModelState.AddModelError(nameof(Event.AvailableTickets), "Наличните билети трябва да са равни на общия брой при създаване.");
        }

        if (!ModelState.IsValid)
        {
            return View(eventItem);
        }

        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Edit(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null)
        {
            return NotFound();
        }

        return View(eventItem);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Event eventItem)
    {
        if (id != eventItem.Id)
        {
            return NotFound();
        }

        if (eventItem.AvailableTickets > eventItem.TotalTickets)
        {
            ModelState.AddModelError(nameof(Event.AvailableTickets), "Наличните билети не могат да са повече от общия брой.");
        }

        if (!ModelState.IsValid)
        {
            return View(eventItem);
        }

        _context.Update(eventItem);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
        if (eventItem == null)
        {
            return NotFound();
        }

        return View(eventItem);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = Roles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem != null)
        {
            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}