using Bonjour.Domain.Helpers;
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
            return UnprocessableEntity(ModelState);
        }
        if (await dbContext.Users.AnyAsync(u => u.Username == createUserRequest.Username))
        {
            ModelState.AddModelError("Username", "Username already exists");
            return UnprocessableEntity(ModelState);
        }
        var hashedPassword = passwordHasher.HashPassword(createUserRequest.Password);
        var roles = JsonConvert.DeserializeObject<string[]>(createUserRequest.Roles);
        if (roles.Length == 0)
        {
            ModelState.AddModelError("RoleId", "Select at least one role");
            return UnprocessableEntity(ModelState);
        }
        var _user = new User()
        {
            Name = createUserRequest.Name,
            Username = createUserRequest.Username,
            Email = "example@email.com",
            Password = hashedPassword.HashedPassword,
            Salt = hashedPassword.Salt,
            RoleId = int.Parse(roles.First()),
        };
        dbContext.Users.Add(_user);
        foreach (var roleId in roles)
        {
            var _userHasRole = new UserHasRole()
            {
                User = _user,
                RoleId = int.Parse(roleId)
            };
            dbContext.UserHasRoles.Add(_userHasRole);
        }
        await dbContext.SaveChangesAsync();
        return Ok("User created successfully");
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var _user = await dbContext.Users.FindAsync(id);
        if (_user == null)
        {
            ModelState.AddModelError("User", "User not found");
            return UnprocessableEntity(ModelState);
        }
        if (_user.Username == "admin")
        {
            ModelState.AddModelError(string.Empty, "Admin user cannot be deleted");
            return UnprocessableEntity(ModelState);
        }
        if (_user.Username == User.Identity.Name)
        {
            ModelState.AddModelError("User", "You cannot delete your own account");
            return UnprocessableEntity(ModelState);
        }
        var _userHasRoles = await dbContext.UserHasRoles.Where(m => m.UserId == id).ToListAsync();
        dbContext.UserHasRoles.RemoveRange(_userHasRoles);
        dbContext.Users.Remove(_user);
        await dbContext.SaveChangesAsync();
        return Ok("Deleted successfully");
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword(int id, ChangePasswordRequest changePasswordRequest)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var _user = await dbContext.Users.FindAsync(id);
        if (_user == null)
        {
            ModelState.AddModelError("User", "User not found");
            return UnprocessableEntity(ModelState);
        }
        if (_user.Username == "admin" && User.Identity.Name != "admin")
        {
            ModelState.AddModelError(string.Empty, "Only admin can change admin password");
            return UnprocessableEntity(ModelState);
        }
        var password = passwordHasher.HashPassword(changePasswordRequest.Password);
        _user.Password = password.HashedPassword;
        _user.Salt = password.Salt;
        dbContext.Users.Update(_user);
        await dbContext.SaveChangesAsync();
        return Ok("Password changed successfully");
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var _user = await dbContext.Users.FindAsync(id);
        if (_user == null)
        {
            ModelState.AddModelError("User", "User not found");
            return UnprocessableEntity(ModelState);
        }
        var _userHasRoles = await dbContext.UserHasRoles.Where(m => m.UserId == id).ToListAsync();
        var roles = _userHasRoles.Select(m => m.RoleId).ToArray();
        var editUserResponse = new EditUserRequest()
        {
            Name = _user.Name,
            Username = _user.Username,
            Roles = JsonConvert.SerializeObject(roles)
        };
        return Ok(editUserResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, EditUserRequest editUserRequest)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var _user = await dbContext.Users.FindAsync(id);
        if (_user == null)
        {
            ModelState.AddModelError("User", "User not found");
            return UnprocessableEntity(ModelState);
        }
        if (await dbContext.Users.AnyAsync(u => u.Username == editUserRequest.Username && u.Id != id))
        {
            ModelState.AddModelError("Username", "Username already exists");
            return UnprocessableEntity(ModelState);
        }
        var roles = JsonConvert.DeserializeObject<string[]>(editUserRequest.Roles);
        if (roles.Length == 0)
        {
            ModelState.AddModelError("RoleId", "Select at least one role");
            return UnprocessableEntity(ModelState);
        }
        _user.Name = editUserRequest.Name;
        _user.Username = editUserRequest.Username;
        _user.RoleId = int.Parse(roles.First());
        dbContext.Users.Update(_user);
        var _userHasRoles = await dbContext.UserHasRoles.Where(m => m.UserId == id).ToListAsync();
        if (_userHasRoles.Any())
        {
            dbContext.UserHasRoles.RemoveRange(_userHasRoles);
        }
        foreach (var roleId in roles)
        {
            var _userHasRole = new UserHasRole()
            {
                UserId = id,
                RoleId = int.Parse(roleId)
            };
            dbContext.UserHasRoles.Add(_userHasRole);
        }
        await dbContext.SaveChangesAsync();
        return Ok("Deleted successfully");
    }
}