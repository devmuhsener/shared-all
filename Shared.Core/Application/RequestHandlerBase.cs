using Shared.Core.Context;
using Shared.Core.Domain.Exceptions;

namespace Shared.Core.Application;

public abstract class RequestHandlerBase(IExecutionContextAccessor accessor)
{
    public bool IsSystem()
    {
        return accessor.Context.ActorType.Equals("System", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsAdmin()
    {
        return accessor.Context.ActorType.Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }


    public bool IsUser()
    {
        return accessor.Context.ActorType.Equals("User", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsGuest()
    {
        return accessor.Context.ActorType.Equals("guest", StringComparison.OrdinalIgnoreCase);
    }

    public bool TryGetUserId(out Guid? userId)
    {
        if (IsUser())
        {
            userId = Guid.Parse(accessor.Context.ActorId!);
            return true;
        }

        userId = null;
        return false;
    }


    public void EnsureOwner(Guid resourceOwnerId)
    {
        if (!IsUser()) return;

        // If user, must be owner of the resource!
        if (!Equals(resourceOwnerId, ExtractUserId()))
        {
            throw new ForbiddenException("User has no right to access this resource.");
        }
    }

    public ViewContext GetViewContext(IReadOnlySet<string>? expands = null, Guid? resourceOwnerId = null)
    {
        if (IsSystem())
            throw new InvalidOperationException("System has no right to view any resources.");

        if (IsGuest())
            return ViewContext.Anonymous(expands);

        var isAdmin = IsAdmin();
        var userId = ExtractUserId();

        if (isAdmin)
        {
            bool? isOwner = resourceOwnerId.HasValue ? resourceOwnerId.Value == userId : null;
            return ViewContext.ForAdmin(userId, expands, isOwner);
        }

        if (IsUser())
        {
            if (resourceOwnerId.HasValue)
            {
                return resourceOwnerId.Value == userId
                    ? ViewContext.ForOwner(userId, expands)
                    : ViewContext.ForNonOwner(userId, expands);
            }

            // Sahibi henüz bilinmiyor, DB çözecek
            return ViewContext.ForUser(userId, expands);
        }

        throw new InvalidOperationException("Cannot create view context, because actor is not defined.");
    }


    private Guid ExtractUserId()
    {
        var actorId = accessor.Context.ActorId;
        ArgumentNullException.ThrowIfNull(actorId, nameof(actorId));

        return Guid.Parse(actorId);
    }
}