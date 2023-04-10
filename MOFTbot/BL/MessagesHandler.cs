using MOFTbot.BL.Interfaces;
using Telegram.Bot;
using static System.Net.Mime.MediaTypeNames;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MOFTbot.BL;

public class MessagesHandler : IMessagesHandler
{
    public MessagesHandler(IBotCommandsController botCommandsController)
    {
        _botCommandsController = botCommandsController;
    }

    private readonly IBotCommandsController _botCommandsController;

    public void StartReceiving(ITelegramBotClient client)
    {
        client.StartReceiving(Update, Error);
    }

    public async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {
        switch (update.Type)
        {
            case UpdateType.CallbackQuery:
                var msg = update.CallbackQuery!.Message;
                msg!.Text = update.CallbackQuery.Data;
                await MessageCommand(msg);
                break;

            case UpdateType.Message:
                await MessageCommand(update.Message!);
                break;

            default:
                break;
        }
        async Task MessageCommand(Message msg)
        {
            var command = _botCommandsController.GetBotCommand(msg);
            if (command != null)
            {
                await command.Execute(client, msg, token);
            }
            else
            {
                await client.SendTextMessageAsync(msg.Chat.Id, "command not found");
            }
        }
    }

    public static async Task Error(ITelegramBotClient client,
    Exception exception,
    CancellationToken token)
    {
        Console.WriteLine(exception.Message);
    }
}