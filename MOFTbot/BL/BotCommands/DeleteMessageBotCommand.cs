using Telegram.Bot;

namespace MOFTbot.BL.BotCommands;

public class DeleteMessageBotCommand : BotCommand
{
    public override string Name => "/deletemessage";

    public override async Task Execute(ITelegramBotClient client, Telegram.Bot.Types.Message message, CancellationToken token)
    {
        var body = await GetBodyCommand(client, message, token, false);

        int messageId;

        try
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                messageId = message.MessageId;
            }
            else
            {
                messageId = int.Parse(body);
            }

            await client.DeleteMessageAsync(message.Chat.Id, messageId);
        }
        catch(Exception ex)
        {
            return;
        }
    }
}
