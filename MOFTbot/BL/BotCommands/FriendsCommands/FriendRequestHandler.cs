namespace MOFTbot.BL.BotCommands.FriendsCommands;

public abstract class FriendRequestHandler : BotCommand
{
    protected bool TryGetFriendshipId(string? messageBody, out int friendshipId)
    {
        friendshipId = -1;

        try
        {
            friendshipId = int.Parse(messageBody);
        }
        catch
        {
            return false;
        }

        return true;
    }
}
