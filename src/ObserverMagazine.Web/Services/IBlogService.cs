using ObserverMagazine.Web.Models;

namespace ObserverMagazine.Web.Services;

public interface IBlogService
{
    Task<BlogPostMetadata[]> GetPostsAsync();
    Task<BlogPostMetadata?> GetPostMetadataAsync(string slug);
    Task<string> GetPostHtmlAsync(string slug);
}
