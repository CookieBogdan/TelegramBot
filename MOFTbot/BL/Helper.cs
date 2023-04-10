using MOFTbot.DAL.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace MOFTbot.BL;

public static class Helper
{
    public static InlineKeyboardMarkup GetFollowingButton(this TeamModel team, IEnumerable<TeamModel> userFollows)
    {
        return (new TeamModel[1] { team }).GetFollowingButtons(userFollows);
    }

    public static InlineKeyboardMarkup GetFollowingButtons(this IEnumerable<TeamModel> teams, IEnumerable<TeamModel> userFollows)
    {
        var keyboard = new List<InlineKeyboardButton[]>();

        foreach (var team in teams)
        {
            var buttons = new InlineKeyboardButton[2];
            bool isFollow = userFollows.Any(uf => uf.Id == team.Id);
            buttons[0] = new InlineKeyboardButton(team.Name);
            buttons[0].CallbackData = " ";
            buttons[1] = InlineKeyboardButton.WithCallbackData(isFollow ? "🔕" : "🔔", isFollow ? $"/unfollow {team.Name}" : $"/follow {team.Name}");

            keyboard.Add(buttons);
        }

        keyboard.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Delete this message 🚫", "/deletemessage") });
        return new InlineKeyboardMarkup(keyboard);

    }
}