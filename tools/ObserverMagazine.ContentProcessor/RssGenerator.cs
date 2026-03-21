using System.Xml.Linq;

namespace ObserverMagazine.ContentProcessor;

public static class RssGenerator
{
    private static readonly XNamespace ContentNs = "http://purl.org/rss/1.0/modules/content/";

    public static string Generate(
        string title, string description, string siteUrl,
        IReadOnlyList<PostIndexEntry> posts,
        Func<string, string?>? getPostHtml = null)
    {
        var items = posts.Select(p =>
        {
            var itemElements = new List<object>
            {
                new XElement("title", p.Title),
                new XElement("link", $"{siteUrl}/blog/{p.Slug}"),
                new XElement("description", p.Summary),
                new XElement("pubDate", p.Date.ToString("R")),
                new XElement("guid", $"{siteUrl}/blog/{p.Slug}")
            };

            // Include author email if available (RSS spec: email (name))
            if (!string.IsNullOrEmpty(p.AuthorEmail))
            {
                itemElements.Add(new XElement("author", $"{p.AuthorEmail} ({p.AuthorName})"));
            }
            else if (!string.IsNullOrEmpty(p.AuthorName))
            {
                // dc:creator could be used here, but for simplicity just skip <author>
                // when no email is available (RSS spec requires email in <author>)
            }

            // Include full HTML content if available
            var html = getPostHtml?.Invoke(p.Slug);
            if (!string.IsNullOrEmpty(html))
            {
                itemElements.Add(new XElement(ContentNs + "encoded", new XCData(html)));
            }

            if (p.Tags.Length > 0)
            {
                itemElements.AddRange(p.Tags.Select(t => new XElement("category", t)));
            }

            return new XElement("item", itemElements);
        });

        var rss = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("rss",
                new XAttribute("version", "2.0"),
                new XAttribute(XNamespace.Xmlns + "content", ContentNs),
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
