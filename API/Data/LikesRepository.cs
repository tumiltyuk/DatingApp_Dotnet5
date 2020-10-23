using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            // get ALL Users
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            // get ALL Likes
            var likes = _context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked") // User Sent likes
            {
                // return Likes (UserLike) where userId is the source 
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                // return Users (AppUser) where the User is the Likes LikedUser
                users = likes.Select(like => like.LikedUser);
            }

            else if (likesParams.Predicate == "likedBy") // User received likes
            {
                // return Likes (UserLike) where userId is the recipient "LikedUserId" 
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                // return Users (AppUser) where the User is the SourceUser
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                Id = user.Id,
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City

            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, 
                likesParams.PageNumber, likesParams.PageSize);
        }

        // Get User with collection of Likes made by user
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikesMadeByUser)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}