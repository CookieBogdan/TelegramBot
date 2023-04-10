using Microsoft.VisualBasic;
using MOFTbot.DAL.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MOFTbot.BL.BotCommands.FriendsCommands;

public class MyFriendsBotCommand : BotCommand
{
    public MyFriendsBotCommand(IFriendsDAL friendsDAL)
    {
        _friendDAL = friendsDAL;
    }

    public override string Name => "/myfriends";

    private IFriendsDAL _friendDAL;

    public override async Task Execute(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var userId = message.Chat.Id;

        var friends = await _friendDAL.GetFriendsAsync(userId);

        if(friends.Count() == 0)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "No friends, and you are not my friend, you are my brother, my friend");
            return;
        }

        var buttons = new List<InlineKeyboardButton[]>();

        foreach (var friend in friends)
        {
            var btns = new InlineKeyboardButton[2];

            btns[0] = InlineKeyboardButton.WithCallbackData(friend.UserName, $"-");
            btns[1] = InlineKeyboardButton.WithCallbackData("🗑", $"/deletefriend {friend.FriendshipId}");

            buttons.Add(btns);
        }

        buttons.Add( new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Delete message 🚫", $"/deletemessage") });
        var keyboard = new InlineKeyboardMarkup(buttons);
        await client.SendTextMessageAsync(message.Chat.Id, "Your friends", replyMarkup: keyboard);
    }
}
