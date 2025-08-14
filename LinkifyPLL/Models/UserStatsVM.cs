namespace LinkifyPLL.Models
{
    public class UserStatsVM
    {
        public LinkifyDAL.Entities.User User { get; set; }
        public int Connections { get; set; }
        public int Posts { get; set; }
        public int Reactions { get; set; }
    }
}
