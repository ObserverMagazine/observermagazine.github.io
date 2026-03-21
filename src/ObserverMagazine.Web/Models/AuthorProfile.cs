namespace ObserverMagazine.Web.Models;

public sealed record AuthorProfile
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string? Email { get; init; }
    public string? Bio { get; init; }
    public string? Avatar { get; init; }
    public Dictionary<string, string>? Socials { get; init; }
}
