using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _autoMapper;
        public UsersController(IUserRepository userRepository, 
                                IMapper autoMapper)
        {
            _autoMapper = autoMapper;
            _userRepository = userRepository;
        }

        // api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var usersToReturn = await _userRepository.GetMembersAsync();

            return Ok(usersToReturn);
        }

        // api/users/paul
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var userToReturn = await _userRepository.GetMemberAsync(username);
            
            return userToReturn;
        }
    }
}