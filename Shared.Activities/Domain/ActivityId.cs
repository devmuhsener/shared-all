using Shared.Core.Domain.VOs;

namespace Shared.Activities.Domain;

public record ActivityId : BaseId<Guid>
{
    public ActivityId()
    {
    }

    public ActivityId(Guid value) : base(value)
    {
    }

    public static ActivityId New() => new(Guid.NewGuid());
    public static ActivityId From(Guid value) => new(value);
}
