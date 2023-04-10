using MOFTbot.DAL.Models;

namespace MOFTbot.DAL.Interfaces;

public interface IAuthDal
{
    Task<UserModel?> GetUserModelAsync(long id);
    Task<UserModel?> GetUserModelAsync(string nickname);

    Task<IEnumerable<UserModel>> GetTopNUsers(int N);

    Task CreateUserAsync(UserModel userModel);
    Task UpdateUserNicknameAsync(long userId, string nickname);

    Task AddUserPointsAsync(long userId, int value);
    Task RemovePointsAsync(long userId, int value);
}