using Dapper;
using MOFTbot.DAL.Interfaces;
using MOFTbot.DAL.Models;
using Npgsql;

namespace MOFTbot.DAL;

internal class TeamsDal : ITeamsDal
{
	public async Task AddTeamAsync(TeamModel model)
	{
		using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
		connection.Open();

		model.Name = model.Name.ToLower();

		await connection.ExecuteAsync(
			@"insert into Teams(Id, Name) 
			values(@Id, @Name)", model);
	}

	public async Task AddFavoriteTeamAsync(long userId, int teamId)
	{
		using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
		connection.Open();

		await connection.ExecuteAsync(
			@"insert into FavoriteTeams(UserId, TeamId) 
			values(@userId, @teamId)", new { userId = userId, teamId = teamId});
	}

	public async Task AddObservedTeamAsync(long userId, int teamId)
	{
		using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
		connection.Open();

		await connection.ExecuteAsync(
			@"insert into ObservedTeams(UserId, TeamId) 
			values(@userId, @teamId)", new { userId = userId, teamId = teamId });
	}

	public async Task<IEnumerable<UserModel>> GetFansAsync(int teamId)
	{
		using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
		connection.Open();

		return await connection.QueryAsync<UserModel>(
			@"select u.Id, u.NickName, u.Points
				from (select UserId from FavoriteTeams where TeamId = @teamId) t
				left join Users u 
				on t.UserId = u.Id", new { teamId = teamId });
	}

	public async Task<IEnumerable<UserModel>> GetObserversAsync(int teamId)
	{
		using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
		connection.Open();

		return await connection.QueryAsync<UserModel>(
			@"select u.Id, u.NickName, u.Points
				from (select UserId from ObservedTeams where TeamId = @teamId) t
				left join Users u 
				on t.UserId = u.Id", new { teamId = teamId });
	}

	public async Task<IEnumerable<TeamModel>> GetFavoriteTeamsAsync(long userId)
	{
		using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
		connection.Open();

		return await connection.QueryAsync<TeamModel>(
			@"select t.Id, t.Name
				from (select TeamId from FavoriteTeams where UserId = @id) ft
				left join Teams t 
				on t.Id = ft.TeamId", new { id = userId });
	}

	public async Task<IEnumerable<TeamModel>> GetObservedTeamsAsync(long userId)
	{
		using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
		connection.Open();

		return await connection.QueryAsync<TeamModel>(
			@"select t.Id, t.Name
				from (select TeamId from ObservedTeams where UserId = @id) ot
				left join Teams t 
				on t.Id = ot.TeamId", new { id = userId });
	}


	public async Task<TeamModel?> GetTeamModelAsync(int id)
	{
		using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
		connection.Open();

		return await connection.QueryFirstOrDefaultAsync<TeamModel>(
			@"select Id, Name
				from Teams
				where Id = @id", new { id = id });
	}

	public async Task<TeamModel?> GetTeamModelAsync(string teamName)
	{
		using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
		connection.Open();

		return await connection.QueryFirstOrDefaultAsync<TeamModel>(
            @"select Id, Name
				from Teams
				where Name = @teamName", new { teamName = teamName.ToLower() });
	}

	public async Task RemoveObservedTeamAsync(long userId, int teamId)
	{
		using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
		connection.Open();

		await connection.QueryFirstOrDefaultAsync(
            @"delete from ObservedTeams
				where UserId = @userId and TeamId = @teamId",
			new { userId = userId, teamId = teamId });
	}

	public async Task<IEnumerable<UserModel>> GetObserversAsync(string teamName)
	{
        using var connection = new NpgsqlConnection(MyConfiguration.ConnectionString);
        connection.Open();

		return await connection.QueryAsync<UserModel>(
            @"select u.Id, u.NickName, u.Points
				from (select ob.UserId from
					(select t.Id, t.Name
					from teams t
					where t.Name = @teamName) t
				left join observedteams ob
				on t.Id = ob.TeamId) t
			left join users u
			on u.Id = t.UserId;",
			new { teamName = teamName.ToLower() });
    }
}