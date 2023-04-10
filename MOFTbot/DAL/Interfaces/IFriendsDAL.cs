using MOFTbot.DAL.Models;

namespace MOFTbot.DAL.Interfaces;

public interface IFriendsDAL
{
    Task<FriendshipModel?> GetFriendshipAsync(int id);
    Task<FriendshipModel?> GetFriendshipAsync(long userId, long otherUserId);
    Task<IEnumerable<FriendModel>> GetFriendsAsync(long userId);
    
    Task<int> CreateFriendshipAsync(FriendshipModel friendshipModel);
    Task UpdateFriendshipStatusAsync(int friendshipId, FriendShipStatus status);
    Task DeleteFriendshipAsync(int friendshipId);
}