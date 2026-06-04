using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using DarazUltimateMVC.Data;
using DarazUltimateMVC.Models;
using DarazUltimateMVC.Services;

namespace DarazUltimateMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest(new { message = "Email already exists!" });
            }
            
            var hashedPassword = PasswordService.HashPassword(model.Password);
            
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = hashedPassword,
                Role = model.Role ?? "USER"
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role);
            
            return Ok(new { message = "Registration successful!", name = user.Name, role = user.Role });
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            
            if (user == null)
            {
                return BadRequest(new { message = "Invalid email or password!" });
            }
            
            bool isValid = PasswordService.VerifyPassword(model.Password, user.PasswordHash);
            
            if (!isValid)
            {
                return BadRequest(new { message = "Invalid email or password!" });
            }
            
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role);
            
            return Ok(new { message = "Login successful!", name = user.Name, role = user.Role });
        }
        
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully!" });
        }
        
        [HttpGet("check")]
        public IActionResult CheckAuth()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
                
            return Ok(new { userId, name = userName, role = userRole });
        }
    }
}
