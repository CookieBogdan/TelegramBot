using MOFTbot.BL;
using MOFTbot.BL.Interfaces;
using MOFTbot.DAL;
using Telegram.Bot;

namespace MOFTbot;

public class Program
{
    public static void Main(string[] args)
    {
        MyConfiguration.AddConfiguration();

        var repository = new MatchesRepository();
        var htmlParser = new HtmlParser();
        var observer = new HltvObserver(repository, htmlParser, 1);
        
        var authDAL = new AuthDal();
        var teamsDal = new TeamsDal();
        var betsDAL = new BetsDAL();
        var friendsDAL = new FriendsDAL();

        var client = new TelegramBotClient(MyConfiguration.Token);
        IMessagesHandler messagesHandler = new MessagesHandler(
            new BotCommandsController(
                authDAL,
                teamsDal,
                betsDAL,
                friendsDAL,
                htmlParser,
                repository
                ));

        IUserNotificator notificator = new Notificator(client);
        var matchResultHandler = new MatchResultHandler(teamsDal, betsDAL, authDAL, notificator);
        observer.OnMatchFinished += matchResultHandler.HandleMatchResultAsync;
        observer.OnMatchStarting += matchResultHandler.HandlerMatchStartingAsync;
        observer.Start();

        messagesHandler.StartReceiving(client);

        Console.ReadLine();
    }
}