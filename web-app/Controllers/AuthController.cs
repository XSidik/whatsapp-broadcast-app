namespace web_app.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using web_app.DTOs;
using web_app.Data;
using BCryptNet = BCrypt.Net.BCrypt;

public class AuthController : Controller
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // Redirect to home if already authenticated
            return RedirectToAction("Index", "Home");
        }
        return View();
    }


    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(Login param)
    {
        if (!ModelState.IsValid)
            return View(param);

        var user = _context.Users.FirstOrDefault(u => u.Email == param.Email);
        if (user != null)
        {
            if (BCryptNet.Verify(param.Password, user.Password))
            {
                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Email!),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
        }

        ModelState.AddModelError("", "Username or password is incorrect");

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login", "Auth");
    }
}
