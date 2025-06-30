using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ibuy.Models;
using Ibuy.Service;
using Microsoft.AspNetCore.Mvc;

using Ibuy.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Ibuy.Controllers
{
    [ApiController]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordServices _passwordService = new();
        private readonly UserService _userService;

        public UsersController(AppDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] UserFromRequest userFromRequest)
        {
            if (string.IsNullOrWhiteSpace(userFromRequest.Email))
            {
                return BadRequest(new { error = "Email is required for registration" });
            }
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == userFromRequest.Username);

            if (existingUser != null)
            {
                return Conflict("Username already taken");
            }
            if (string.IsNullOrEmpty(userFromRequest.PreferredContact))
            {
                userFromRequest.PreferredContact = "None";
            }

            var user = userFromRequest.GetUser(_passwordService);
            _context.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User registered successfully");
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] UserFromRequest userFromRequest)
        {
            if (userFromRequest == null)
            {
                return BadRequest("Request body is null.");
            }

            if (string.IsNullOrEmpty(userFromRequest.Username) || string.IsNullOrEmpty(userFromRequest.Password))
            {
                return BadRequest("Username or password missing.");
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == userFromRequest.Username);
            if (user == null)
            {
                return Unauthorized("User not Found");
            }

            bool isPassword = _passwordService.VerifyPassword(user.PasswordHash, userFromRequest.Password);
            if (!isPassword)
            {
                return Unauthorized("Invalid Password");
            }
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);

            return Ok(new { Message = "Login Success"});
        }
        [HttpGet("/login/user/{username}")]
        public async Task<IActionResult> GetUser(string username)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }



            return Ok(user);
            
        }   

        [HttpPost ("/user/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session", new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            return Ok("Logged Out");
        }

   
        [HttpPut("update/user/profile")]
        public async Task<IActionResult> UpdateContactInfo([FromBody] ContactUpdateRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
            {
                return NotFound(new { message = "User not found"});
            }

            user.PreferredContact = request.PreferredContact;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Contact info updated" });


        }

        

    }
}