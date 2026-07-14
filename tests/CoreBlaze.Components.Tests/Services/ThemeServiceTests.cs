namespace CoreBlaze.Components.Tests.Services;

public class ThemeServiceTests
{
    [Fact]
    public void CurrentTheme_DefaultsToLight()
    {
        var svc = new ThemeService();
        Assert.Equal("light", svc.CurrentTheme);
    }

    [Fact]
    public void SetTheme_ChangesCurrentAndRaisesEvent()
    {
        var svc = new ThemeService();
        string? raised = null;
        svc.ThemeChanged += t => raised = t;

        svc.SetTheme("dark");

        Assert.Equal("dark", svc.CurrentTheme);
        Assert.Equal("dark", raised);
    }

    [Fact]
    public void SetTheme_SameValueDoesNotRaiseEvent()
    {
        var svc = new ThemeService();
        var raised = 0;
        svc.ThemeChanged += _ => raised++;

        svc.SetTheme("light"); // already light — no event

        Assert.Equal(0, raised);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetTheme_IgnoresInvalidValues(string? theme)
    {
        var svc = new ThemeService();
        svc.SetTheme(theme!);
        Assert.Equal("light", svc.CurrentTheme);
    }
}
