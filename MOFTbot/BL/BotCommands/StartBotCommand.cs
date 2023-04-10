using MOFTbot.DAL;
using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MOFTbot.BL.BotCommands;

public class StartBotCommand : BotCommand
{
    public StartBotCommand(IAuthDal authDAL)
    {
        _authDAL = authDAL;
    }

    public override string Name => "/start";

    private IAuthDal _authDAL;


    public override async Task Execute(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var id = message.Chat.Id;

        var buttons = new KeyboardButton[][]
        {
            new[]{ new KeyboardButton("/myfollows") },
            new[]{ new KeyboardButton("/rating") },
            new[]{ new KeyboardButton("/balance") },
        };

        var keyboard = new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true
        };

        if(_authDAL.GetUserModelAsync(id).Result == null)
        {
            var name = message.From?.LastName ?? "noname";
            await _authDAL.CreateUserAsync(new UserModel() { Id = message.Chat.Id, NickName = name, Points = 100 });
            await SendAnswer(client, message, token, $"Welcome to MainFan, {name}!");
        }

        await client.SendTextMessageAsync(chatId: id,
                                            text: "ПОИХАЛИ",
                                            //hello, check help
                                            replyMarkup: keyboard,
                                            cancellationToken: token);
    }
}

