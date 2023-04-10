using Telegram.Bot;
using Telegram.Bot.Types;

namespace MOFTbot.BL.Interfaces;

public interface IMessagesHandler
{
    Task Update(ITelegramBotClient client, Update update, CancellationToken token);
    void StartReceiving(ITelegramBotClient client);
}