using MOFTbot.DAL.Models;

namespace MOFTbot.DAL.Interfaces;

public interface ITeamsDal
{
    Task AddTeamAsync(TeamModel model);
    Task AddFavoriteTeamAsync(long userId, int teamId);
    Task AddObservedTeamAsync(long userId, int teamId);

    Task RemoveObservedTeamAsync(long userId, int teamId);
     
    Task<TeamModel?> GetTeamModelAsync(int id);
    Task<TeamModel?> GetTeamModelAsync(string name);
    Task<IEnumerable<TeamModel>> GetObservedTeamsAsync(long userId);
    Task<IEnumerable<TeamModel>> GetFavoriteTeamsAsync(long userId);
    Task<IEnumerable<UserModel>> GetFansAsync(int teamId);
    Task<IEnumerable<UserModel>> GetObserversAsync(int teamId);
    Task<IEnumerable<UserModel>> GetObserversAsync(string teamName);
}