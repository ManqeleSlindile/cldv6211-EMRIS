using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================
        // Display all bookings
        // ==========================
        public async Task<IActionResult> Index(string searchString)
        {
            var bookings = _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                    b.BookingID.ToString().Contains(searchString) ||
                    b.Event!.EventName!.Contains(searchString));
            }

            return View(await bookings.ToListAsync());
        }

        // ==========================
        // GET: Create
        // ==========================
        public IActionResult Create()
        {
            ViewBag.Events = _context.Event.ToList();
            ViewBag.Venues = _context.Venue.ToList();

            return View();
        }

        // ==========================
        // POST: Create
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            var selectedEvent = await _context.Event
                .FirstOrDefaultAsync(e => e.EventID == booking.EventID);

            if (selectedEvent != null)
            {
                bool conflict = await _context.Booking
                    .Include(b => b.Event)
                    .AnyAsync(b =>
                        b.VenueID == booking.VenueID &&
                        selectedEvent.StartDate < b.Event!.EndDate &&
                        selectedEvent.EndDate > b.Event.StartDate);

                if (conflict)
                {
                    ModelState.AddModelError("", "This venue is already booked during the selected dates.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Booking.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Events = _context.Event.ToList();
            ViewBag.Venues = _context.Venue.ToList();

            return View(booking);
        }

        // ==========================
        // GET: Details
        // ==========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // ==========================
        // GET: Edit
        // ==========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Booking.FindAsync(id);

            if (booking == null)
                return NotFound();

            ViewBag.EventID = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);

            return View(booking);
        }

        // ==========================
        // POST: Edit
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingID)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.EventID = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);

            return View(booking);
        }

        // ==========================
        // GET: Delete
        // ==========================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // ==========================
        // POST: Delete
        // ==========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);

            if (booking != null)
            {
                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}