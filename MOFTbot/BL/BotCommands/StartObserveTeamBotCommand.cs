using MOFTbot.DAL.Interfaces;
using Telegram.Bot;

namespace MOFTbot.BL.BotCommands;

public class StartObserveTeamBotCommand : BotCommand
{
    public StartObserveTeamBotCommand(ITeamsDal teamsDal)
    {
        _teamsDal = teamsDal;
    }

    private ITeamsDal _teamsDal;

    public override string Name => "/follow";

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

        var observers = await _teamsDal.GetObserversAsync(teamModel.Id);

        if (!observers.Any(obs => obs.Id == message.Chat.Id))
        {
            await _teamsDal.AddObservedTeamAsync(message.Chat.Id, teamModel.Id);
            await SendAnswer(client, message, token, $"You started followed on {teamModel.Name}");
        }
        else
        {
            await SendAnswer(client, message, token, $"You already followed on {teamModel.Name}");
        }
        
    }
}
