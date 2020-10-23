namespace API.Entities
{
    public class UserLike
    {
        // The user who is giving "likes"
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }

        // The user who is recieving the "likes"
        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }

    }
}