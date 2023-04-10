using MOFTbot.DAL.Interfaces;
using Telegram.Bot;

namespace MOFTbot.BL.BotCommands;

public class BecomeFanBotCommand : BotCommand
{
    public BecomeFanBotCommand(ITeamsDal teamsDal)
    {
        _teamsDal = teamsDal;
    }

    private ITeamsDal _teamsDal;

    public override string Name => "/fan";

    public async override Task Execute(ITelegramBotClient client, Telegram.Bot.Types.Message message, CancellationToken token)
    {
        var teamName = await GetBodyCommand(client, message, token);

        if (string.IsNullOrWhiteSpace(teamName))
        {
            await SendAnswer(client, message, token, $"Invalid name");
            return;
        }

        var teamModel = await _teamsDal.GetTeamModelAsync(teamName);

        if (teamModel == null)
        {
            await SendAnswer(client, message, token, $"Команда {teamName} не найдена. Проверьте имя и, если не ошиблись, добавьте команду вручную с помощью команды /add");
            return;
        }

        var userId = message.Chat.Id;
        if ((await _teamsDal.GetFavoriteTeamsAsync(userId)).Count() > 0)
        {
            await SendAnswer(client, message, token, $"You already have favorite team");
            return;
        }

        await _teamsDal.AddFavoriteTeamAsync(userId, teamModel.Id);
    }
}