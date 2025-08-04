using LinkifyDAL.Entities;
using LinkifyDAL.Enums;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface IFriendsRepository
    {
        bool FriendshipExists(string userId1, string userId2);
        void AddFriendRequest(string requesterId, string addresseeId);
        void AcceptFriendRequest(string requesterId, string addresseeId);
        void DeclineFriendRequest(string requesterId, string addresseeId);
        void CancelFriendRequest(string requesterId, string addresseeId);
        void BlockUser(string blockerId, string blockedId);
        void Unfriend(string userId1, string userId2);
        int GetFriendCount(string userId);
        int GetPendingRequestCount(string userId);
        IEnumerable<Friends> GetPendingRequests(string userId);
        IEnumerable<Friends> GetFriends(string userId);
        IEnumerable<Friends> GetBlockedUsers(string userId);

        //non friend users
        IEnumerable<User> GetAllUsers();
        FriendStatus GetFriendshipStatus(string userId1, string userId2);
    }
}
