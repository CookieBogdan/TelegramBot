using MOFTbot.BL.Interfaces;
using MOFTbot.BL.BotCommands;
using MOFTbot.DAL;
using MOFTbot.DAL.Interfaces;
using MOFTbot.BL.BotCommands.FriendsCommands;

namespace MOFTbot.BL;

public class BotCommandsController : IBotCommandsController
{
    public BotCommandsController(
        IAuthDal authDal,
        ITeamsDal teamsDal,
        IBetsDAL betsDAL,
        IFriendsDAL friendsDAL,
        IHtmlParser htmlParser,
        IMatchesRepository matchesRepository)
    {
        _authDal = authDal;
        _teamsDal = teamsDal;
        _htmlParser = htmlParser;

        _commands = new BotCommand[]
        {
            new StartBotCommand(_authDal),
            new SetNicknameBotCommand(_authDal),
            new StartObserveTeamBotCommand(_teamsDal),
            new AddTeamBotCommand(_teamsDal, _htmlParser),
            new BecomeFanBotCommand(_teamsDal),
            new EndObserveBotCommand (_teamsDal),
            new TopBotCommand(_htmlParser, teamsDal),
            new BetBotCommand(matchesRepository, betsDAL, _authDal, teamsDal),
            new GetUsersRatingBotCommand(authDal, teamsDal),
            new BalanceBotCommand(authDal),
            new DeleteMessageBotCommand(),
            new MyFollowsBotCommand(teamsDal),
            new AddFriendBotCommand(friendsDAL, authDal),
            new AcceptFriendRequestBotCommand(friendsDAL),
            new RejectFriendRequestBotCommand(friendsDAL),
            new MyFriendsBotCommand(friendsDAL),
            new DeleteFriendBotCommand(friendsDAL),


        };
    }

    private readonly IAuthDal _authDal;
    private readonly ITeamsDal _teamsDal;
    private readonly IHtmlParser _htmlParser;

    private BotCommand[] _commands;

    public BotCommand? GetBotCommand(Telegram.Bot.Types.Message msg)
    {
        return _commands.FirstOrDefault(c => c.Contains(msg));
    }
}