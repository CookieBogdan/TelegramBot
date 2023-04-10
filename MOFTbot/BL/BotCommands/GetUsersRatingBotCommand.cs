using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MOFTbot.BL.BotCommands;

public class GetUsersRatingBotCommand : BotCommand
{
    public GetUsersRatingBotCommand(IAuthDal authDAL, ITeamsDal teamsDAL)
    {
        _authDAL = authDAL;
        _teamsDAL = teamsDAL;
    }

    public override string Name => "/rating";
    
    private const int TOPCOUNT = 10;

    private IAuthDal _authDAL;
    private ITeamsDal _teamsDAL;

    public override async Task Execute(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var body = await GetBodyCommand(client, message, token, false);
        var sb = new StringBuilder();

        IEnumerable<UserModel> users;

        if (string.IsNullOrWhiteSpace(body))
        {
            var userFavoriteTeams = await _teamsDAL.GetFavoriteTeamsAsync(message.Chat.Id);

            var buttons = new List<List<InlineKeyboardButton>>();

            foreach (var team in userFavoriteTeams)
            {
                var button = new InlineKeyboardButton(team.Name);
                button.CallbackData = $"/rating {team.Name}";
                buttons.Add(new List<InlineKeyboardButton>() { button });
            }

            var worldRateButton = new InlineKeyboardButton("World Rating 🏆");
            worldRateButton.CallbackData = "/rating @all";
            buttons.Add(new List<InlineKeyboardButton>() { worldRateButton });

            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);

            await client.SendTextMessageAsync(message.Chat.Id, "Выберете команду", replyMarkup: keyboard);
            return;
        }

        if (body == "@all")
        {
            users = await _authDAL.GetTopNUsers(TOPCOUNT);
            sb.Append($"WORLD SCOREBOARD\n");
        }
        else
        {
            var team = await _teamsDAL.GetTeamModelAsync(body);

            if (team == null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Team not found");
                return;
            }

            users = _teamsDAL.GetFansAsync(team.Id).Result.OrderBy(fan => fan.Points);

            sb.Append(users.Count() == 0 ? $"У {team.Name} еще нет фанатов обидно за этих добряков конечно но не очень" : $"Top fans of {team.Name}\n");
        }

        int i = 0;
        foreach (var user in users)
        {
            sb.Append($"{i + 1}) {user.NickName} \n");

            if (i >= TOPCOUNT) break;
        }

        try
        {
            await client.EditMessageTextAsync(message.Chat.Id, message.MessageId, sb.ToString());
        }
        catch 
        {
            await client.SendTextMessageAsync(message.Chat.Id, sb.ToString());
        }
        
    }
}