namespace ObserverMagazine.Web.Models;

public sealed record Product
{
    public string Name { get; init; } = "";
    public string Category { get; init; } = "";
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public double Rating { get; init; }
    public string Description { get; init; } = "";
}
