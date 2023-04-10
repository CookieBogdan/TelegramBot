using MOFTbot.DAL.Interfaces;
using Telegram.Bot;

namespace MOFTbot.BL.BotCommands.FriendsCommands;

public class DeleteFriendBotCommand : DeleteFriendshipBotCommand
{
    public DeleteFriendBotCommand(IFriendsDAL friendsDAL) : base(friendsDAL)
    {
        _friendsDAL = friendsDAL;
    }

    public override string Name => "/deletefriend";

    public override async Task Execute(ITelegramBotClient client, Telegram.Bot.Types.Message message, CancellationToken token)
    {
        await DeleteFriendship(await GetBodyCommand(client, message, token), message);
        await client.SendTextMessageAsync(message.Chat.Id, "Friend removed");
    }
}