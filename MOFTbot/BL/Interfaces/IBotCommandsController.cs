using MOFTbot.BL.BotCommands;

namespace MOFTbot.BL.Interfaces;

public interface IBotCommandsController
{
    BotCommand? GetBotCommand(Telegram.Bot.Types.Message msg);
}
