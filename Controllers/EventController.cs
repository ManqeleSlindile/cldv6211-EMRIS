using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST EVENTS + ADVANCED FILTERING
        public async Task<IActionResult> Index(
            int? eventTypeId,
            DateTime? startDate,
            DateTime? endDate,
            bool? availableOnly)
        {
            var events = _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            // Filter by Event Type
            if (eventTypeId.HasValue)
            {
                events = events.Where(e => e.EventTypeId == eventTypeId.Value);
            }

            // Filter by Start Date
            if (startDate.HasValue)
            {
                events = events.Where(e => e.StartDate >= startDate.Value);
            }

            // Filter by End Date
            if (endDate.HasValue)
            {
                events = events.Where(e => e.EndDate <= endDate.Value);
            }


            // Show only events without bookings
            if (availableOnly == true)
            {
                events = events.Where(e =>
                    !_context.Booking.Any(b => b.EventID == e.EventID));
            }

            ViewBag.EventTypes = await _context.EventType.ToListAsync();

            return View(await events.ToListAsync());
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            ViewBag.Venues = _context.Venue.ToList();
            ViewBag.EventTypes = _context.EventType.ToList();

            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (@event.EndDate < @event.StartDate)
            {
                ModelState.AddModelError("", "End Date cannot be before Start Date.");
            }

            if (ModelState.IsValid)
            {
                _context.Event.Add(@event);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Venues = _context.Venue.ToList();
            ViewBag.EventTypes = _context.EventType.ToList();

            return View(@event);
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Event.FindAsync(id);

            if (@event == null)
                return NotFound();

            ViewBag.Venues = _context.Venue.ToList();
            ViewBag.EventTypes = _context.EventType.ToList();

            return View(@event);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.EventID)
                return NotFound();

            if (@event.EndDate < @event.StartDate)
            {
                ModelState.AddModelError("", "End Date cannot be before Start Date.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(@event);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Venues = _context.Venue.ToList();
            ViewBag.EventTypes = _context.EventType.ToList();

            return View(@event);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (@event == null)
                return NotFound();

            return View(@event);
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (@event == null)
                return NotFound();

            return View(@event);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool hasBookings = await _context.Booking.AnyAsync(b => b.EventID == id);

            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete an event that has bookings.";
                return RedirectToAction(nameof(Index));
            }

            var @event = await _context.Event.FindAsync(id);

            if (@event != null)
            {
                _context.Event.Remove(@event);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}