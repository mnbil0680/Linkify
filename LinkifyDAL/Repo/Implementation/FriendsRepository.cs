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
        public async Task AcceptFriendRequestAsync(string requesterId, string addresseeId)
        {
            var friendship = await _db.Friends.FirstOrDefaultAsync(f =>
                f.RequesterId == requesterId &&
                f.AddresseeId == addresseeId &&
                f.Status == FriendStatus.Pending);

            if (friendship == null)
                throw new KeyNotFoundException("Friend request not found");

            friendship.EditStatus(FriendStatus.Accepted);
            await _db.SaveChangesAsync();
        }
        public async Task AddFriendRequestAsync(string requesterId, string addresseeId)
        {
            if (requesterId == addresseeId)
                throw new ArgumentException("Cannot send friend request to yourself");

            if (await FriendshipExistsAsync(requesterId, addresseeId))
                throw new InvalidOperationException("Friendship already exists");

            var friendship = new Friends(requesterId, addresseeId);

            await _db.Friends.AddAsync(friendship);
            await _db.SaveChangesAsync();
        }
        public async Task BlockUserAsync(string blockerId, string blockedId)
        {
            if (string.IsNullOrEmpty(blockerId) || string.IsNullOrEmpty(blockedId))
                throw new ArgumentException("User IDs cannot be null or empty");
            if (blockerId == blockedId)
                throw new InvalidOperationException("Cannot block yourself");
            var existingRelationship = await _db.Friends
                .FirstOrDefaultAsync(f =>
                    (f.RequesterId == blockerId && f.AddresseeId == blockedId) ||
                    (f.RequesterId == blockedId && f.AddresseeId == blockerId));
            if (existingRelationship == null)
            {
                await _db.Friends.AddAsync(new Friends(blockerId, blockedId));
            }
            else
            {
                existingRelationship.EditStatus(FriendStatus.Blocked);
            }
            await _db.SaveChangesAsync();
        }
        public async Task DeclineFriendRequestAsync(string requesterId, string addresseeId)
        {
            var request = await _db.Friends.FirstOrDefaultAsync(f =>
                ((f.RequesterId == addresseeId && f.AddresseeId == requesterId)) &&
                f.Status == FriendStatus.Pending);

            if (request == null)
                throw new KeyNotFoundException("No pending friend request found");

            request.EditStatus(FriendStatus.Declined);
            await _db.SaveChangesAsync();
        }
        public async Task CancelFriendRequestAsync(string requesterId, string addresseeId)
        {
            var request = await _db.Friends.FirstOrDefaultAsync(f =>
                    ((f.RequesterId == requesterId && f.AddresseeId == addresseeId)) &&
                    f.Status == FriendStatus.Pending);
            if (request == null)
                throw new KeyNotFoundException("No pending friend request found");

            request.EditStatus(FriendStatus.None);
            await _db.SaveChangesAsync();
        }
        public async Task<bool> FriendshipExistsAsync(string userId1, string userId2)
        {
            return await _db.Friends.AnyAsync(f =>
                (f.RequesterId == userId1 && f.AddresseeId == userId2) ||
                (f.RequesterId == userId2 && f.AddresseeId == userId1));
        }
        public async Task<IEnumerable<Friends>> GetBlockedUsersAsync(string userId)
        {
            return await _db.Friends
                .Where(f =>
                    (f.RequesterId == userId || f.AddresseeId == userId) &&
                    f.Status == FriendStatus.Blocked)
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<int> GetFriendCountAsync(string userId)
        {
            return await _db.Friends
                .CountAsync(f =>
                    (f.RequesterId == userId || f.AddresseeId == userId) &&
                    f.Status == FriendStatus.Accepted);
        }
        public async Task<IEnumerable<Friends>> GetFriendsAsync(string userId)
        {
            return await _db.Friends
                .Where(f => (f.RequesterId == userId || f.AddresseeId == userId) &&
                            f.Status == FriendStatus.Accepted)
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .ToListAsync();
        }
        public async Task<FriendStatus> GetFriendshipStatusAsync(string userId1, string userId2)
        {
            var relationship = await _db.Friends
                .FirstOrDefaultAsync(f =>
                    (f.RequesterId == userId1 && f.AddresseeId == userId2) ||
                    (f.RequesterId == userId2 && f.AddresseeId == userId1));
            return relationship?.Status ?? FriendStatus.None;
        }
        public async Task<int> GetPendingRequestCountAsync(string userId)
        {
            return await _db.Friends
                .CountAsync(f =>
                    f.AddresseeId == userId &&
                    f.Status == FriendStatus.Pending);
        }
        public async Task<IEnumerable<Friends>> GetPendingRequestsAsync(string userId)
        {
            return await _db.Friends
                .Where(f => f.AddresseeId == userId && f.Status == FriendStatus.Pending)
                .Include(f => f.Requester)
                .ToListAsync();
        }
        public async Task UnfriendAsync(string userId1, string userId2)
        {
            var friendship = await _db.Friends
                .FirstOrDefaultAsync(f =>
                    ((f.RequesterId == userId1 && f.AddresseeId == userId2) ||
                     (f.RequesterId == userId2 && f.AddresseeId == userId1)) &&
                    f.Status == FriendStatus.Accepted);
            if (friendship == null)
                throw new KeyNotFoundException("No active friendship found");
            friendship.EditStatus(FriendStatus.Removed);
            await _db.SaveChangesAsync();
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _db.User.ToListAsync();
        }
        public async Task<IEnumerable<User>> GetPeopleYouMayKnowAsync(string currentUserId)
        {
            var relatedIds = await _db.Friends
                .Where(u =>
                    (u.RequesterId == currentUserId || u.AddresseeId == currentUserId) &&
                    (u.Status == FriendStatus.Pending ||
                     u.Status == FriendStatus.Accepted ||
                     u.Status == FriendStatus.Blocked))
                .Select(u =>
                    u.RequesterId == currentUserId ? u.AddresseeId : u.RequesterId)
                .Distinct()
                .ToListAsync();

            return await _db.User
                .Where(u => u.Id != currentUserId && !relatedIds.Contains(u.Id))
                .ToListAsync();
        }
        public async Task<int> GetMutualFriendCountAsync(string currentUserId, string otherUserId)
        {
            var currentUserFriends = await _db.Friends
                .Where(f => (f.RequesterId == currentUserId || f.AddresseeId == currentUserId) && (f.Status == FriendStatus.Accepted) )
                .Select(f => f.RequesterId == currentUserId ? f.AddresseeId : f.RequesterId)
                .ToListAsync();
            var otherUserFriends = await _db.Friends
                .Where(f => (f.RequesterId == otherUserId || f.AddresseeId == otherUserId) && (f.Status == FriendStatus.Accepted))
                .Select(f => f.RequesterId == otherUserId ? f.AddresseeId : f.RequesterId)
                .ToListAsync();
            return currentUserFriends.Intersect(otherUserFriends).Count();
        }
    }
}
