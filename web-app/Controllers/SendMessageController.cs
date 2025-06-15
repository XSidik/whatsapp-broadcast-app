namespace web_app.Controllers;

using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web_app.DTOs;
using web_app.Models;
using web_app.Services;

[Authorize]
public class SendMessageController : Controller
{
    private readonly ILogger<SendMessageController> _logger;
    private readonly WhatsappApiService _whatsappApiService;

    public SendMessageController(ILogger<SendMessageController> logger, WhatsappApiService whatsappApiService)
    {
        _logger = logger;
        _whatsappApiService = whatsappApiService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(SendMessageDto sendMessageDto)
    {
       if (!ModelState.IsValid)
            return View("Index");

        
        foreach (var number in sendMessageDto.Numbers)
        {
            await _whatsappApiService.SendMessageAsync(number, sendMessageDto.Message);
        }
        
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
