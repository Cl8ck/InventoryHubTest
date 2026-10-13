using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Shared.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    /// <summary>Gets all products with their category details.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<Product>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await productService.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    /// <summary>Gets a product by its identifier, including its category details.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<Product>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await productService.GetByIdAsync(id, cancellationToken);
        return product is null
            ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found")
            : Ok(product);
    }

    /// <summary>Adds a product to the current session's inventory.</summary>
    [HttpPost]
    [ProducesResponseType<Product>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
}
