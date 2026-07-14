using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Tests.Inputs;

public class TextInputTests : TestContext
{
    [Fact]
    public void RendersLabelAndInput()
    {
        var cut = RenderComponent<TextInput>(p => p
            .Add(x => x.Label, "Name")
            .Add(x => x.Value, "Alice"));

        Assert.Contains("Name", cut.Find("label").TextContent);
        Assert.Equal("Alice", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void DisabledFlag_PropagatesToRootAndInput()
    {
        var cut = RenderComponent<TextInput>(p => p.Add(x => x.Disabled, true));

        var root = cut.Find(".cb-input");
        Assert.Contains("cb-input--disabled", root.ClassList);

        var input = cut.Find("input");
        Assert.True(input.HasAttribute("disabled"));
    }

    [Fact]
    public void RequiredFlag_AddsRequiredModifierAndAsterisk()
    {
        var cut = RenderComponent<TextInput>(p => p
            .Add(x => x.Label, "Email")
            .Add(x => x.Required, true));

        Assert.Contains("cb-input--required", cut.Find(".cb-input").ClassList);
        Assert.Contains("*", cut.Find("label").TextContent);
    }

    [Fact]
    public void ErrorMessage_RendersInvalidStateAndMessage()
    {
        var cut = RenderComponent<TextInput>(p => p
            .Add(x => x.ErrorMessage, "Boom"));

        Assert.Contains("cb-input--invalid", cut.Find(".cb-input").ClassList);
        Assert.Contains("Boom", cut.Find(".cb-validation").TextContent);
    }

    [Fact]
    public async Task ChangingValue_RaisesValueChangedAndOnChange()
    {
        string? bound = null;
        string? onChange = null;
        var cut = RenderComponent<TextInput>(p => p
            .Add(x => x.Value, "initial")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => bound = v))
            .Add(x => x.OnChange, EventCallback.Factory.Create<string?>(this, v => onChange = v)));

        await cut.Find("input").ChangeAsync(new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "typed" });

        Assert.Equal("typed", bound);
        Assert.Equal("typed", onChange);
    }

    [Fact]
    public void CssClass_IsAppendedToRoot()
    {
        var cut = RenderComponent<TextInput>(p => p.Add(x => x.CssClass, "mine"));
        Assert.Contains("mine", cut.Find(".cb-input").ClassList);
    }
}
