namespace web_app.Controllers;

using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web_app.Models;
using web_app.Services;

[Authorize]
public class WhatsappController : Controller
{
    private readonly ILogger<WhatsappController> _logger;
    private readonly WhatsappApiService _whatsappApiService;

    public WhatsappController(ILogger<WhatsappController> logger, WhatsappApiService whatsappApiService)
    {
        _logger = logger;
        _whatsappApiService = whatsappApiService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetQRAccess()
    {
        var base64Image = await _whatsappApiService.GetQRAccessBase64ImageAsync();

        ViewBag.Base64Image = base64Image;
        return View("Index");
    }    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
