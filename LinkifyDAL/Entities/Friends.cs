using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;
public class Friends
{
    public int Id { get; private set; }
    public string RequesterId { get; private set; }
    public string AddresseeId { get; private set; }
    public FriendStatus Status { get; private set; }
    public DateTime RequestDate { get; private set; }
    public DateTime? AcceptanceDate { get; private set; }
    public DateTime? ModificationDate { get; private set; }

    [ForeignKey(nameof(RequesterId))]
    public virtual User Requester { get; private set; }

    [ForeignKey(nameof(AddresseeId))]
    public virtual User Addressee { get; private set; }
    public Friends(string RequesterId, string AddresseeId)
    {
        this.RequesterId = RequesterId;
        this.AddresseeId = AddresseeId;
        this.Status = FriendStatus.Pending;
        this.RequestDate = DateTime.Now;
    }
    public void EditStatus(FriendStatus st)
    {
        if (st == FriendStatus.Accepted)
        {
            AcceptanceDate = DateTime.Now;
        }
        else { 
            ModificationDate = DateTime.Now;
        }
        this.Status = st;
    }
}