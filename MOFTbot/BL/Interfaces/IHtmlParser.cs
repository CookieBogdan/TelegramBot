using System;
namespace MOFTbot.BL.Interfaces
{
	public interface IHtmlParser
	{
        Task<string?> GetTitle(string teamUrl);
        Task<IEnumerable<MatchResponse>> GetTodayMatchesAsync();
        Task<IEnumerable<MatchResponse>> GetMatches(int teamId);
        Task<IEnumerable<TeamResponse>> GetTop20Teams();
        Task<IEnumerable<ResultResponse>> GetMatchResults(int countMatches);
    }
}

