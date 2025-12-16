////using System;
//using System.Collections.Generic;

//namespace MessAidVOne.Persistence.Models;

//public partial class ActivityOutbox
//{
//    public long Id { get; set; }

//    public long? CreatedBy { get; set; }

//    public DateTime? CreatedOn { get; set; }

//    public long? ModifiedBy { get; set; }

//    public DateTime? ModifiedOn { get; set; }

//    public bool? IsDeleted { get; set; }

//    public long? DeletedBy { get; set; }

//    public DateTime? DeletedOn { get; set; }

//    public string EventKey { get; set; } = null!;

//    public sbyte Domain { get; set; }

//    public long ActorUserId { get; set; }

//    public long EntityId { get; set; }

//    public string EntityType { get; set; } = null!;

//    public string PayloadJson { get; set; } = null!;

//    public sbyte Status { get; set; }

//    public int ProcessingAttempts { get; set; }

//    public DateTime? ProcessedAt { get; set; }
//}
