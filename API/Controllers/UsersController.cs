using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _autoMapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, 
                                IMapper autoMapper,
                                IPhotoService photoService)
        {
            _autoMapper = autoMapper;
            _userRepository = userRepository;
            _photoService = photoService;
        }

        // api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var usersToReturn = await _userRepository.GetMembersAsync();

            return Ok(usersToReturn);
        }

        // api/users/paul
        [HttpGet("{username}", Name="GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var userToReturn = await _userRepository.GetMemberAsync(username);
            
            return userToReturn;
        }

        // api/user/
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // Get user 
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUserNameAsync(username);
            
            _autoMapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        // Add Photo
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            // Get username
            var username = User.GetUsername();            
            // Get User and associated Photos
            var user = await _userRepository.GetUserByUserNameAsync(username);

            // Add photo to Cloudinary
            var result = await _photoService.AddPhotoAsync(file);

            // If no response or error from cloudinary then return badRequest
            if (result.Error != null) return BadRequest(result.Error.Message);

            // If success create new local photo from result returned from Cloudinary
            var photo = new Photo 
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            // If the user has no current photos then set this Photo as the main photo
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            //Add this Photo to the User-Photo object
            user.Photos.Add(photo);

            // If All OK save changes to user then return PhotoDto of the new Photo from Cloudinary
            if (await _userRepository.SaveAllAsync())
            {
                // return the route of where to get the object "GetUser" route associated above
                return CreatedAtRoute("GetUser", new {username = user.UserName}, _autoMapper.Map<PhotoDto>(photo));
            } 
            // Else if a problem return BadRequest
            return BadRequest("Problem adding photo");
        }

        // Set Main Photo
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<IActionResult> SetMainPhoto(int photoId) 
        {
            // get username of currentUser
            var username = User.GetUsername();
            // get User of currentUser
            var user = await _userRepository.GetUserByUserNameAsync(username);

            // get photo that matches photoId being passed in
            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);

            // check if photo IsMain is already set as Main
            if (photo.IsMain) return BadRequest("This is already your main photo.");

            // get current main photo and set isMain to false and set new Photo to IsMain true
            var currentMainPhoto = user.Photos.FirstOrDefault(photo => photo.IsMain == true);

            if (currentMainPhoto != null) currentMainPhoto.IsMain = false;
            photo.IsMain = true;
            // if successfull
            if (await _userRepository.SaveAllAsync()) return NoContent();
            // for all other events
            return BadRequest("Failed to set Main Photo");
        }

        // Delete photo
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            // Get user
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your main photo. Set a new main photo and try again.");
            if (photo.PublicId != null) 
            {
                // Delete photo to Cloudinary
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                // If no response or error from cloudinary then return badRequest
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            
            // If Delete successfull with Cloudinary - Remove from DB
            user.Photos.Remove(photo);
            if (await _userRepository.SaveAllAsync())
            {
                // return the route of where to get the object "GetUser" route associated above
                return Ok();
            } 
            // Else if a problem return BadRequest
            return BadRequest("Problem Deleting Photo");
        }
    }
}