using System.Security.Claims;
using Api.Dtos;
using Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseApiController
    {
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() 
        {
            var users = await userRepository.GetMembersAsync();
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username) 
        {
            var user = await userRepository.GetMemberAsync(username);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) 
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier);
            if (username == null) return BadRequest("No username found in token");

            var user = await userRepository.GetUserByUsernameAsync(username.Value);
            if (user == null) return BadRequest("Could not find user");

            mapper.Map(memberUpdateDto, user);

            if (await userRepository.SaveAllSync()) return NoContent();

            return BadRequest("Failed to update the user");
        }
    }
}