using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Activities.Domain;

namespace Shared.Activities.Infra.Persistence.Configurations;

public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.ToTable("user_activities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, val => ActivityId.From(val))
            .ValueGeneratedNever();

        builder.Property(x => x.TraceId)
            .HasColumnName("trace_id")
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.ActorId)
            .HasColumnName("actor_id")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.ActorType)
            .HasColumnName("actor_type")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.OwnerId)
            .HasColumnName("owner_id")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.ImpersonatorId)
            .HasColumnName("impersonator_id")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.ExecutionType)
            .HasColumnName("execution_type")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.Verb)
            .HasColumnName("verb")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.ObjectId)
            .HasColumnName("object_id")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.ObjectType)
            .HasColumnName("object_type")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.TargetId)
            .HasColumnName("target_id")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.TargetType)
            .HasColumnName("target_type")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(x => x.ClientIp)
            .HasColumnName("client_ip")
            .HasColumnType("inet")
            .IsRequired(false);

        builder.Property(x => x.UserAgent)
            .HasColumnName("user_agent")
            .HasMaxLength(512)
            .IsRequired(false);

        builder.Property(x => x.Device)
            .HasColumnName("device")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.Country)
            .HasColumnName("country")
            .HasMaxLength(5)
            .IsRequired(false);

        builder.Property(x => x.ClientType)
            .HasColumnName("client_type")
            .HasMaxLength(32)
            .IsRequired(false);

        builder.Property(x => x.OccurredAt)
            .HasColumnName("occurred_at")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // B-Tree Indexes as specified in SCH-002
        builder.HasIndex(x => new { x.OwnerId, x.OccurredAt })
            .HasDatabaseName("idx_user_activities_owner_occurred")
            .HasFilter("owner_id IS NOT NULL");

        builder.HasIndex(x => new { x.ActorId, x.ActorType, x.OccurredAt })
            .HasDatabaseName("idx_user_activities_actor_occurred")
            .HasFilter("actor_id IS NOT NULL");

        builder.HasIndex(x => new { x.Verb, x.OccurredAt })
            .HasDatabaseName("idx_user_activities_verb_occurred");

        builder.HasIndex(x => new { x.ClientIp, x.OccurredAt })
            .HasDatabaseName("idx_user_activities_ip_occurred")
            .HasFilter("client_ip IS NOT NULL");

        builder.HasIndex(x => x.TraceId)
            .HasDatabaseName("idx_user_activities_trace_id")
            .HasFilter("trace_id IS NOT NULL");
    }
}