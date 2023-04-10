using MOFTbot.BL.Interfaces;
using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MOFTbot.BL.BotCommands;

public class TopBotCommand : BotCommand
{
    public TopBotCommand(IHtmlParser htmlParser, ITeamsDal teamsDAL)
    {
        _htmlParser = htmlParser;
        _teamsDAL = teamsDAL;
    }

    public override string Name => "/topteams";
    public override string Help => "help with /topteams";

    private IHtmlParser _htmlParser;
    private ITeamsDal _teamsDAL;

    public override async Task Execute(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var teams = (await _htmlParser.GetTop20Teams()).Select(t => new TeamModel() { Id = t.Id, Name = t.Name});

        var buttons = teams.GetFollowingButtons(await _teamsDAL.GetObservedTeamsAsync(message.Chat.Id));

        await client.SendTextMessageAsync
            (
                chatId: message.Chat.Id,
                text: "Press 🔔 to follow",
                cancellationToken: token,
                replyMarkup: buttons
            );
    }
}