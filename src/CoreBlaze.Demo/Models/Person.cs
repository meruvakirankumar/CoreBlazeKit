using System.ComponentModel.DataAnnotations;

namespace CoreBlaze.Demo.Models;

/// <summary>Sample domain object used by the DataGrid and Form demo pages.</summary>
public sealed class Person
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Full name.</summary>
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Name must be 2–80 characters.")]
    public string? Name { get; set; }

    /// <summary>Age in years.</summary>
    [Range(0, 130, ErrorMessage = "Age must be between 0 and 130.")]
    public int Age { get; set; }

    /// <summary>Department code.</summary>
    [Required(ErrorMessage = "Please pick a department.")]
    public string? Department { get; set; }

    /// <summary>Hire date.</summary>
    public DateTime? HireDate { get; set; }

    /// <summary>Whether the person is currently active.</summary>
    public bool IsActive { get; set; } = true;
}
