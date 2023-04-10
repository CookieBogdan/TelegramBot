using MOFTbot.DAL.Interfaces;
using Telegram.Bot;
using MOFTbot.DAL.Models;

namespace MOFTbot.BL.BotCommands;

public class SetNicknameBotCommand : BotCommand
{
    public SetNicknameBotCommand(IAuthDal authDal)
    {
        _authDal = authDal;
    }

    public override string Name => "/myname";
    private IAuthDal _authDal;

    public override async Task Execute(ITelegramBotClient client, Telegram.Bot.Types.Message message, CancellationToken token)
    {
        var nickname = await GetBodyCommand(client, message, token);
        
        if(string.IsNullOrWhiteSpace(nickname))
        {
            await SendAnswer(client, message, token, "nickname is invalid");
            return;
        }

        await _authDal.UpdateUserNicknameAsync(message.Chat.Id, nickname);
        await SendAnswer(client, message, token, $"Имя {nickname} идет тебе больше, мне нрав");
    }
}