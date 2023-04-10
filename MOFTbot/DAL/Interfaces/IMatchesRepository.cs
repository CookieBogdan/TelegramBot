namespace MOFTbot.DAL.Interfaces;

public interface IMatchesRepository
{
    IEnumerable<MatchResponse> GetMatches();
    void AddMatches(IEnumerable<MatchResponse> matches);
    void RemoveMatches(IEnumerable<MatchResponse> matches);
}