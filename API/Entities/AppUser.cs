using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }
        
        // Likes Received by User
        public ICollection<UserLike> LikesReceived { get; set; }  // LikedByUsers
        // Likes sent by User
        public ICollection<UserLike> LikesSent { get; set; } // LikedUsers

        // Messages Received by User
        public ICollection<Message> MessagesReceived {get; set; }
        // Messages Sent by User
        public ICollection<Message> MessagesSent {get; set; }
        
        // Identity - UserRoles
        public ICollection<AppUserRole> UserRoles { get; set; }
        
        // public int GetAge()
        // {
        //     return DateOfBirth.CalculateAge();
        // }
        
    }
}