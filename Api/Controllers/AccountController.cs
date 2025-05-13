using System.Security.Cryptography;
using System.Text;
using Api.Data;
using Api.Dtos;
using Api.Entities;
using Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper) : BaseApiController
    {

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            if (await UserExists(registerDto.Username)) 
                return BadRequest("Username is taken");

            var user = mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.ToLower();

            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) 
                return BadRequest(result.Errors);

            var userDto = new UserDto {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = await tokenService.CreateToken(user),
                Gender = user.Gender
            };

            return userDto;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.Users
                .Include("Photos")
                .FirstOrDefaultAsync(x=> 
                    x.NormalizedUserName == loginDto.Username.ToUpper()
                );

            if (user == null || user.UserName == null) return Unauthorized("Invalid credentials");

            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
            
            if (!result) return Unauthorized();
            
            return new UserDto {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string username) 
        {
            return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
        }
    }
}