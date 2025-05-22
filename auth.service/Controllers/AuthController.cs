using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace auth.service.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : Controller
{
    private readonly UserManager<MongoUser> _userManager;
    private readonly RoleManager<MongoIdentityRole<ObjectId>> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<MongoUser> userManager,
        RoleManager<MongoIdentityRole<ObjectId>> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new MongoUser
        {
            UserName = model.Username,
            Email = model.Email,
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber,
            Address = model.Address,
            Avatar = model.Avatar,
            Gender = model.Gender,
            DateOfBirth = model.DateOfBirth
        };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { UserId = user.Id.ToString() });
    }

    [HttpPost("token")]
    public async Task<IActionResult> GetToken([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(1000),
            signingCredentials: creds
        );

        return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    [Authorize]
    [HttpGet("get-user-info")]
    public async Task<IActionResult> GetUserInfo(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId userId))
        {
            return BadRequest("Invalid user ID format. Expected a valid ObjectId.");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return NotFound();
        var userInfo = new
        {
            id,
            user.FullName,
            user.Email,
            user.PhoneNumber,
            user.Address,
            user.Avatar,
            user.Gender,
            user.DateOfBirth
        };
        return Ok(userInfo);
    }

    [Authorize]
    [HttpPut("update-user-info")]
    public async Task<IActionResult> UpdateUserInfo(string id, [FromBody] UpdateUserInfoModel model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        user.FullName = model.FullName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;
        user.Avatar = model.Avatar;
        user.Gender = model.Gender;
        user.DateOfBirth = model.DateOfBirth;
        await _userManager.UpdateAsync(user);
        return Ok(new { Message = "Cập nhật thông tin người dùng thành công" });
    }

    [Authorize]
    [HttpGet("get-all-users")]
    public IActionResult GetAllUsers(
        [FromQuery] bool isPaging = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchFullName = null)
    {
        var query = _userManager.Users.AsQueryable();

        // Apply search filter if searchFullName is provided
        if (!string.IsNullOrEmpty(searchFullName))
        {
            query = query.Where(u => u.FullName.Contains(searchFullName));
        }

        // Apply paging if isPaging is true
        if (isPaging)
        {
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var users = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(user => new
                {
                    id = user.Id.ToString(),
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    user.Address,
                    user.Avatar,
                    user.Gender,
                    user.DateOfBirth,
                    user.IsActive
                })
                .ToList();

            return Ok(new
            {
                Items = users,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            });
        }

        // If not paging, return all results
        var allUsers = query
            .Select(user => new
            {
                id = user.Id.ToString(),
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.Address,
                user.Avatar,
                user.Gender,
                user.DateOfBirth,
                user.IsActive
            })
            .ToList();

        return Ok(allUsers);
    }

    [Authorize]
    [HttpPut("update-user-role")]
    public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateUserRoleModel model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        user.PhanQuyen = model.PhanQuyen;
        await _userManager.UpdateAsync(user);
        return Ok(new { Message = "Cập nhật vai trò người dùng thành công" });
    }

    [Authorize]
    [HttpPut("update-user-password")]
    public async Task<IActionResult> UpdateUserPassword(string id, [FromBody] UpdateUserPasswordModel model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
        await _userManager.UpdateAsync(user);
        return Ok(new { Message = "Cập nhật mật khẩu người dùng thành công" });
    }

    [Authorize]
    [HttpPut("lock-user")]
    public async Task<IActionResult> LockUser(string id, bool isActive)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        user.IsActive = isActive;
        await _userManager.UpdateAsync(user);
        return Ok(new { Message = isActive ? "Mở khóa người dùng thành công" : "Khóa người dùng thành công" });
    }
}

public class RegisterModel
{
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    // public string? PhanQuyen { get; set; } // Thêm trường Role (tùy chọn)
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Avatar { get; set; }
    public bool? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class RoleModel
{
    public string Name { get; set; }
}

public class UpdateUserInfoModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Avatar { get; set; }
    public bool Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
}

public class UpdateUserRoleModel
{
    public string PhanQuyen { get; set; }
}

public class UpdateUserPasswordModel
{
    public string Password { get; set; }
}
