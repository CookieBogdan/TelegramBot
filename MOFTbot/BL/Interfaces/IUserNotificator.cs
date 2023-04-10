namespace MOFTbot.BL.Interfaces;

internal interface IUserNotificator
{
    void Notify(long userId, string message);
}
