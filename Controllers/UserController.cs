using Bonjour.Domain.Users;
using Bonjour.Models;
using Bonjour.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bonjour.Controllers;

[Authorize(Policy = "Admin")]
public class UserController : Controller
{
    private readonly ApplicationDbContext dbContext;
    private readonly PasswordHasher passwordHasher;

    public UserController(ApplicationDbContext dbContext, PasswordHasher passwordHasher)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
    }

    public async Task<IActionResult> Index()
    {
        var _users = await dbContext.Users.ToListAsync();
        return View(_users);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateUserRequest createUserRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(JsonConvert.SerializeObject(ModelState));
        }
        var hashedPassword = passwordHasher.HashPassword(createUserRequest.Password);
        var _user = new User()
        {
            Name = createUserRequest.Name,
            Username = createUserRequest.Username,
            Email = "example@email.com",
            Password = hashedPassword.HashedPassword,
            Salt = hashedPassword.Salt
        };
        dbContext.Users.Add(_user);
        await dbContext.SaveChangesAsync();
        return Ok(JsonConvert.SerializeObject(_user));
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var _user = await dbContext.Users.FindAsync(id);
            if (_user == null)
            {
                return NotFound();
            }
            var _userHasRoles = await dbContext.UserHasRoles.Where(m => m.UserId == id).ToListAsync();
            dbContext.UserHasRoles.RemoveRange(_userHasRoles);
            dbContext.Users.Remove(_user);
            await dbContext.SaveChangesAsync();
            return Ok("Deleted successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}