using System;
using System.Timers;
using MOFTbot.BL.Interfaces;
using MOFTbot.DAL.Interfaces;

using Timer = System.Timers.Timer;

namespace MOFTbot.BL;

public class HltvObserver
{
    public HltvObserver(
        IMatchesRepository matchesRepository,
        IHtmlParser htmlParser,
        int minutes)
    {
        _matchesRepository = matchesRepository;
        _intervalInMinutes = minutes;
        _htmlParser = htmlParser;
    }

    public event Action<ResultResponse>? OnMatchFinished;
    public event Action<MatchResponse>? OnMatchStarting;

    private IMatchesRepository _matchesRepository;
    private IHtmlParser _htmlParser;
    private Timer _timer = default!;

    private readonly int _intervalInMinutes;

    public void Start()
    {
        Update();
        SetTimer(_intervalInMinutes);
    }
    private void SetTimer(int minutes)
    {
        _timer = new Timer(new TimeSpan(0, minutes, 0));
        _timer.Elapsed += Update;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    private async void Update(object? source = null, ElapsedEventArgs? e = null)
    {
        Console.WriteLine("update");

        var allTodayMatches = await _htmlParser.GetTodayMatchesAsync();

        var finishedMatches = await _htmlParser.GetMatchResults(15);

        var removeList = new List<MatchResponse>();
        foreach(var match in finishedMatches)
        {
            var finishedMatch = allTodayMatches.FirstOrDefault(m => m.Id == match.MatchId);
            if(finishedMatch != null)
            {
                OnMatchFinished?.Invoke(match);
                removeList.Add(finishedMatch);
            }            
        }
        _matchesRepository.RemoveMatches(removeList);

        var cacheMatches = _matchesRepository.GetMatches();

        var newMatches = allTodayMatches.Where(m => !cacheMatches.Any(c => c.Id == m.Id));

        _matchesRepository.AddMatches(newMatches);

        var a = _matchesRepository.GetMatches();


        var startingMatches = a
            .Where(m =>
                m.DateTime.CompareTo(DateTime.Now) >= 0 &&
                m.DateTime.Subtract(DateTime.Now) <= new TimeSpan(0, _intervalInMinutes, 0)).ToArray();

        foreach (var match in startingMatches)
        {
            OnMatchStarting?.Invoke(match);
        }
    }
}