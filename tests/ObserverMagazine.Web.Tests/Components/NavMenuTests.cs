using Bunit;
using Microsoft.JSInterop;
using ObserverMagazine.Web.Layout;
using Xunit;

namespace ObserverMagazine.Web.Tests.Components;

public class NavMenuTests : IDisposable
{
    private readonly BunitContext _ctx = new();

    [Fact]
    public void FirstRender_AppliesSavedTheme()
    {
        _ctx.JSInterop.Setup<string>("observerTheme.get").SetResult("dark");

        var cut = _ctx.Render<NavMenu>();

        cut.WaitForAssertion(() =>
            Assert.Equal("dark", cut.Find("select.theme-select").GetAttribute("value")));
    }

    [Fact]
    public void FirstRender_WhenThemeReadFails_FallsBackToSystem()
    {
        _ctx.JSInterop.Setup<string>("observerTheme.get").SetException(new JSException("storage unavailable"));

        var cut = _ctx.Render<NavMenu>();

        Assert.Equal("system", cut.Find("select.theme-select").GetAttribute("value"));
    }

    [Fact]
    public void ChangingTheme_PersistsThroughInterop()
    {
        _ctx.JSInterop.Setup<string>("observerTheme.get").SetResult("system");
        _ctx.JSInterop.SetupVoid("observerTheme.set", _ => true).SetVoidResult();

        var cut = _ctx.Render<NavMenu>();
        cut.Find("select.theme-select").Change("sepia");

        var invocation = _ctx.JSInterop.VerifyInvoke("observerTheme.set");
        Assert.Equal("sepia", invocation.Arguments[0]);
        Assert.Equal("sepia", cut.Find("select.theme-select").GetAttribute("value"));
    }

    [Fact]
    public void ChangingTheme_WhenInteropFails_DoesNotThrow()
    {
        _ctx.JSInterop.Setup<string>("observerTheme.get").SetResult("system");
        _ctx.JSInterop.SetupVoid("observerTheme.set", _ => true).SetException(new JSException("storage unavailable"));

        var cut = _ctx.Render<NavMenu>();
        cut.Find("select.theme-select").Change("dark");

        Assert.Equal("dark", cut.Find("select.theme-select").GetAttribute("value"));
    }

    [Fact]
    public void ToggleButton_OpensAndClosesMenu()
    {
        _ctx.JSInterop.Setup<string>("observerTheme.get").SetResult("system");

        var cut = _ctx.Render<NavMenu>();
        cut.Find(".nav-toggle").Click();
        Assert.Contains("open", cut.Find(".nav-links").ClassList);

        cut.Find(".nav-overlay").Click();
        Assert.DoesNotContain("open", cut.Find(".nav-links").ClassList);
    }

    public void Dispose() => _ctx.Dispose();
}
