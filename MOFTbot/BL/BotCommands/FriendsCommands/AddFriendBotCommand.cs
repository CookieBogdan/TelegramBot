using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MOFTbot.BL.BotCommands.FriendsCommands;

public class AddFriendBotCommand : BotCommand
{
    public AddFriendBotCommand(IFriendsDAL friendsDAL, IAuthDal usersDAL)
    {
        _usersDAL = usersDAL;
        _friendsDAL = friendsDAL;
    }

    public override string Name => "/addfriend";

    private IFriendsDAL _friendsDAL;
    private IAuthDal _usersDAL;

    public override async Task Execute(ITelegramBotClient client, Telegram.Bot.Types.Message message, CancellationToken token)
    {
        var body = await GetBodyCommand(client, message, token);

        if (string.IsNullOrWhiteSpace(body))
        {
            return;
        }

        long friendId;

        try
        {
            friendId = int.Parse(body);
        }
        catch
        {
            return;
        }

        var userId = message.Chat.Id;

        if (userId == friendId)
        {
            await SendAnswer(client, message, token, "Being friends with yourself is a bit boring =( but we don't judge");
            return;
        }

        if (_usersDAL.GetUserModelAsync(friendId).Result == null)
        {
            await SendAnswer(client, message, token, "User not found");
            return;
        }

        if (_friendsDAL.GetFriendshipAsync(userId, friendId).Result != null)
        {
            await SendAnswer(client, message, token, "The request for friendship has already been created, it remains only to accept it");
            return;
        }

        var friendshipId = await _friendsDAL.CreateFriendshipAsync(new FriendshipModel() { FirstUserId = userId, SecondUserId = friendId, Status = (int)FriendShipStatus.Waiting });
        await SendFriendRequest(client, userId, friendId, friendshipId);
        await client.SendTextMessageAsync(userId, "Waiting... ");
    }

    private async Task SendFriendRequest(ITelegramBotClient client, long fromId, long toId, int friendshipId)
    {
        var buttons = new InlineKeyboardButton[2];
        buttons[0] = InlineKeyboardButton.WithCallbackData("✅", $"/acceptrequest {friendshipId}");
        buttons[1] = InlineKeyboardButton.WithCallbackData("🚫", $"/rejectrequest {friendshipId}");

        await client.SendTextMessageAsync(toId, $"User {fromId} want to be your friend", replyMarkup: new InlineKeyboardMarkup(buttons));
    }
}