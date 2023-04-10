using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text.RegularExpressions;
using AngleSharp;
using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using MOFTbot.BL.Interfaces;

namespace MOFTbot.BL.BotCommands;

public class AddTeamBotCommand : BotCommand
{
    public AddTeamBotCommand(ITeamsDal teamsDal, IHtmlParser htmlParser)
    {
        _teamsDal = teamsDal;
        _htmlParser = htmlParser;
    }

    public override string Name => "/addteam";
    public override string Help => "help with /add";

    private ITeamsDal _teamsDal;
    private IHtmlParser _htmlParser;

    public override async Task Execute(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var userId = message.Chat.Id;  

        //check url
        var url = await GetBodyCommand(client, message, token);
        if (url == null) return;

        if (!Regex.IsMatch(url, @"https:\/\/www\.hltv\.org\/team\/\d+\/[a-z-0-9]+", RegexOptions.IgnoreCase))
        {
            //bad link [TODO]
            await SendAnswer(client, message, token, "bad regex link");
            return; 
        }

        var startStr = "https://www.hltv.org/team";
        var idAndTitle = url.Substring(url.IndexOf(startStr) + startStr.Length + 1);
        var teamId = int.Parse(idAndTitle.Substring(0, idAndTitle.LastIndexOf('/')));

        //parsing
        var title = await _htmlParser.GetTitle(url);

        //bad link [TODO]
        if (title == null)
        {
            await SendAnswer(client, message, token, "bad link");
            return;
        }

        var team = await _teamsDal.GetTeamModelAsync(teamId);
        if (team == null)
        {
            team = new TeamModel() { Id = teamId, Name = title.ToLower() };
            await _teamsDal.AddTeamAsync(team);
            await client.SendTextMessageAsync(message.Chat.Id, $"{title} added", replyMarkup: team.GetFollowingButton(new List<TeamModel>()));
        }
        else
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"team {title} has already added in base", 
                replyMarkup: team.GetFollowingButton(await _teamsDal.GetObservedTeamsAsync(userId)));
        }

    }
}