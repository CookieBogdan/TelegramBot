using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MOFTbot.BL.BotCommands;

public class HelpBotCommand : BotCommand
{
    public override string Name => "/help";

    public override async Task Execute(ITelegramBotClient client, Message message, CancellationToken token)
    {
        await SendAnswer(client, message, token);
    }
}


