namespace ObserverMagazine.Web.Models;

public sealed record BlogPostMetadata
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public required DateTime Date { get; init; }
    public DateTime? Updated { get; init; }
    public string Author { get; init; } = "";
    public string AuthorName { get; init; } = "";
    public string Summary { get; init; } = "";
    public string[] Tags { get; init; } = [];
    public bool Featured { get; init; }
    public string? Series { get; init; }
    public string? Image { get; init; }
    public int ReadingTimeMinutes { get; init; }
}
