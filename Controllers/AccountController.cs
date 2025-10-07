using System.Security.Claims;
using Bonjour.Domain.Users;
using Bonjour.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bonjour.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext dbContext;
    private readonly PasswordHasher passwordHasher;

    public AccountController(ApplicationDbContext dbContext, PasswordHasher passwordHasher)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(AccountModel account)
    {
        if (!ModelState.IsValid)
        {
            return View(account);
        }
        var _user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == account.Username);
        if (_user == null)
        {
            ModelState.AddModelError(nameof(account.Username), "User not found");
            return View(account);
        }
        if (passwordHasher.VerifyPassword(account.Password, _user.Password, _user.Salt))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Role, "Shipper")
            };
            var claimsIdentity = new ClaimsIdentity(claims, "cookie");
            await HttpContext.SignInAsync("cookie", new ClaimsPrincipal(claimsIdentity));
            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError(nameof(account.Password), "Invalid password");
        return View(account);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("cookie");
        return RedirectToAction("Login", "Account");
    }

    [Authorize]
    public IActionResult AccessDenied() => View();
}