using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using Telegram.Bot;

namespace MOFTbot.BL.BotCommands;

public class BotCommandWithMandatoryLogin : BotCommand
{
    public BotCommandWithMandatoryLogin(BotCommand command, IAuthDal authDal)
    {
        _command = command;
        _authDal = authDal;
    }

    public override string Name => _command.Name;

    private readonly BotCommand _command;
    private readonly IAuthDal _authDal;

    public override async Task Execute(ITelegramBotClient client, Telegram.Bot.Types.Message message, CancellationToken token)
    {
        if(await _authDal.GetUserModelAsync(message.Chat.Id) == null)
        {
            await SendAnswer(client, message, token, $"You need to login first with command /reg be4 using {Name}");
            return;
        }

        await _command.Execute(client, message, token);
    }

}
