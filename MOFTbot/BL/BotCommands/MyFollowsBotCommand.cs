using MOFTbot.DAL.Interfaces;
using System;
using System.Text;
using Telegram.Bot;

namespace MOFTbot.BL.BotCommands;

public class MyFollowsBotCommand : BotCommand
{
    public MyFollowsBotCommand(ITeamsDal teamsDal)
    {
        _teamsDal = teamsDal;
    }

    public override string Name => "/myfollows";

    private ITeamsDal _teamsDal;

    public override async Task Execute(ITelegramBotClient client, Telegram.Bot.Types.Message message, CancellationToken token)
    {
        var follows = await _teamsDal.GetObservedTeamsAsync(message.Chat.Id);

        var buttons = follows.GetFollowingButtons(follows);

        await client.SendTextMessageAsync(message.Chat.Id, "Your follows", replyMarkup: buttons);
    }
}