using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
namespace LinkifyBLL.Services.Implementation
{
    public class FriendsService : IFriendsService
    {
        private readonly IFriendsRepository _friendsRepository;

        public FriendsService(IFriendsRepository friendsRepository)
        {
            _friendsRepository = friendsRepository ?? throw new ArgumentNullException(nameof(friendsRepository));
        }

        public async Task<bool> FriendshipExistsAsync(string userId1, string userId2)
        {
            if (string.IsNullOrEmpty(userId1) || string.IsNullOrEmpty(userId2))
            {
                throw new ArgumentException("User IDs cannot be null or empty");
            }

            return await _friendsRepository.FriendshipExistsAsync(userId1, userId2);
        }

        public async Task AddFriendRequestAsync(string requesterId, string addresseeId)
        {
            if (string.IsNullOrEmpty(requesterId) || string.IsNullOrEmpty(addresseeId))
            {
                throw new ArgumentException("User IDs cannot be null or empty");
            }

            if (requesterId == addresseeId)
            {
                throw new InvalidOperationException("Cannot send friend request to yourself");
            }

            if (await _friendsRepository.FriendshipExistsAsync(requesterId, addresseeId))
            {
                throw new InvalidOperationException("Friendship or request already exists");
            }

            await _friendsRepository.AddFriendRequestAsync(requesterId, addresseeId);
        }

        public async Task AcceptFriendRequestAsync(string requesterId, string addresseeId)
        {
            if (string.IsNullOrEmpty(requesterId) || string.IsNullOrEmpty(addresseeId))
            {
                throw new ArgumentException("User IDs cannot be null or empty");
            }

            var status = await _friendsRepository.GetFriendshipStatusAsync(requesterId, addresseeId);
            if (status != FriendStatus.Pending)
            {
                throw new InvalidOperationException("No pending friend request exists");
            }

            await _friendsRepository.AcceptFriendRequestAsync(requesterId, addresseeId);
        }

        public async Task DeclineFriendRequestAsync(string requesterId, string addresseeId)
        {
            if (string.IsNullOrEmpty(requesterId) || string.IsNullOrEmpty(addresseeId))
            {
                throw new ArgumentException("User IDs cannot be null or empty");
            }

            var status = await _friendsRepository.GetFriendshipStatusAsync(requesterId, addresseeId);
            if (status != FriendStatus.Pending)
            {
                throw new InvalidOperationException("No pending friend request exists");
            }

            await _friendsRepository.DeclineFriendRequestAsync(requesterId, addresseeId);
        }

        public async Task CancelFriendRequestAsync(string requesterId, string addresseeId)
        {
            if (string.IsNullOrEmpty(requesterId) || string.IsNullOrEmpty(addresseeId))
            {
                throw new ArgumentException("User IDs cannot be null or empty");
            }

            var status = await _friendsRepository.GetFriendshipStatusAsync(requesterId, addresseeId);
            if (status != FriendStatus.Pending)
            {
                throw new InvalidOperationException("No pending friend request exists");
            }

            await _friendsRepository.CancelFriendRequestAsync(requesterId, addresseeId);
        }

        public async Task BlockUserAsync(string blockerId, string blockedId)
        {
            if (string.IsNullOrEmpty(blockerId) || string.IsNullOrEmpty(blockedId))
            {
                throw new ArgumentException("User IDs cannot be null or empty");
            }

            if (blockerId == blockedId)
            {
                throw new InvalidOperationException("Cannot block yourself");
            }

            await _friendsRepository.BlockUserAsync(blockerId, blockedId);
        }

        public async Task UnfriendAsync(string userId1, string userId2)
        {
            if (string.IsNullOrEmpty(userId1) || string.IsNullOrEmpty(userId2))
            {
                throw new ArgumentException("User IDs cannot be null or empty");
            }

            var status = await _friendsRepository.GetFriendshipStatusAsync(userId1, userId2);
            if (status != FriendStatus.Accepted)
            {
                throw new InvalidOperationException("Users are not friends");
            }

            await _friendsRepository.UnfriendAsync(userId1, userId2);
        }

        public async Task<int> GetFriendCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty");
            }

            return await _friendsRepository.GetFriendCountAsync(userId);
        }

        public async Task<int> GetPendingRequestCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty");
            }

            return await _friendsRepository.GetPendingRequestCountAsync(userId);
        }

        public async Task<IEnumerable<Friends>> GetPendingRequestsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty");
            }

            return await _friendsRepository.GetPendingRequestsAsync(userId);
        }

        public async Task<IEnumerable<Friends>> GetFriendsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty");
            }

            return await _friendsRepository.GetFriendsAsync(userId);
        }

        public async Task<IEnumerable<Friends>> GetBlockedUsersAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty");
            }

            return await _friendsRepository.GetBlockedUsersAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _friendsRepository.GetAllUsersAsync();
        }

        public async Task<FriendStatus> GetFriendshipStatusAsync(string userId1, string userId2)
        {
            if (string.IsNullOrEmpty(userId1) || string.IsNullOrEmpty(userId2))
            {
                throw new ArgumentException("User IDs cannot be null or empty");
            }

            return await _friendsRepository.GetFriendshipStatusAsync(userId1, userId2);
        }

        public async Task<IEnumerable<User>> GetPeopleYouMayKnowAsync(string currentUserId)
        {
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new ArgumentException("User ID cannot be null or empty");
            }

            return await _friendsRepository.GetPeopleYouMayKnowAsync(currentUserId);
        }

        public async Task<int> GetMutualFriendCountAsync(string currentUserId, string otherUserId)
        {
            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(otherUserId))
            {
                throw new ArgumentException("User IDs cannot be null or empty");
            }

            return await _friendsRepository.GetMutualFriendCountAsync(currentUserId, otherUserId);
        }
    }
}
