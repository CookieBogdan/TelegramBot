using MOFTbot.DAL.Models;

namespace MOFTbot.DAL.Interfaces;

public interface IBetsDAL
{
    Task CreateBetAsync(BetMemberModel betMemberModel);
    Task<IEnumerable<BetMemberModel>> GetBetMembersAsync(int matchId);
}