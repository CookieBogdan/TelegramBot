using MOFTbot.DAL.Interfaces;
using Telegram.Bot;

namespace MOFTbot.BL.BotCommands.FriendsCommands;

public class RejectFriendRequestBotCommand : DeleteFriendshipBotCommand
{
    public RejectFriendRequestBotCommand(IFriendsDAL friendsDAL) : base(friendsDAL)
    {
    }

    public override string Name => "/rejectrequest";

    public override async Task Execute(ITelegramBotClient client, Telegram.Bot.Types.Message message, CancellationToken token)
    {
        await DeleteFriendship(await GetBodyCommand(client, message, token), message);

        try
        {
            await client.EditMessageTextAsync(message.Chat.Id, message.MessageId, "Request rejected");
        }
        catch
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Request rejected");
        }
        
    }
}