using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.Services.Implementation
{
    public class FriendsService : IFriendsService
    {
        private readonly IFriendsRepository _IFR;
        public FriendsService(IFriendsRepository ifr)
        {
            this._IFR = ifr;
        }
        public void AcceptFriendRequest(string requesterId, string addresseeId)
        {
            _IFR.AcceptFriendRequest(requesterId, addresseeId);
        }

        public void AddFriendRequest(string requesterId, string addresseeId)
        {
            _IFR.AddFriendRequest(requesterId, addresseeId);
        }

        public void BlockUser(string blockerId, string blockedId)
        {
            _IFR.BlockUser(blockerId, blockedId);
        }

        public void CancelFriendRequest(string requesterId, string addresseeId)
        {
            _IFR.CancelFriendRequest(requesterId, addresseeId);
        }

        public void DeclineFriendRequest(string requesterId, string addresseeId)
        {
            _IFR.DeclineFriendRequest(requesterId, addresseeId);
        }

        public bool FriendshipExists(string userId1, string userId2)
        {
            return _IFR.FriendshipExists(userId1, userId2);
        }

        public IEnumerable<PoepleMV> GetAllUsers()
        {
            List<PoepleMV> ll = new List<PoepleMV>();
            var list = _IFR.GetAllUsers();

            foreach (var item in list)
            {
                ll.Add(new PoepleMV
                {
                    Id = item.Id,
                    Name = item.UserName,
                    ImgPath = item.ImgPath,
                    Status = _IFR.GetFriendshipStatus(item.Id, item.Id),
                    MutualFriendsCount = _IFR.GetMutualFriendCount(item.Id, item.Id),
                    Title = item.Title
                });
            }




            return ll;
        }

        public IEnumerable<Friends> GetBlockedUsers(string userId)
        {
            return _IFR.GetBlockedUsers(userId);
        }

        public int GetFriendCount(string userId)
        {
            return _IFR.GetFriendCount(userId);
        }

        public IEnumerable<Friends> GetFriends(string userId)
        {
            return _IFR.GetFriends(userId);
        }

        public FriendStatus GetFriendshipStatus(string userId1, string userId2)
        {
            return _IFR.GetFriendshipStatus(userId1, userId2);
        }

        public int GetPendingRequestCount(string userId)
        {
            return _IFR.GetPendingRequestCount(userId);
        }

        public IEnumerable<Friends> GetPendingRequests(string userId)
        {
            return _IFR.GetPendingRequests(userId);
        }

        public void Unfriend(string userId1, string userId2)
        {
            _IFR.Unfriend(userId1, userId2);
        }

        public IEnumerable<PoepleMV> GetPeopleYouMayKnow(string currentUserId)
        {
            var users = _IFR.GetPeopleYouMayKnow(currentUserId);

            return users.Select(u => new PoepleMV
            {
                Id = u.Id,
                Name = u.UserName,
                ImgPath = u.ImgPath,
                Status = _IFR.GetFriendshipStatus(currentUserId, u.Id),
                MutualFriendsCount = _IFR.GetMutualFriendCount(currentUserId, u.Id),
                Title = u.Title
            }).ToList();
        }
    }
}
