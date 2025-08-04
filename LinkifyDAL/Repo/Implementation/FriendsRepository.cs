using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LinkifyDAL.Repo.Implementation
{
    public class FriendsRepository : IFriendsRepository
    {
        private readonly LinkifyDbContext _db;

        public FriendsRepository(LinkifyDbContext db)
        {
            _db = db;
        }
        public void AcceptFriendRequest(string requesterId, string addresseeId)
        {
            var friendship = _db.Friends.FirstOrDefault(f =>
            f.RequesterId == requesterId &&
            f.AddresseeId == addresseeId &&
            f.Status == FriendStatus.Pending);

            if (friendship == null)
                throw new KeyNotFoundException("Friend request not found");

            friendship.EditStatus(FriendStatus.Accepted);
            _db.SaveChanges();
        }

        public void AddFriendRequest(string requesterId, string addresseeId)
        {
            if (requesterId == addresseeId)
                throw new ArgumentException("Cannot send friend request to yourself");

            if (FriendshipExists(requesterId, addresseeId))
                throw new InvalidOperationException("Friendship already exists");

            var friendship = new Friends(requesterId, addresseeId);

            _db.Friends.Add(friendship);
            _db.SaveChanges();
        }

        public void BlockUser(string blockerId, string blockedId)
        {
            if (string.IsNullOrEmpty(blockerId) || string.IsNullOrEmpty(blockedId))
                throw new ArgumentException("User IDs cannot be null or empty");

            if (blockerId == blockedId)
                throw new InvalidOperationException("Cannot block yourself");

            var existingRelationship = _db.Friends
                .FirstOrDefault(f =>
                    (f.RequesterId == blockerId && f.AddresseeId == blockedId) ||
                    (f.RequesterId == blockedId && f.AddresseeId == blockerId));

            if (existingRelationship == null)
            {
                _db.Friends.Add(new Friends(blockerId, blockedId));
            }
            else
            {
                existingRelationship.EditStatus(FriendStatus.Blocked);
            }

            _db.SaveChanges();
        }

        public void DeclineFriendRequest(string requesterId, string addresseeId)
        {
            var request = _db.Friends.FirstOrDefault(f =>
                ((f.RequesterId == addresseeId && f.AddresseeId == requesterId)) &&
                f.Status == FriendStatus.Pending);

            if (request == null)
                throw new KeyNotFoundException("No pending friend request found");

            request.EditStatus(FriendStatus.Declined);
            _db.SaveChanges();
        }

        public void CancelFriendRequest(string requesterId, string addresseeId)
        {
            var request = _db.Friends.FirstOrDefault(f =>
                    ((f.RequesterId == requesterId && f.AddresseeId == addresseeId)) &&
                    f.Status == FriendStatus.Pending);
            if (request == null)
                throw new KeyNotFoundException("No pending friend request found");

            request.EditStatus(FriendStatus.None);
            _db.SaveChanges();
        }

        public bool FriendshipExists(string userId1, string userId2)
        {
            return _db.Friends.Any(f =>
                (f.RequesterId == userId1 && f.AddresseeId == userId2) ||
                (f.RequesterId == userId2 && f.AddresseeId == userId1));
        }

        public IEnumerable<Friends> GetBlockedUsers(string userId)
        {
            return _db.Friends
            .Where(f =>
                (f.RequesterId == userId || f.AddresseeId == userId) &&
                f.Status == FriendStatus.Blocked)
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .AsNoTracking()
            .ToList();
        }

        public int GetFriendCount(string userId)
        {
            return _db.Friends
            .Count(f =>
                (f.RequesterId == userId || f.AddresseeId == userId) &&
                f.Status == FriendStatus.Accepted);
        }

        public IEnumerable<Friends> GetFriends(string userId)
        {
            return _db.Friends
            .Where(f => (f.RequesterId == userId || f.AddresseeId == userId) &&
                        f.Status == FriendStatus.Accepted)
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .ToList();
        }

        public FriendStatus GetFriendshipStatus(string userId1, string userId2)
        {
            var relationship = _db.Friends
            .FirstOrDefault(f =>
                (f.RequesterId == userId1 && f.AddresseeId == userId2) ||
                (f.RequesterId == userId2 && f.AddresseeId == userId1));

            return relationship?.Status ?? FriendStatus.None;
        }

        public int GetPendingRequestCount(string userId)
        {
            return _db.Friends
            .Count(f =>
                f.AddresseeId == userId &&
                f.Status == FriendStatus.Pending);
        }

        public IEnumerable<Friends> GetPendingRequests(string userId)
        {
            return _db.Friends
            .Where(f => f.AddresseeId == userId && f.Status == FriendStatus.Pending)
            .Include(f => f.Requester)
            .ToList();
        }

        public void Unfriend(string userId1, string userId2)
        {
            var friendship = _db.Friends
            .FirstOrDefault(f =>
                ((f.RequesterId == userId1 && f.AddresseeId == userId2) ||
                 (f.RequesterId == userId2 && f.AddresseeId == userId1)) &&
                f.Status == FriendStatus.Accepted);

            if (friendship == null)
                throw new KeyNotFoundException("No active friendship found");

            friendship.EditStatus(FriendStatus.Removed);
            _db.SaveChanges();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _db.User.ToList();
        }

        public IEnumerable<User> GetPeopleYouMayKnow(string currentUserId)
        {
            var relatedIds = _db.Friends.Where(u =>
                (u.RequesterId == currentUserId || u.AddresseeId == currentUserId) && // me as a Requester or Addressee 
                ( u.Status == FriendStatus.Pending || // if i am pending friend request or i have been being pended
                  u.Status == FriendStatus.Accepted ||
                  u.Status == FriendStatus.Blocked))
                .Select(u =>
                    u.RequesterId == currentUserId ? u.AddresseeId : u.RequesterId) // extract the ID of the person i am connected to 
                .Distinct() // distinct to avoid duplicates
                .ToList();

            // Get all users except the current user and those already related
            return _db.User
                .Where(u => u.Id != currentUserId && !relatedIds.Contains(u.Id))
                .ToList(); //

        }
        public int GetMutualFriendCount(string currentUserId, string otherUserId)
        {
            var currentUserFriends = _db.Friends
                .Where( f => f.RequesterId == currentUserId || f.AddresseeId == currentUserId)
                .Select(f => f.RequesterId == currentUserId ? f.AddresseeId : f.RequesterId)
                .ToList();
            var otherUserFriends = _db.Friends
                .Where(f => f.RequesterId == otherUserId || f.AddresseeId == otherUserId)
                .Select(f => f.RequesterId == otherUserId ? f.AddresseeId : f.RequesterId)
                .ToList();
            return currentUserFriends.Intersect(otherUserFriends).Count();

        }

    }
}
