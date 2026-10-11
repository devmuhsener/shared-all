namespace Shared.Core.Domain.VOs;

public class Actor
{
    public string? Id { get; private set; }
    public string Type { get; private set; } = null!;

    public HashSet<string> Roles { get; private set; } = [];


    public string? Name { get; private set; }
    public string? Email { get; private set; }
    public string? Username { get; private set; }


    public static Actor User(Guid id, IEnumerable<string>? roles = null, string? name = null, string? email = null,
        string? username = null)
    {
        return new Actor()
        {
            Id = id.ToString(),
            Type = "User",
            Roles = roles != null ? [..roles] : [],
            Name = name,
            Email = email,
            Username = username
        };
    }

    public static Actor Admin(Guid id, IEnumerable<string>? roles = null, string? name = null, string? email = null,
        string? username = null)
    {
        return new Actor()
        {
            Id = id.ToString(),
            Type = "Admin",
            Roles = roles != null ? [..roles] : [],
            Name = name,
            Email = email,
            Username = username
        };
    }


    public static Actor System(string id)
    {
        return new Actor()
        {
            Id = id,
            Type = "System"
        };
    }
}