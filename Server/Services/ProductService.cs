using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Shared.Models;

namespace Server.Services;

public sealed class ProductService : IProductService
{
    private const string SessionProductsKey = "InventoryHub.Products";
    private static int _lastProductId = 3;

    private static readonly IReadOnlyList<Product> DefaultProducts =
    [
        new Product(1, "Wireless Mouse", 29.99m, 42, new Category(1, "Accessories")),
        new Product(2, "Mechanical Keyboard", 89.99m, 18, new Category(1, "Accessories")),
        new Product(3, "USB-C Hub", 49.50m, 27, new Category(2, "Adapters"))
    ];

    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        var session = await GetSessionAsync(cancellationToken);
        return [.. DefaultProducts, .. ReadSessionProducts(session)];
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var products = await GetAllAsync(cancellationToken);
        return products.FirstOrDefault(product => product.Id == id);
    }

    public async Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var session = await GetSessionAsync(cancellationToken);
        var sessionProducts = ReadSessionProducts(session);
        var products = DefaultProducts.Concat(sessionProducts).ToArray();
        var category = products
            .Select(product => product.Category)
            .FirstOrDefault(existingCategory =>
                string.Equals(existingCategory.Name, request.CategoryName.Trim(), StringComparison.OrdinalIgnoreCase));

        if (category is null)
        {
            var categoryId = products.Select(product => product.Category.Id).DefaultIfEmpty(0).Max() + 1;
            category = new Category(categoryId, request.CategoryName.Trim());
        }

        var product = new Product(
            Interlocked.Increment(ref _lastProductId),
            request.Name.Trim(),
            request.Price,
            request.Stock,
            category);

        sessionProducts.Add(product);
        session.SetString(SessionProductsKey, JsonSerializer.Serialize(sessionProducts));
        return product;
    }

    private async Task<ISession> GetSessionAsync(CancellationToken cancellationToken)
    {
        var session = _httpContextAccessor.HttpContext?.Session
            ?? throw new InvalidOperationException("The product API requires an active session.");
        await session.LoadAsync(cancellationToken);
        return session;
    }

    private static List<Product> ReadSessionProducts(ISession session)
    {
        var productsJson = session.GetString(SessionProductsKey);
        return productsJson is null
            ? []
            : JsonSerializer.Deserialize<List<Product>>(productsJson)
                ?? throw new JsonException("The stored session product list is invalid.");
    }
}
