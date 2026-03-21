using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ObserverMagazine.Web.Models;

namespace ObserverMagazine.Web.Services;

public sealed class BlogService(HttpClient http, ILogger<BlogService> logger) : IBlogService
{
    private BlogPostMetadata[]? cachedIndex;
    private AuthorProfile[]? cachedAuthors;

    public async Task<BlogPostMetadata[]> GetPostsAsync()
    {
        if (cachedIndex is not null) return cachedIndex;

        logger.LogInformation("Fetching blog posts index");
        try
        {
            var posts = await http.GetFromJsonAsync<BlogPostMetadata[]>("blog-data/posts-index.json");
            cachedIndex = posts?
                .OrderByDescending(p => p.Date)
                .ToArray() ?? [];
            logger.LogInformation("Loaded {Count} blog posts", cachedIndex.Length);
            return cachedIndex;
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Blog index not found — content processor may not have run");
            cachedIndex = [];
            return cachedIndex;
        }
    }

    public async Task<BlogPostMetadata?> GetPostMetadataAsync(string slug)
    {
        var posts = await GetPostsAsync();
        return posts.FirstOrDefault(p =>
            string.Equals(p.Slug, slug, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<string> GetPostHtmlAsync(string slug)
    {
        logger.LogInformation("Fetching HTML for post: {Slug}", slug);
        try
        {
            return await http.GetStringAsync($"blog-data/{slug}.html");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to load HTML for post: {Slug}", slug);
            return "<p>Could not load post content.</p>";
        }
    }

    public async Task<AuthorProfile[]> GetAllAuthorsAsync()
    {
        if (cachedAuthors is not null) return cachedAuthors;

        logger.LogInformation("Fetching authors index");
        try
        {
            var authors = await http.GetFromJsonAsync<AuthorProfile[]>("blog-data/authors.json");
            cachedAuthors = authors ?? [];
            logger.LogInformation("Loaded {Count} author profiles", cachedAuthors.Length);
            return cachedAuthors;
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Authors index not found");
            cachedAuthors = [];
            return cachedAuthors;
        }
    }

    public async Task<AuthorProfile?> GetAuthorAsync(string authorId)
    {
        var authors = await GetAllAuthorsAsync();
        return authors.FirstOrDefault(a =>
            string.Equals(a.Id, authorId, StringComparison.OrdinalIgnoreCase));
    }
}
