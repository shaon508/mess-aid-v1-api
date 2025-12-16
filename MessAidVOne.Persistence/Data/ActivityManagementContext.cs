using Microsoft.EntityFrameworkCore;

namespace MessAidVOne.Persistence.Data;

public partial class ActivityManagementContext : DbContext
{
    public ActivityManagementContext()
    {
    }

    public ActivityManagementContext(DbContextOptions<ActivityManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityInformation> ActivityInformations { get; set; }

    public virtual DbSet<ActivityOutbox> ActivityOutboxes { get; set; }

    public virtual DbSet<UserActivity> UserActivities { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<ActivityInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("activity_information");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActorUserId).HasColumnName("actor_user_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_on");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.DeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("deleted_on");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(30)
                .HasColumnName("entity_type");
            entity.Property(e => e.EventKey)
                .HasMaxLength(100)
                .HasColumnName("event_key");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("modified_on");
        });

        modelBuilder.Entity<ActivityOutbox>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("activity_outbox");

            entity.HasIndex(e => e.CreatedOn, "idx_outbox_created");

            entity.HasIndex(e => new { e.Status, e.ProcessingAttempts }, "idx_outbox_status_attempts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActorUserId).HasColumnName("actor_user_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_on");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.DeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("deleted_on");
            entity.Property(e => e.Domain).HasColumnName("domain");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.EventKey)
                .HasMaxLength(100)
                .HasColumnName("event_key");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("modified_on");
            entity.Property(e => e.PayloadJson)
                .HasColumnType("json")
                .HasColumnName("payload_json");
            entity.Property(e => e.ProcessedAt)
                .HasMaxLength(6)
                .HasColumnName("processed_at");
            entity.Property(e => e.ProcessingAttempts).HasColumnName("processing_attempts");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_activity");

            entity.HasIndex(e => e.UserId, "idx_user_activity_user_id");

            entity.HasIndex(e => new { e.ActivityId, e.UserId }, "uq_user_activity").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActivityId).HasColumnName("activity_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_on");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.DeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("deleted_on");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("modified_on");
            entity.Property(e => e.ReadAt)
                .HasColumnType("datetime")
                .HasColumnName("read_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Activity).WithMany(p => p.UserActivities)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_activity_activity");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
