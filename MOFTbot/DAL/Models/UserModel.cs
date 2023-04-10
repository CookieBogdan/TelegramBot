namespace MOFTbot.DAL.Models;

public class UserModel
{
    public long Id { get; set; }
    public string NickName { get; set; } = null!;
    public int Points { get; set; }
}