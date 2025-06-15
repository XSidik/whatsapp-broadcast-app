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
            query = query.Where(c => c.Name!.ToLower().Contains(filter.ToLower()) || c.WANumber!.ToLower().Contains(filter.ToLower()));
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
            // check wa_number exists
            var existing = await _context.Contacts.FirstOrDefaultAsync(c => c.WANumber == contact.WANumber && !c.IsDeleted);

            if (existing != null)
            {
                ModelState.AddModelError("WANumber", "WA Number already exists");
                return View(contact);
            }
           
            contact.WANumber = ValidateWANumber(contact.WANumber!);
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

                // check wa_number exists
                var existingContact = await _context.Contacts.FirstOrDefaultAsync(c => c.WANumber == contact.WANumber && c.Id != id);
                if (existingContact != null)
                {
                    ModelState.AddModelError("WANumber", "WA Number already exists");
                    return View(contact);
                }                

                existing.Name = contact.Name;
                existing.WANumber = ValidateWANumber(contact.WANumber!);
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

    private string ValidateWANumber(string number)
    {
        number.Replace("-", "")
                    .Replace(".", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace(" ", "")
                    .Replace("+", "");
                     
        if (number.StartsWith("0"))
        {
            number = string.Concat("62", number.AsSpan(1));
        }

        return number;
    }

    public IActionResult UploadVcf()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadVcf(IFormFile vcfFile)
    {
        if (vcfFile == null || vcfFile.Length == 0)
            return BadRequest("File is empty");

        using var reader = new StreamReader(vcfFile.OpenReadStream());
        var lines = await reader.ReadToEndAsync();
        var contacts = ParseVcf(lines);
        var newContacts = new List<Contact>();

        // You can optimize with bulk insert if needed
        foreach (var contact in contacts)
        {
            if (!string.IsNullOrEmpty(contact.WANumber) && contact.WANumber.Length > 10)
            {
                contact.Name = contact.Name;
                contact.WANumber = ValidateWANumber(contact.WANumber);
                contact.CreatedAt = DateTime.UtcNow;
                contact.CreatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                // check wa_number exists
                var existing = await _context.Contacts.FirstOrDefaultAsync(c => c.WANumber == contact.WANumber);
                if (existing != null)
                    continue;

                newContacts.Add(contact);
            }
        }

        _context.Contacts.AddRange(newContacts);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Imported successfully", count = contacts.Count, totalUploaded = newContacts.Count });
    }

    private static List<Contact> ParseVcf(string fileContent)
    {
        var contacts = new List<Contact>();
        using var reader = new StringReader(fileContent);
        string? line;
        Contact? contact = null;

        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("BEGIN:VCARD"))
                contact = new Contact { Name = "", WANumber = "" };
            else if (line.StartsWith("FN:") && contact != null)
                contact.Name = line.Substring(3);
            else if (line.StartsWith("TEL") && contact != null)
                contact.WANumber = line.Split(':')[1];
            // else if (line.StartsWith("EMAIL") && contact != null)
            //     contact.Email = line.Split(':')[1];
            else if (line.StartsWith("END:VCARD") && contact != null)
            {
                contacts.Add(contact);
                contact = null;
            }
        }

        return contacts;
    }

    public IActionResult Search(string? filter = null)
    {
        var results = _context.Contacts
                     .Where(c => c.Name!.Contains(filter!))
                     .OrderBy(c => c.Name)
                     .Take(10)
                     .Select(c => new
                     {
                         number = c.WANumber,
                         name = c.Name
                     })
                     .ToList();

        return Json(results);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
