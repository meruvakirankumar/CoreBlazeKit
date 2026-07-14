namespace CoreBlaze.Components.Tests.Utilities;

public class CssBuilderTests
{
    [Fact]
    public void Default_WithBaseClass_StartsWithBaseClass()
    {
        var css = CssBuilder.Default("root").Build();
        Assert.Equal("root", css);
    }

    [Fact]
    public void AddClass_AppendsWithSpace()
    {
        var css = CssBuilder.Default("root").AddClass("modifier").Build();
        Assert.Equal("root modifier", css);
    }

    [Fact]
    public void AddClass_WithCondition_TrueAppends()
    {
        var css = CssBuilder.Default("root").AddClass("mod", when: true).Build();
        Assert.Equal("root mod", css);
    }

    [Fact]
    public void AddClass_WithCondition_FalseSkips()
    {
        var css = CssBuilder.Default("root").AddClass("mod", when: false).Build();
        Assert.Equal("root", css);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddClass_IgnoresNullOrWhitespace(string? value)
    {
        var css = CssBuilder.Default("root").AddClass(value).Build();
        Assert.Equal("root", css);
    }

    [Fact]
    public void Empty_ReturnsEmptyString()
    {
        var css = CssBuilder.Empty().Build();
        Assert.Equal(string.Empty, css);
    }

    [Fact]
    public void Chained_ProducesExpectedOrder()
    {
        var css = CssBuilder.Default("cb-input")
            .AddClass("cb-input--disabled", true)
            .AddClass("cb-input--invalid", false)
            .AddClass("user-class")
            .Build();
        Assert.Equal("cb-input cb-input--disabled user-class", css);
    }
}
