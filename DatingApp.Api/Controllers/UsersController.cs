

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.Dtos;
using DatingApp.Api.Helpers;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]ListParams listParams) 
        {

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await repo.GetUser(currentUserId);
            listParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(listParams.Gender)) {
                listParams.Gender = userFromRepo.Gender == "male" ? "female": "male";
            }

            var users = await repo.GetUsers(listParams);
            var usersToReturn = mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await repo.GetUser(id);
            var userToReturn = mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto) { 
            // Validate if the user is the same of the token, fot his case only can update my profile 
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var userFromRepo = await repo.GetUser(id);
            
            mapper.Map(userForUpdateDto, userFromRepo);
            
            if (await repo.SaveAll())
                return NoContent();
            
            throw new Exception($"Updating user {id} failed on save");

        }


        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId) 
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var like = await repo.GetLike(id, recipientId);

            if (like != null)
                return BadRequest("You already like this user");

            if (await repo.GetUser(recipientId) == null)
                return NotFound();

            like = new Like {
                LikerId = id,
                LikeeId = recipientId
            };

            repo.Add<Like>(like);

            if (await repo.SaveAll())
                return Ok();

            return BadRequest("Failed to like user");
        }

    }
}