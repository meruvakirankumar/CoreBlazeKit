using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Tests.Grid;

public class DataGridTests : TestContext
{
    private sealed class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private static readonly List<Person> People = new()
    {
        new() { Id = 1, Name = "Charlie" },
        new() { Id = 2, Name = "Alice" },
        new() { Id = 3, Name = "Bob" },
    };

    private RenderFragment BasicColumns() => builder =>
    {
        builder.OpenComponent<GridColumn<Person>>(0);
        builder.AddAttribute(1, nameof(GridColumn<Person>.Field), "Id");
        builder.AddAttribute(2, nameof(GridColumn<Person>.Title), "ID");
        builder.CloseComponent();

        builder.OpenComponent<GridColumn<Person>>(3);
        builder.AddAttribute(4, nameof(GridColumn<Person>.Field), "Name");
        builder.AddAttribute(5, nameof(GridColumn<Person>.Title), "Name");
        builder.CloseComponent();
    };

    [Fact]
    public void RendersHeadersFromColumns()
    {
        var cut = RenderComponent<DataGrid<Person>>(p => p
            .Add(x => x.Data, People)
            .Add(x => x.Columns, BasicColumns()));

        var headers = cut.FindAll("thead tr:first-child th").Select(t => t.TextContent.Trim()).ToList();
        Assert.Contains("ID", headers);
        Assert.Contains("Name", headers);
    }

    [Fact]
    public void RendersOneRowPerItem_WithinPageSize()
    {
        var cut = RenderComponent<DataGrid<Person>>(p => p
            .Add(x => x.Data, People)
            .Add(x => x.PageSize, 10)
            .Add(x => x.Columns, BasicColumns()));

        var bodyRows = cut.FindAll("tbody tr.cb-grid__row");
        Assert.Equal(People.Count, bodyRows.Count);
    }

    [Fact]
    public void EmptyData_RendersEmptyMessage()
    {
        var cut = RenderComponent<DataGrid<Person>>(p => p
            .Add(x => x.Data, new List<Person>())
            .Add(x => x.EmptyText, "Nothing here")
            .Add(x => x.Columns, BasicColumns()));

        Assert.Contains("Nothing here", cut.Find(".cb-grid__empty").TextContent);
    }

    [Fact]
    public void PagingLimitsVisibleRows()
    {
        var many = Enumerable.Range(1, 25).Select(i => new Person { Id = i, Name = $"P{i}" }).ToList();
        var cut = RenderComponent<DataGrid<Person>>(p => p
            .Add(x => x.Data, many)
            .Add(x => x.PageSize, 5)
            .Add(x => x.Columns, BasicColumns()));

        Assert.Equal(5, cut.FindAll("tbody tr.cb-grid__row").Count);
    }

    [Fact]
    public void ClickingSortableHeader_SortsAscendingThenDescending()
    {
        var cut = RenderComponent<DataGrid<Person>>(p => p
            .Add(x => x.Data, People)
            .Add(x => x.PageSize, 10)
            .Add(x => x.Columns, BasicColumns()));

        // Second header is "Name"
        var headers = cut.FindAll("thead tr:first-child th.cb-grid__th--sortable");
        headers[1].Click(); // ascending

        var firstCellsAsc = cut.FindAll("tbody tr.cb-grid__row td:nth-child(2)").Select(c => c.TextContent.Trim()).ToList();
        Assert.Equal(new[] { "Alice", "Bob", "Charlie" }, firstCellsAsc);

        headers = cut.FindAll("thead tr:first-child th.cb-grid__th--sortable");
        headers[1].Click(); // descending

        var firstCellsDesc = cut.FindAll("tbody tr.cb-grid__row td:nth-child(2)").Select(c => c.TextContent.Trim()).ToList();
        Assert.Equal(new[] { "Charlie", "Bob", "Alice" }, firstCellsDesc);
    }

    [Fact]
    public void CommandColumn_ShowsEditAndDeleteButtons_WhenAllowed()
    {
        var cut = RenderComponent<DataGrid<Person>>(p => p
            .Add(x => x.Data, People)
            .Add(x => x.Columns, BasicColumns()));

        var buttons = cut.FindAll("tbody tr.cb-grid__row:first-child .cb-grid__td--commands button")
            .Select(b => b.TextContent.Trim()).ToList();

        Assert.Contains("Edit", buttons);
        Assert.Contains("Delete", buttons);
    }
}
