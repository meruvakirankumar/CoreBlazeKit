using CoreBlaze.Demo.Models;

namespace CoreBlaze.Demo.Services;

/// <summary>In-memory sample data source for the demo pages.</summary>
public sealed class PersonRepository
{
    private readonly List<Person> _people = new()
    {
        new() { Id = 1, Name = "Alice Anderson",  Age = 32, Department = "Engineering", HireDate = new DateTime(2019, 5, 12), IsActive = true },
        new() { Id = 2, Name = "Bob Baker",       Age = 45, Department = "Sales",       HireDate = new DateTime(2015, 1,  4), IsActive = true },
        new() { Id = 3, Name = "Carol Chen",      Age = 28, Department = "Marketing",   HireDate = new DateTime(2021, 9, 20), IsActive = true },
        new() { Id = 4, Name = "David Davis",     Age = 51, Department = "Finance",     HireDate = new DateTime(2010, 3, 15), IsActive = false },
        new() { Id = 5, Name = "Eva Escobar",     Age = 39, Department = "Engineering", HireDate = new DateTime(2018, 7,  1), IsActive = true },
        new() { Id = 6, Name = "Frank Foster",    Age = 44, Department = "Support",     HireDate = new DateTime(2012,10, 22), IsActive = true },
        new() { Id = 7, Name = "Grace Green",     Age = 26, Department = "Engineering", HireDate = new DateTime(2023, 2,  8), IsActive = true },
        new() { Id = 8, Name = "Henry Hall",      Age = 37, Department = "Sales",       HireDate = new DateTime(2016,11, 30), IsActive = true },
        new() { Id = 9, Name = "Isabel Ibarra",   Age = 30, Department = "Marketing",   HireDate = new DateTime(2020, 4,  4), IsActive = false },
        new() { Id =10, Name = "Jonas Jensen",    Age = 33, Department = "Engineering", HireDate = new DateTime(2017, 8, 19), IsActive = true },
        new() { Id =11, Name = "Kim Kowalski",    Age = 29, Department = "Support",     HireDate = new DateTime(2022, 6,  6), IsActive = true },
        new() { Id =12, Name = "Lena Lopez",      Age = 41, Department = "Finance",     HireDate = new DateTime(2014, 2, 27), IsActive = true },
    };

    /// <summary>Returns a new list backed by the same items (safe to mutate for the demo).</summary>
    public List<Person> GetAll() => _people.ToList();

    /// <summary>Departments used for dropdown demos.</summary>
    public IReadOnlyList<string> Departments { get; } = new[]
    {
        "Engineering", "Sales", "Marketing", "Finance", "Support"
    };
}
