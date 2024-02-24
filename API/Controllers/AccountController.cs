
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("Username is taken");
            }
            using var hmac = new HMACSHA512(); // implement IDisposable Interface ,when we finish with this class then the dispose methhod will be called 
            var user = new AppUser()
            {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),

            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.
            Include(p => p.Photos).
            FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
            if (user == null)
            {
                return Unauthorized("Invalid Username");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < hashedPassword.Length; i++)
            {
                if (hashedPassword[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }
            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(u => u.IsMain == true)?.Url

            };

        }
        private async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName.ToLower());
        }
    }
}