using System.ComponentModel.DataAnnotations;

namespace MOFTbot.DAL.Models;

public class BetMemberModel
{
    public long UserId { get; set; }
    public int MatchId { get; set; }
    public int TeamId { get; set; }
    public int Value { get; set; }
}