namespace Neru.Models
{
    public class UserRemembrance : Entity
    {
        public string UserId { get; set; }
        public string? UserNickname { get; set; }
        public string? UserFavouriteWord { get; set; }
        public int UserHP { get; set; }
        public float UserLove { get; set; }
        public DateOnly UserBirthday { get; set; }
    }
}
