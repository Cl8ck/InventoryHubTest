namespace Shared.Models;

/// <summary>Represents a product, including its category, in API responses.</summary>
public sealed record Product(int Id, string Name, decimal Price, int Stock, Category Category);
