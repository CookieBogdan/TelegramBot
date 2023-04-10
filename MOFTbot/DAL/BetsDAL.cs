using Dapper;
using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using Npgsql;

namespace MOFTbot.DAL;

internal class BetsDAL : IBetsDAL
{
    public async Task CreateBetAsync(BetMemberModel betMemberModel)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        await connection.ExecuteAsync(
            @"begin;
                insert into BetMembers(UserId, MatchId, TeamId, Value)
                    values(@UserId, @MatchId, @TeamId, @Value);
                update Users set Points = Points - @Value
                    where Id = @UserId;
            commit;", betMemberModel);
    }

    public async Task<IEnumerable<BetMemberModel>> GetBetMembersAsync(int matchId)
    {
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

        return await connection.QueryAsync<BetMemberModel>(
            @"select *
            from BetMembers
            where MatchId = @matchId", new { matchId = matchId });
    }
}