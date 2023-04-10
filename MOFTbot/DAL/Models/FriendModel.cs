namespace MOFTbot.DAL.Models;

public class FriendModel
{
    public int FriendshipId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;

}
