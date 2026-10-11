namespace Shared.Core.Application;

public sealed record ViewContext(
    Guid? ViewerId = null,
    bool IsAdmin = false,
    bool? KnownIsOwner = null,
    IReadOnlySet<string>? Expands = null)
{
    private static readonly IReadOnlySet<string> EmptyExpands =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlySet<string> Expands { get; init; } = Expands ?? EmptyExpands;

    public bool HasExpand(string field) => Expands.Contains(field);

    public static ViewContext Anonymous(IReadOnlySet<string>? expands = null) =>
        new() { Expands = expands ?? EmptyExpands };

    public static ViewContext ForAdmin(Guid adminId, IReadOnlySet<string>? expands = null, bool? isOwner = null) =>
        new(adminId, IsAdmin: true, KnownIsOwner: isOwner, Expands: expands ?? EmptyExpands);

    
    // We are sure he is owner.
    public static ViewContext ForOwner(Guid ownerId, IReadOnlySet<string>? expands = null, bool isAdmin = false) =>
        new(ownerId, IsAdmin: isAdmin, KnownIsOwner: true, Expands: expands ?? EmptyExpands);

    // We are sure he is NOT owner.
    public static ViewContext ForNonOwner(Guid userId, IReadOnlySet<string>? expands = null) =>
        new(userId, IsAdmin: false, KnownIsOwner: false, Expands: expands ?? EmptyExpands);

    
    // We do not know he is owner or not!
    public static ViewContext ForUser(Guid userId, IReadOnlySet<string>? expands = null) =>
        new(userId, IsAdmin: false, KnownIsOwner: null, Expands: expands ?? EmptyExpands);
}