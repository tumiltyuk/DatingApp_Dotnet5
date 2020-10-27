using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username) 
        {
            var sourceUserId = User.GetUserId();

            // get User we are liking
            var likedUser = await _userRepository.GetUserByUserNameAsync(username);
            // get SourceUser with associated Likes
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            // check if liked user exists
            if (likedUser == null) return NotFound();
            // prevent user from liking themselves
            if (sourceUser.Id == likedUser.Id) return BadRequest("You cannot like yourself.");

            // check if UserLike already exists
            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null) return BadRequest("You have already liked this user.");

            // if UserLike does not already exist then create a new UserLike
            userLike = new UserLike
            {
                SourceUserId = sourceUserId,    // set SourceUserId
                LikedUserId = likedUser.Id      // set LikedUserId
                // other fields will be null
            };

            // add Like to SourceUser
            sourceUser.LikesSent.Add(userLike);

            // save changes
            if (await _userRepository.SaveAllAsync()) return Ok();

            // if unsucessfull
            return BadRequest("Failed to Like user.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await _likesRepository.GetUserLikes(likesParams);
            // Update Pagination to response
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

    }
}