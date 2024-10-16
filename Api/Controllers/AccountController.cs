using System.Security.Cryptography;
using System.Text;
using Api.Data;
using Api.Dtos;
using Api.Entities;
using Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
    {

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            using var hmac = new HMACSHA512();

            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            var user = new AppUser
            {
                UserName = registerDto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            var userDto = new UserDto {
                Username = user.UserName,
                Token = tokenService.CreateToken(user)
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return userDto;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await context.Users.FirstOrDefaultAsync(x=> x.UserName.ToLower() == loginDto.Username.ToLower());
            if (user == null) return Unauthorized("Invalid credentials");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid credentials");
            }
            
            return new UserDto {
                Username = user.UserName,
                Token = tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username) 
        {
            return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
    }
}