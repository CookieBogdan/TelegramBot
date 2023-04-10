using MOFTbot.DAL.Interfaces;

namespace MOFTbot.DAL;

public class MatchesRepository : IMatchesRepository
{
    public MatchesRepository()
    {
        _matchesRepository = new List<MatchResponse>();
    }

    private readonly List<MatchResponse> _matchesRepository;

    public void AddMatches(IEnumerable<MatchResponse> matches)
    {
        _matchesRepository.AddRange(matches);
    }

    public IEnumerable<MatchResponse> GetMatches()
    {
        return _matchesRepository;
    }

    public void RemoveMatches(IEnumerable<MatchResponse> matches)
    {
        matches.Select(m => _matchesRepository.Remove(m));
    }
}