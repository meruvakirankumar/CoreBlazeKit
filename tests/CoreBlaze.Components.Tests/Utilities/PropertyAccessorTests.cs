namespace CoreBlaze.Components.Tests.Utilities;

public class PropertyAccessorTests
{
    private sealed class Sample
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string ReadOnly { get; } = "fixed";
    }

    [Fact]
    public void For_UnknownProperty_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => PropertyAccessor.For(typeof(Sample), "Missing"));
    }

    [Fact]
    public void TryFor_UnknownProperty_ReturnsNull()
    {
        Assert.Null(PropertyAccessor.TryFor(typeof(Sample), "Missing"));
    }

    [Fact]
    public void Getter_ReadsPropertyValue()
    {
        var s = new Sample { Id = 42, Name = "hello" };
        var (getter, _, _, _) = PropertyAccessor.For(typeof(Sample), "Name");
        Assert.Equal("hello", getter(s));
    }

    [Fact]
    public void Setter_WritesPropertyValue()
    {
        var s = new Sample();
        var (_, setter, _, _) = PropertyAccessor.For(typeof(Sample), "Id");
        Assert.NotNull(setter);
        setter!(s, 123);
        Assert.Equal(123, s.Id);
    }

    [Fact]
    public void Setter_IsNullForReadOnlyProperty()
    {
        var (_, setter, _, _) = PropertyAccessor.For(typeof(Sample), "ReadOnly");
        Assert.Null(setter);
    }

    [Fact]
    public void GetSetValue_HighLevelHelpers_RoundTrip()
    {
        var s = new Sample();
        PropertyAccessor.SetValue(s, "Name", "world");
        Assert.Equal("world", PropertyAccessor.GetValue(s, "Name"));
    }

    [Fact]
    public void Accessor_IsCached_SameDelegateReturned()
    {
        var first = PropertyAccessor.For(typeof(Sample), "Id");
        var second = PropertyAccessor.For(typeof(Sample), "Id");
        Assert.Same(first.Getter, second.Getter);
    }
}
