using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ObserverMagazine.ContentProcessor;

public static class FrontMatterParser
{
    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Splits a markdown file into YAML front matter and markdown body.
    /// Front matter is delimited by --- at the start and end.
    /// </summary>
    public static (FrontMatter FrontMatter, string MarkdownBody) Parse(string rawContent)
    {
        if (!rawContent.StartsWith("---"))
        {
            return (new FrontMatter(), rawContent);
        }

        var endIndex = rawContent.IndexOf("---", 3, StringComparison.Ordinal);
        if (endIndex < 0)
        {
            return (new FrontMatter(), rawContent);
        }

        var yamlBlock = rawContent[3..endIndex].Trim();
        var body = rawContent[(endIndex + 3)..].TrimStart('\r', '\n');

        var frontMatter = Deserializer.Deserialize<FrontMatter>(yamlBlock);
        return (frontMatter ?? new FrontMatter(), body);
    }

    /// <summary>
    /// Parses an author YAML file into an AuthorProfile.
    /// </summary>
    public static AuthorProfile? ParseAuthor(string yamlContent, string id)
    {
        try
        {
            var profile = Deserializer.Deserialize<AuthorProfile>(yamlContent);
            if (profile is null) return null;
            profile.Id = id;
            return profile;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Derives a slug from a filename like "2026-01-15-welcome-to-observer-magazine".
    /// Strips the leading date prefix if present.
    /// </summary>
    public static string DeriveSlug(string fileName)
    {
        // Pattern: YYYY-MM-DD-rest-of-slug
        if (fileName.Length > 11 &&
            char.IsDigit(fileName[0]) &&
            fileName[4] == '-' &&
            fileName[7] == '-' &&
            fileName[10] == '-')
        {
            return fileName[11..];
        }
        return fileName;
    }

    /// <summary>
    /// Calculates estimated reading time in minutes from markdown text.
    /// Uses 200 words per minute, minimum 1 minute.
    /// </summary>
    public static int CalculateReadingTime(string markdownBody)
    {
        var wordCount = markdownBody
            .Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries)
            .Length;
        return Math.Max(1, (int)Math.Ceiling(wordCount / 200.0));
    }
}

public sealed class FrontMatter
{
    public string Title { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.MinValue;
    public DateTime? Updated { get; set; }
    public string? Author { get; set; }
    public string? Summary { get; set; }
    public string[]? Tags { get; set; }
    public bool Draft { get; set; }
    public bool Featured { get; set; }
    public string? Series { get; set; }
    public string? Image { get; set; }
}

public sealed class AuthorProfile
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public string? Bio { get; set; }
    public string? Avatar { get; set; }
    public Dictionary<string, string>? Socials { get; set; }
}
