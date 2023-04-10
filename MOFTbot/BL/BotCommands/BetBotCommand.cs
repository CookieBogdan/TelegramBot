using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MOFTbot.BL.BotCommands;

public class BetBotCommand : BotCommand
{
    public BetBotCommand(IMatchesRepository matchesRepository, IBetsDAL betsDAL, IAuthDal authDAL, ITeamsDal teamsDAL)
    {
        _matchesRepository = matchesRepository;
        _betDAL = betsDAL;
        _authDAL = authDAL;
        _teamsDAL = teamsDAL;
    }

    private IMatchesRepository _matchesRepository;
    private IBetsDAL _betDAL;
    private IAuthDal _authDAL;
    private ITeamsDal _teamsDAL;

    public override string Name => "/bet";

    public override async Task Execute(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var body = await GetBodyCommand(client, message, token);

        if (string.IsNullOrWhiteSpace(body))
        {
            await client.SendTextMessageAsync(message.Chat.Id, Help);
            return;
        }

        var parametrs = body.Split(' ');

        try
        {
            int value = int.Parse(parametrs[0]);

            if(value <= 0 || value > (_authDAL.GetUserModelAsync(message.Chat.Id).Result?.Points ?? value - 1))
            {
                throw new Exception("Insufficient funds");
            }

            switch (parametrs.Length)
            {
                case 1:
                    await ShowMatches(client, message, value);
                    break;
                case 2:
                    await ShowMatchMembers(client, message, value, GetMatch(int.Parse(parametrs[1])));
                    break;
                case 3:
                    var match = GetMatch(int.Parse(parametrs[1]));
                    if (match.DateTime.CompareTo(DateTime.Now) <= 0)
                    {
                        await SendAnswer(client, message, token, "match already started");
                    }
                    else
                    {
                        await MakeBet(
                            client: client,
                            message: message,
                            betValue: value,
                            match: match,
                            betableTeamId: int.Parse(parametrs[2]));
                    }
                    break;
                default:
                    break;
            }
        }
        catch(Exception ex)
        {
            await client.SendTextMessageAsync(message.Chat.Id, ex.Message);
            return;
        }

    }

    private MatchResponse GetMatch(int matchId)
    {
        var match = _matchesRepository.GetMatches().FirstOrDefault(m => m.Id == matchId);

        ArgumentNullException.ThrowIfNull(match, "match not founded");

        return match;
    }

    private TeamResponse GetMatchMember(int matchId, int teamId)
    {
        var match = GetMatch(matchId);

        var team = match.Teams.FirstOrDefault(team => team.Id == teamId);

        ArgumentNullException.ThrowIfNull(team, $"Team {teamId} does not participate match {matchId}");

        return team;
    }

    private async Task ShowMatches(ITelegramBotClient client, Message message, int betValue)
    {
        var matches = _matchesRepository.GetMatches();

        ArgumentNullException.ThrowIfNull(matches, "No matches =(  cs dead(((");
        
        if(matches.Count() == 0)
        {
            throw new Exception("No matches =(  cs dead(((");
        }

        var buttons = matches.Select(m => InlineKeyboardButton.WithCallbackData(
                text: $"[{m.DateTime.Hour}:{(m.DateTime.Minute == 0 ? "00" : m.DateTime.Minute)}] {m.Teams[0].Name} vs {m.Teams[1].Name}",
                callbackData: $"/bet {betValue} {m.Id}"))
            .Chunk(1);

        await client.SendTextMessageAsync(message.Chat.Id, "Choose match", replyMarkup: new InlineKeyboardMarkup(buttons));
    }

    private async Task ShowMatchMembers(ITelegramBotClient client, Message message, int betValue, MatchResponse match)
    {
        var team1 = match.Teams[0]!;
        var team2 = match.Teams[1]!;

        var button1 = InlineKeyboardButton.WithCallbackData(
            text: team1.Name,
            callbackData: $"/bet {betValue} {match.Id} {team1.Id}");

        var button2 = InlineKeyboardButton.WithCallbackData(
            text: team2.Name,
            callbackData: $"/bet {betValue} {match.Id} {team2.Id}");

        var keyboard = new InlineKeyboardMarkup(new[] { button1, button2 });

        try
        {
            await client.EditMessageTextAsync(message.Chat.Id, message.MessageId, "Choose team", replyMarkup: keyboard);
        }
        catch
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Choose team", replyMarkup: keyboard);
        }
        
    }

    private async Task MakeBet(ITelegramBotClient client, Message message, int betValue, MatchResponse match, int betableTeamId)
    {
        var team = match.Teams.FirstOrDefault(t => t.Id == betableTeamId);

        ArgumentNullException.ThrowIfNull(team, $"Trying to bet on team {betableTeamId} that doesnt play match {match.Id}");

        if(_teamsDAL.GetTeamModelAsync(betableTeamId).Result == null)
        {
            await _teamsDAL.AddTeamAsync(new TeamModel() { Id = betableTeamId, Name = team.Name });
        }

        await _betDAL.CreateBetAsync(new BetMemberModel() 
        { 
            MatchId = match.Id,
            TeamId = betableTeamId,
            UserId = message.Chat.Id, 
            Value = betValue
        });

        try
        {
            await client.EditMessageTextAsync(message.Chat.Id, message.MessageId, $"Вы поставили {betValue} на команду {team.Name}");
        }
        catch
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"Вы поставили {betValue} на команду {team.Name} на матч {match.Teams[0].Name} vs {match.Teams[1].Name}");
        }        
    }
}