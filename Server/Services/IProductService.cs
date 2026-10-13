using Shared.Models;

namespace Server.Services;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
}
