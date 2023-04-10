using Telegram.Bot;
using Telegram.Bot.Types;

namespace MOFTbot.BL.BotCommands;

public abstract class BotCommand
{
    public abstract string Name { get; }
    public virtual string Help { get; } = "read help";

    public abstract Task Execute(ITelegramBotClient client, Message message, CancellationToken token);

    public bool Contains(Message message)
    {
        if (message.Text == null)
            return false;

        if (message.Text.Length < Name.Length)
            return false;

        string msg = message.Text.ToLower();
        return msg[..(Name.Length)] == Name.ToLower();
    }

    protected async Task SendAnswer(ITelegramBotClient client, Message message, CancellationToken token, string? msg = null)
    {
        if (msg == null)
            msg = Name.Substring(1);

        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: msg,
            cancellationToken: token);
    }

    protected async Task<string?> GetBodyCommand(ITelegramBotClient client, Message message, CancellationToken token, bool needToResponseClient = true)
    {
        if(message.Text!.Length > Name.Length)
        {
            return message.Text!.Substring(Name.Length + 1);
        }

        if (needToResponseClient)
        {
            await SendAnswer(client, message, token, Help);
        }

        return null;
    } 
}