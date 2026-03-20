using System.Xml.Linq;

namespace ObserverMagazine.ContentProcessor;

public static class RssGenerator
{
    public static string Generate(
        string title,
        string description,
        string siteUrl,
        IReadOnlyList<PostIndexEntry> posts)
    {
        var items = posts.Select(p =>
            new XElement("item",
                new XElement("title", p.Title),
                new XElement("link", $"{siteUrl}/blog/{p.Slug}"),
                new XElement("description", p.Summary),
                new XElement("pubDate", p.Date.ToString("R")),
                new XElement("guid", $"{siteUrl}/blog/{p.Slug}"),
                p.Tags.Length > 0
                    ? p.Tags.Select(t => new XElement("category", t))
                    : null
            ));

        var rss = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("rss",
                new XAttribute("version", "2.0"),
                new XElement("channel",
                    new XElement("title", title),
                    new XElement("link", siteUrl),
                    new XElement("description", description),
                    new XElement("language", "en-us"),
                    new XElement("lastBuildDate", DateTime.UtcNow.ToString("R")),
                    items
                )
            )
        );

        return rss.Declaration + Environment.NewLine + rss;
    }
}
