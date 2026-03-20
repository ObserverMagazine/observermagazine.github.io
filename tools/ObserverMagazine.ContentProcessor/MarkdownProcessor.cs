using Markdig;

namespace ObserverMagazine.ContentProcessor;

public static class MarkdownProcessor
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public static string ToHtml(string markdown)
    {
        return Markdown.ToHtml(markdown, Pipeline);
    }
}
