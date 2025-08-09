using LinkifyDAL.Entities;
using LinkifyDAL.Enums;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface IFriendsRepository
    {
        Task<bool> FriendshipExistsAsync(string userId1, string userId2);
        Task AddFriendRequestAsync(string requesterId, string addresseeId);
        Task AcceptFriendRequestAsync(string requesterId, string addresseeId);
        Task DeclineFriendRequestAsync(string requesterId, string addresseeId);
        Task CancelFriendRequestAsync(string requesterId, string addresseeId);
        Task BlockUserAsync(string blockerId, string blockedId);
        Task UnfriendAsync(string userId1, string userId2);
        Task<int> GetFriendCountAsync(string userId);
        Task<int> GetPendingRequestCountAsync(string userId);
        Task<IEnumerable<Friends>> GetPendingRequestsAsync(string userId);
        Task<IEnumerable<Friends>> GetFriendsAsync(string userId);
        Task<IEnumerable<Friends>> GetBlockedUsersAsync(string userId);

        //non friend users
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<FriendStatus> GetFriendshipStatusAsync(string userId1, string userId2);
        Task<IEnumerable<User>> GetPeopleYouMayKnowAsync(string currentUserId);
        Task<int> GetMutualFriendCountAsync(string currentUserId, string otherUserId);

    }
}
