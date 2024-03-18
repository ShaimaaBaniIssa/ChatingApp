
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Services.Interfaces;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService,
        IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("Username is taken");
            }
            var user = _mapper.Map<AppUser>(registerDto);
            using var hmac = new HMACSHA512(); // implement IDisposable Interface ,when we finish with this class then the dispose methhod will be called 

            user.UserName = registerDto.UserName.ToLower();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
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

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(u => u.IsMain == true)?.Url,
                KnownAs = user.KnownAs
            };

        }
        private async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName.ToLower());
        }
    }
}