using MassAidVOne.Domain.Utilities;

public partial class BaseEntity
{
    public long Id { get; set; }

    public long? CreatedBy { get; set; } = AppUserContext.UserId;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

}
