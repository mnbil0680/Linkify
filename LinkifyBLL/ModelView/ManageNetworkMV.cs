using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class ManageNetworkMV
    {
        public List<ManageUser> PendingRequests { get; set; }
        public List<ManageUser> AcceptedFriends { get; set; }
        public List<ManageUser> BlockedUsers { get; set; }
        public int PendingRequestsCount => PendingRequests.Count ;
        public int AcceptedFriendsCount => AcceptedFriends.Count ;
        public int BlockedUsersCount => BlockedUsers.Count ;
      
    }
}
