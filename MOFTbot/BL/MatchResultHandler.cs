using MOFTbot.BL.Interfaces;
using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;

namespace MOFTbot.BL;

internal class MatchResultHandler
{
    public MatchResultHandler(ITeamsDal teamsDal, IBetsDAL betsDAL, IAuthDal authDAL, IUserNotificator userNotificator)
    {
        _teamsDal = teamsDal;
        _betsDAL = betsDAL;
        _authDAL = authDAL;
        _userNotificator = userNotificator;
    }

    private IBetsDAL _betsDAL;
    private ITeamsDal _teamsDal;
    private IAuthDal _authDAL;
    private IUserNotificator _userNotificator;

    private int FansRewardForTeamWinning = 10;

    public async void HandlerMatchStartingAsync(MatchResponse match)
    {
        var users = new List<UserModel>();

        users.AddRange(await _teamsDal.GetObserversAsync(match.Teams[0].Id));
        users.AddRange(await _teamsDal.GetObserversAsync(match.Teams[1].Id));

        users = users.DistinctBy(u => u.Id).ToList();

        foreach (var user in users)
            _userNotificator.Notify(user.Id, $"[{match.DateTime.Hour}:{match.DateTime.Minute}] {match.Teams[0].Name} vs {match.Teams[1].Name}");
    }

    public async void HandleMatchResultAsync(ResultResponse matchResult)
    {
        var obserevers = new List<Tuple<UserModel, TeamModel>>();

        var teams = new TeamModel?[] { await _teamsDal.GetTeamModelAsync(matchResult.WinnerTeamName), await _teamsDal.GetTeamModelAsync(matchResult.LooserTeamName) };
        var winner = teams[0];
        var looser = teams[1];

        foreach(var team in teams)
        {
            if(team == null) continue;
            obserevers.AddRange(_teamsDal.GetObserversAsync(team.Id).Result.Select(obs => new Tuple<UserModel, TeamModel>(obs, team)));
        }

        obserevers = obserevers.DistinctBy(obs => obs.Item1.Id).ToList();

        foreach(var obs in obserevers)
        {
            var message = (obs.Item2.Name == matchResult.WinnerTeamName) ?
                $"Team {obs.Item2.Name} won {matchResult.LooserTeamName}"
                :
                $"Team {obs.Item2.Name} lost to {matchResult.WinnerTeamName}";

            _userNotificator.Notify(obs.Item1.Id, message);
        }

        var bets = await _betsDAL.GetBetMembersAsync(matchResult.MatchId);

        var winnerTeamFans = (winner is not null)? await _teamsDal.GetFansAsync(winner.Id) : new List<UserModel>();

        foreach (var bet in bets)
        {
            if(bet.TeamId == winner?.Id)
            {
                int coefficient = winnerTeamFans.Any(f => f.Id == bet.UserId) ? 3 : 2;
                int reward = bet.Value * coefficient;
                RewardUser(bet.UserId, reward, $"Your bet on {winner.Name} has won, you have received a reward {reward}💰");
            }
            else
            {
                _userNotificator.Notify(bet.UserId, $"Your bet on {looser!.Name} lost, sad =(");
            }
        }

        if (winner != null)
        {
            RewardTeamFans(winnerTeamFans, FansRewardForTeamWinning, winner.Name);
        }
    }

    private void RewardTeamFans(IEnumerable<UserModel> fans, int reward, string teamName)
    {
        foreach(var user in fans)
        {
            RewardUser(user.Id, reward, $"U received an reward {reward}💰 for the victory of the {teamName}");
        }
    }

    private void RewardUser(long userId, int reward, string message)
    {
        if (reward <= 0) throw new Exception("Value of reward cant be less than zero");

        _authDAL.AddUserPointsAsync(userId, reward);
        _userNotificator.Notify(userId, message);
    }
}