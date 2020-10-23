using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        // Get specific "UserLike"
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId); 

        // Get the "User" with collection associated "likes"
        Task<AppUser> GetUserWithLikes(int userId); 

        // Get "Likes" associated to a specific "User" - Both Made OR Received
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams); 
    }
}