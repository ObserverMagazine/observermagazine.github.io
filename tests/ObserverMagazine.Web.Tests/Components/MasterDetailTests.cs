using Bunit;
using Microsoft.Extensions.DependencyInjection;
using ObserverMagazine.Web.Components;
using ObserverMagazine.Web.Services;
using ObserverMagazine.Web.Tests.Services;
using Xunit;

namespace ObserverMagazine.Web.Tests.Components;

public class MasterDetailTests : IDisposable
{
    private readonly BunitContext ctx = new();

    [Fact]
    public void MasterDetail_RendersWithoutProducts()
    {
        ctx.Services.AddSingleton<IAnalyticsService>(new NoOpAnalyticsService());
        var cut = ctx.Render<MasterDetail>();
        Assert.NotNull(cut);
    }

    public void Dispose() => ctx.Dispose();
}
