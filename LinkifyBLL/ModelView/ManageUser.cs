using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class ManageUser
    {
            public string UserId { get; set; }
            public string FullName { get; set; }
            public string AvatarUrl { get; set; }
            public FriendStatus Status { get; set; }
            public DateTime? Since { get; set; }
    }
}
