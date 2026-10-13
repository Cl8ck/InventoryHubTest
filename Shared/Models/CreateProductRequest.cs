using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

/// <summary>Information required to add a product to the inventory.</summary>
public sealed record CreateProductRequest : IValidatableObject
{
    /// <summary>Gets the product name.</summary>
    [Required, StringLength(200)]
    public required string Name { get; init; }

    /// <summary>Gets the product price.</summary>
    public required decimal Price { get; init; }

    /// <summary>Gets the number of units currently in stock.</summary>
    [Range(0, int.MaxValue)]
    public required int Stock { get; init; }

    /// <summary>Gets the product category name.</summary>
    [Required, StringLength(100)]
    public required string CategoryName { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return new ValidationResult("Name is required.", [nameof(Name)]);
        }

        if (Price is < 0.01m or > 999999.99m)
        {
            yield return new ValidationResult(
                "Price must be between 0.01 and 999999.99.",
                [nameof(Price)]);
        }

        if (Stock < 0)
        {
            yield return new ValidationResult("Stock cannot be negative.", [nameof(Stock)]);
        }

        if (string.IsNullOrWhiteSpace(CategoryName))
        {
            yield return new ValidationResult("Category is required.", [nameof(CategoryName)]);
        }
    }
}
