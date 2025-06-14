namespace web_app.Controllers;

using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_app.Data;
using web_app.Helper;
using web_app.Models;

[Authorize]
public class ContactController : Controller
{
    private readonly ILogger<ContactController> _logger;
    private readonly AppDbContext _context;
    private const int PageSize = 10;


    public ContactController(ILogger<ContactController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> IndexAsync(int page = 1,
        int pageSize = 10,
        string? filter = null,
        string sortBy = "Name",
        string sortOrder = "asc")
    {
        var query = _context.Contacts
       .Where(c => !c.IsDeleted);

        // Add filtering and sorting logic here as needed
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(c => c.Name!.ToLower().Contains(filter.ToLower()));
        }

        query = (sortBy.ToLower(), sortOrder.ToLower()) switch
        {
            ("name", "asc") => query.OrderBy(c => c.Name),
            ("name", "desc") => query.OrderByDescending(c => c.Name),
            _ => query.OrderBy(c => c.Name)
        };

        var paginatedResult = await PaginationHelper.CreateAsync(query, page, pageSize, filter, sortBy, sortOrder);

        return View(paginatedResult);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var contact = await _context.Contacts.FindAsync(id);

        if (contact == null || contact.IsDeleted)
            return NotFound();

        return View(contact);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Contact contact)
    {
        if (ModelState.IsValid)
        {
            contact.CreatedAt = DateTime.UtcNow;
            contact.CreatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _context.Add(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(contact);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var contact = await _context.Contacts.FindAsync(id);

        if (contact == null || contact.IsDeleted)
            return NotFound();

        return View(contact);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Contact contact)
    {
        if (id != contact.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var existing = await _context.Contacts.FindAsync(id);
                if (existing == null || existing.IsDeleted)
                    return NotFound();

                existing.Name = contact.Name;
                existing.WANumber = contact.WANumber;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                _context.Update(existing);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(contact.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(contact);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var contact = await _context.Contacts.FindAsync(id);

        if (contact == null || contact.IsDeleted)
            return NotFound();

        return View(contact);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact != null)
        {
            contact.IsDeleted = true;
            contact.UpdatedAt = DateTime.UtcNow;
            contact.UpdatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _context.Update(contact);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ContactExists(int id)
    {
        return _context.Contacts.Any(e => e.Id == id && !e.IsDeleted);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
