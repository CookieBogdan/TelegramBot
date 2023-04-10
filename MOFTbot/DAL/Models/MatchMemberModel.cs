using System.ComponentModel.DataAnnotations;

namespace MOFTbot.DAL.Models;

public class MatchMemberModel
{
    [Key]
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int MatchId { get; set; }
}