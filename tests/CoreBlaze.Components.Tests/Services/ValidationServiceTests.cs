using System.ComponentModel.DataAnnotations;

namespace CoreBlaze.Components.Tests.Services;

public class ValidationServiceTests
{
    private sealed class Model
    {
        [Required] public string? Name { get; set; }
        [Range(1, 100)] public int Age { get; set; }
    }

    [Fact]
    public void TryValidate_ValidModel_ReturnsTrueAndEmptyResults()
    {
        var svc = new ValidationService();
        var ok = svc.TryValidate(new Model { Name = "Alice", Age = 30 }, out var results);
        Assert.True(ok);
        Assert.Empty(results);
    }

    [Fact]
    public void TryValidate_InvalidModel_ReturnsFalseWithMessages()
    {
        var svc = new ValidationService();
        var ok = svc.TryValidate(new Model { Name = null, Age = 0 }, out var results);
        Assert.False(ok);
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.MemberNames.Contains("Name"));
        Assert.Contains(results, r => r.MemberNames.Contains("Age"));
    }
}
