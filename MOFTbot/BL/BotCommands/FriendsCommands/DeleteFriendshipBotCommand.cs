
using MOFTbot.DAL.Interfaces;
using MySqlX.XDevAPI;
using Telegram.Bot.Types;

namespace MOFTbot.BL.BotCommands.FriendsCommands;

public abstract class DeleteFriendshipBotCommand : FriendRequestHandler
{
    public DeleteFriendshipBotCommand(IFriendsDAL friendsDAL)
    {
        _friendsDAL = friendsDAL;
    }

    protected IFriendsDAL _friendsDAL;

    protected async Task DeleteFriendship(string? messageBody, Message message)
    {
        if (!TryGetFriendshipId(messageBody, out int friendshipId))
        {
            return;
        }

        var friendship = await _friendsDAL.GetFriendshipAsync(friendshipId);
        var userId = message.Chat.Id;

        if (friendship == null || (userId != friendship.FirstUserId && userId != friendship.SecondUserId)) return;

        await _friendsDAL.DeleteFriendshipAsync(friendshipId);
    }
}
