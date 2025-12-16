//using System;
//using System.Collections.Generic;

//namespace MessAidVOne.Persistence.Models;

//public partial class ActivityInformation
//{
//    public long Id { get; set; }

//    public long? CreatedBy { get; set; }

//    public DateTime? CreatedOn { get; set; }

//    public long? ModifiedBy { get; set; }

//    public DateTime? ModifiedOn { get; set; }

//    public bool? IsDeleted { get; set; }

//    public long? DeletedBy { get; set; }

//    public DateTime? DeletedOn { get; set; }

//    public long ActorUserId { get; set; }

//    public long EntityId { get; set; }

//    public string EntityType { get; set; } = null!;

//    public string EventKey { get; set; } = null!;

//    public string Description { get; set; } = null!;

//    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
//}
