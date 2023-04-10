using System;
using MOFTbot.DAL.Interfaces;
using Telegram.Bot;

namespace MOFTbot.BL.BotCommands;

public class BalanceBotCommand : BotCommand
{
    public BalanceBotCommand(IAuthDal authDal)
    {
        _authDal = authDal;
    }

    public override string Name => "/balance";
    public override string Help => "help with /balance";

    private IAuthDal _authDal;

    public override async Task Execute(ITelegramBotClient client, Telegram.Bot.Types.Message message, CancellationToken token)
    {
        var userModel = await _authDal.GetUserModelAsync(message.Chat.Id);
        if (userModel == null)
        {
            //logger
            Console.WriteLine($"{message.Chat.Id} not found in db");
            await SendAnswer(client, message, token, "problem with ur id");
        }
        else
        {
            await SendAnswer(client, message, token, $"Your balance: {userModel.Points}💰");
        }
    }
}
