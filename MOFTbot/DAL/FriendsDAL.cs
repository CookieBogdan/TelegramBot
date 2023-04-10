using Dapper;
using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using Npgsql;

namespace MOFTbot.DAL;

public class FriendsDAL : IFriendsDAL
{
    public async Task<int> CreateFriendshipAsync(FriendshipModel friendshipModel)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        await connection.ExecuteAsync(@"insert into Friendships(FirstUserId, SecondUserId, Status)
                                        values(@FirstUserId, @SecondUserId, @Status)", friendshipModel);

        FriendshipModel res = GetFriendshipAsync(friendshipModel.FirstUserId, friendshipModel.SecondUserId).Result!;
        return (int)res.Id!;
    }

    public async Task DeleteFriendshipAsync(int friendshipId)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        await connection.ExecuteAsync(@"delete from Friendships
                                            where Id = @id", new { id = friendshipId });
    }

    public async Task<IEnumerable<FriendModel>> GetFriendsAsync(long userId)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        return await connection.QueryAsync<FriendModel>(
            $@"select f.UserId, f.Id as FriendshipId, u.Nickname as UserName
            from 
                (select case when FirstUserId = @userId then SecondUserId else FirstUserId end as UserId, Id
                from Friendships 
                where (FirstUserId = @userId or SecondUserId = @userId) and Status = {(int)FriendShipStatus.Accepted}) f
            left join Users u on f.UserId = u.Id", new { userId = userId });
    }


    public async Task<FriendshipModel?> GetFriendshipAsync(long userId, long otherUserId)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        return await connection.QueryFirstOrDefaultAsync<FriendshipModel>(@"select * 
                                                                            from Friendships 
                                                                            where FirstUserId in (@userId, @otherUserId) 
                                                                                  and
                                                                                  SecondUserId in (@userId, @otherUserId)", new { userId = userId, otherUserId = otherUserId });
    }

    public async Task<FriendshipModel?> GetFriendshipAsync(int id)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        return await connection.QueryFirstOrDefaultAsync<FriendshipModel>(@"select * 
                                                                    from Friendships
                                                                    where Id = @id", new { id = id });
    }

    public async Task UpdateFriendshipStatusAsync(int friendshipId, FriendShipStatus status)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        await connection.ExecuteAsync(@"update Friendships
                                        set Status = @status 
                                        where Id = @id", new { id = friendshipId, status = (int)status });
    }
}