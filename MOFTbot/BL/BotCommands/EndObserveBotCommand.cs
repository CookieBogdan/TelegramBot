using MOFTbot.DAL;
using MOFTbot.DAL.Interfaces;
using System;
using System.Net;
using System.Text.RegularExpressions;
using Telegram.Bot;

namespace MOFTbot.BL.BotCommands;

public class EndObserveBotCommand : BotCommand
{
    public EndObserveBotCommand(ITeamsDal teamsDal)
    {
        _teamsDal = teamsDal;
    }

    public override string Name => "/unfollow";
    public override string Help => "help with /unfollow";

    private ITeamsDal _teamsDal;

    public override async Task Execute(ITelegramBotClient client, Telegram.Bot.Types.Message message, CancellationToken token)
    {
        var teamName = await GetBodyCommand(client, message, token);

        if (string.IsNullOrWhiteSpace(teamName))
        {
            await SendAnswer(client, message, token, "Invalid name");
            return;
        }

        teamName = teamName.ToLower();

        var team = _teamsDal.GetObservedTeamsAsync(message.Chat.Id).Result.FirstOrDefault(t => t.Name == teamName);
        if (team is null)
        { 
            await SendAnswer(client, message, token, $"you are not following {teamName}");
            return;
        }

        await _teamsDal.RemoveObservedTeamAsync(message.Chat.Id, team.Id);
        await SendAnswer(client, message, token, $"you successfully unfollowed from {teamName}");
    }
}

