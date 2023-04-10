using MOFTbot.BL.Interfaces;
using Telegram.Bot;

namespace MOFTbot.BL;

internal class Notificator : IUserNotificator
{
    public Notificator(ITelegramBotClient client)
    {
        _client = client;
    }

    private ITelegramBotClient _client;

    public void Notify(long userId, string message)
    {
        try
        {
            _client.SendTextMessageAsync(userId, message);
        }
        catch
        {
            // Logger
        }
    }
}