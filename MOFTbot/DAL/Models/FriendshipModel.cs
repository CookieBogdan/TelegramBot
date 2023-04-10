using System.ComponentModel.DataAnnotations;

namespace MOFTbot.DAL.Models;

public class FriendshipModel
{
    public int? Id { get; set; }
    public long FirstUserId { get; set; }
    public long SecondUserId { get; set; }
    public int Status { get; set; }
}