using Slugify;

namespace Shared.Core.Domain.VOs;

public static class SlugUtil
{
    private static readonly SlugHelper SlugHelper;

    static SlugUtil()
    {
        var config = new SlugHelperConfiguration();
        config.StringReplacements.Add("ş", "s");
        config.StringReplacements.Add("ö", "o");
        config.StringReplacements.Add("ğ", "g");
        config.StringReplacements.Add("ı", "i");
        config.StringReplacements.Add("İ", "ı");
        config.StringReplacements.Add("ç", "c");
        config.StringReplacements.Add("ü", "u");
        SlugHelper = new SlugHelper(config);
    }

    public static Slug ToSlug(string value)
    {
        var trimmed = value.Trim();
        return new Slug(SlugHelper.GenerateSlug(trimmed));
    }
}