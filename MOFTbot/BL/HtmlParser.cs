using MOFTbot.BL.Interfaces;

using AngleSharp;

namespace MOFTbot;

public record TeamResponse(int Id, string Name);
public record MatchResponse(int Id, string Url, DateTime DateTime, TeamResponse[] Teams);
public record ResultResponse(int MatchId, string WinnerTeamName, string LooserTeamName);

public class HtmlParser : IHtmlParser
{
    private readonly IBrowsingContext _context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());

    public async Task<string?> GetTitle(string teamUrl)
    {
        var document = await _context.OpenAsync(teamUrl);
        return document.QuerySelector(".profile-team-name")?.TextContent;
    }

    public async Task<IEnumerable<MatchResponse>> GetTodayMatchesAsync()
    {
        var matches = new List<MatchResponse>();
        var document = await _context.OpenAsync("https://www.hltv.org/matches");

        var matchesSection = document.QuerySelector(".upcomingMatchesSection")!;

        var day = int.Parse(matchesSection.QuerySelector(".matchDayHeadline")!.TextContent.Split('-').Last());

        foreach (var upcomingMatchSection in matchesSection.QuerySelectorAll(".upcomingMatch"))
        {
            var matchSection = upcomingMatchSection.QuerySelector(".match")!;

            string href = matchSection.GetAttribute("href")!;
            int matchId = int.Parse(href.Split('/')[2]);
            string matchUrl = "https://www.hltv.org/matches/" + matchId + "/1";
            int[] timeArr = matchSection.QuerySelector(".matchTime")!.TextContent.Split(":").Select(s => int.Parse(s)).ToArray();

            DateTime dateTime = new DateTime(
                year: DateTime.Now.Year,
                month: DateTime.Now.Month,
                day: day,
                hour: timeArr[0],
                minute: timeArr[1],
                second: 0);

            dateTime = dateTime.AddHours(2);

            int team1Id = int.Parse(upcomingMatchSection.GetAttribute("team1") ?? "-1");
            int team2Id = int.Parse(upcomingMatchSection.GetAttribute("team2") ?? "-1");

            if (team1Id == -1 || team2Id == -1)
                continue;

            var matchTeamNamesSection = matchSection.QuerySelectorAll(".matchTeamName");

            string team1Name = matchTeamNamesSection[0].TextContent;
            string team2Name = matchTeamNamesSection[1].TextContent;

            matches.Add(new MatchResponse(matchId, matchUrl, dateTime, new[]
            {
                new TeamResponse(team1Id, team1Name),
                new TeamResponse(team2Id, team2Name)
            }));
        }

        return matches;
    }

    public async Task<IEnumerable<MatchResponse>> GetMatches(int teamId)
    {
        var matches = new List<MatchResponse>();
        var document = await _context.OpenAsync($"https://www.hltv.org/team/{teamId}/{1}#tab-matchesBox");

        var htmlTables = document.QuerySelectorAll(".table-container.match-table");
        if (htmlTables.Length > 1)
        {
            var htmlMatches = htmlTables[0].QuerySelectorAll(".team-row");

            foreach (var match in htmlMatches)
            {
                string matchUrl = "https://www.hltv.org" +
                    match.QuerySelector(".matchpage-button")!.GetAttribute("href");

                matches.Add(await GetInfoAbouMatch(matchUrl));
            }
        }

        return matches;
    }

    private async Task<MatchResponse> GetInfoAbouMatch(string matchUrl)
    {
        var document = await _context.OpenAsync(matchUrl);
        var dateAndTime = document.QuerySelector(".dropdownTimeAndEvent");

        var time = dateAndTime!.QuerySelector(".time")!.TextContent;
        var date = dateAndTime!.QuerySelector(".date")!.TextContent;

        var hoursMinutes = time.Split(':').Select(s => int.Parse(s)).ToArray();
        var day = int.Parse(date.Split(' ')[0]);

        var dateTime = new DateTime(
            year: DateTime.Now.Year,
            month: DateTime.Now.Day > day ? DateTime.Now.Month + 1 : DateTime.Now.Month,
            day: day,
            hour: hoursMinutes[0] + 2,
            minute: hoursMinutes[1],
            second: 0
            );

        var htmlTeams = document.QuerySelector(".standard-box.teamsBox")!.QuerySelectorAll(".team");

        var teams = htmlTeams.Select(t => new TeamResponse(
            int.Parse(t.QuerySelector("a")!.GetAttribute("href")!.Split('/')[2]),
            t.QuerySelector(".teamName")!.TextContent
            )).ToArray();

        var matchId = int.Parse(matchUrl.Substring(30).Split('/')[0]);

        return new MatchResponse(matchId, matchUrl, dateTime, teams);
    }

    public async Task<IEnumerable<TeamResponse>> GetTop20Teams()
    {
        var teams = new List<TeamResponse>();

        var document = await _context.OpenAsync("https://www.hltv.org/ranking/teams");

        foreach(var teamPanel in document.QuerySelectorAll(".bg-holder").SkipLast(10))
        {
            var teamName = teamPanel.QuerySelector(".name")!.TextContent;
            var urlTeam = teamPanel.QuerySelector(".moreLink")!.GetAttribute("href");
            var id = int.Parse(urlTeam!.Split('/')[2]);
            teams.Add(new TeamResponse(id, teamName));
        }
        return teams;
    }

    public async Task<IEnumerable<ResultResponse>> GetMatchResults(int countMatches)
    {
        var list = new List<ResultResponse>();
        var document = await _context.OpenAsync("https://www.hltv.org/results");

        var results = document.QuerySelectorAll(".results-all")!;
        var result = results.Length > 1 ? results[results.Length - 1] : results[0];
        var allResults = result.QuerySelectorAll(".a-reset")!;
        foreach (var matchResultSection in Enumerable.Range(0, countMatches).Select(i => allResults[i]))
        {
            int matchId = int.Parse(matchResultSection.GetAttribute("href")!.Split('/')[2]);

            string[] teams = matchResultSection.QuerySelectorAll(".team").Select(m => m.TextContent).ToArray();
            string winnerName = matchResultSection.QuerySelector(".team-won")!.TextContent;
            string looserName = teams.First(t => t != winnerName);

            list.Add(new ResultResponse(matchId, winnerName, looserName));
        }

        return list;
    }
}