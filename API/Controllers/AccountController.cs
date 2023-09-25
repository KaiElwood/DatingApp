using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {

		private readonly DataContext _context;
		private readonly ITokenService _tokenService;
		public AccountController(DataContext context, ITokenService tokenService)
		{
			_context = context;
			_tokenService = tokenService;
		}
        
		[HttpPost("register")] // api/account/register

		// a task is... not sure! but it is returning an appuser here. the function accepts a string username and a string password
		public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
		{
			// check if in use

			if (await UserExists(registerDto.username)) return BadRequest("username is taken");
			// "using" allows you to get rid of extra space added by component.
			using var hmac = new HMACSHA512();

			var user = new AppUser 
			{
				UserName = registerDto.username.ToLower(),
				PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
				PasswordSalt = hmac.Key
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return new UserDto
			{
				username = user.UserName,
				token = _tokenService.CreateToken(user)
			};

		}


		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
		{
			var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.username);

			if (user == null) return Unauthorized("Invalid username");

			using var hmac = new HMACSHA512(user.PasswordSalt);

			var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));

			for (int i = 0; i < computedHash.Length; i++) 
			{
				if  (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
			}

			return new UserDto
				{
					username = user.UserName,
					token = _tokenService.CreateToken(user)
				};
		}

		private async Task<bool> UserExists(string username) 
		{
			return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
		}
    }
}