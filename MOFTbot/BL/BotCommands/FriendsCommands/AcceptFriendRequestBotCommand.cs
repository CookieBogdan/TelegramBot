using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using Telegram.Bot;

namespace MOFTbot.BL.BotCommands.FriendsCommands;

public class AcceptFriendRequestBotCommand : FriendRequestHandler
{
    public AcceptFriendRequestBotCommand(IFriendsDAL friendsDAL)
    {
        _friendsDAL = friendsDAL;
    }

    public override string Name => "/acceptrequest";

    private IFriendsDAL _friendsDAL;

    public override async Task Execute(ITelegramBotClient client, Telegram.Bot.Types.Message message, CancellationToken token)
    {
        if (!TryGetFriendshipId(await GetBodyCommand(client, message, token), out int friendshipId))
        {
            return;
        }

        var friendship = await _friendsDAL.GetFriendshipAsync(friendshipId);

        if (friendship == null || friendship.Status != (int)FriendShipStatus.Waiting || (friendship.SecondUserId != message.Chat.Id)) return;

        await _friendsDAL.UpdateFriendshipStatusAsync(friendshipId, DAL.Models.FriendShipStatus.Accepted);
        await client.SendTextMessageAsync(friendship.FirstUserId, $"User {friendship.SecondUserId} accepted your request");

        try
        {
            await client.DeleteMessageAsync(message.Chat.Id, message.MessageId);
        }
        catch
        {

        }
    }
}
