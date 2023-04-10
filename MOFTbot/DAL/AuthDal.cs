using Dapper;
using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using Npgsql;

namespace MOFTbot.DAL;

public class AuthDal : IAuthDal
{
    public async Task CreateUserAsync(UserModel userModel)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        await connection.ExecuteAsync(
            @"insert into Users(Id, NickName, Points) 
            values(@Id, @NickName, @Points)", userModel);
    }

    public async Task<UserModel?> GetUserModelAsync(long id)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        return await connection.QueryFirstOrDefaultAsync<UserModel>(
            @"select Id, NickName, Points 
                from Users
                where Id = @id", new { id = id });
    }

    public async Task<UserModel?> GetUserModelAsync(string nickname)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        return await connection.QueryFirstOrDefaultAsync<UserModel>(
            @"select Id, NickName, Points 
            from Users
            where NickName = @nickname", new { nickname = nickname });
    }

    public async Task<IEnumerable<UserModel>> GetTopNUsers(int N)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        return await connection.QueryAsync<UserModel>(
            @$"SELECT * 
            FROM Users
            ORDER BY Points desc
            limit {N}");
    }

    public async Task UpdateUserNicknameAsync(long userId, string nickname)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        await connection.ExecuteAsync(
            @"update Users
            set Nickname = @nickname
            where Id = @userId", new { userId = userId, nickname = nickname });
    }

    public async Task AddUserPointsAsync(long userId, int value)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        await connection.ExecuteAsync(
            @"update Users
            set Points = Points + @value
            where Id = @userId", new { userId = userId, value = value });
    }

    public async Task RemovePointsAsync(long userId, int value)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        await connection.ExecuteAsync(
            @"update Users
            set Points = Points - @value
            where Id = @userId", new { userId = userId, value = value });
    }
}