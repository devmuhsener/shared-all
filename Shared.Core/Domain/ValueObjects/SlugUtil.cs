using Slugify;

namespace Shared.Core.Domain.ValueObjects;

public static class SlugUtil
{
    private static readonly SlugHelper _slugHelper = default!;

    static SlugUtil()
    {
        var config = new SlugHelperConfiguration();
        config.StringReplacements.Add("#", "sharp");
        config.StringReplacements.Add(".", "dot");
        config.StringReplacements.Add("+", "p");
        config.StringReplacements.Add("++", "pp");
        _slugHelper = new SlugHelper(config);
    }

    public static Slug ToSlug(string value)
    {
        return new Slug(_slugHelper.GenerateSlug(value));
    }
}