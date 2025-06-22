using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AWUI.Models;

public class Person
{
    [Required, NotNull]
    public string? Name { get; set; }

    [Required, NotNull]
    public string? Age { get; set; }
}
