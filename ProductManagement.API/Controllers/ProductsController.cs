using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.API.Data;
using Microsoft.AspNetCore.Authorization;
using ProductManagement.API.Models;
using System.Net;

namespace ProductManagement.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductmanagementContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductmanagementContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/products
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Product>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Product>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<Product>>>> GetProducts(
            [FromQuery] string? name,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            try
            {
                _logger.LogInformation("Getting products with filters: name={Name}, minPrice={MinPrice}, maxPrice={MaxPrice}",
                    name, minPrice, maxPrice);

                var query = _context.Products.Where(p => p.IsDelete == false);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(p => p.Name.Contains(name));
                }

                if (minPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= maxPrice.Value);
                }

                var products = await query.ToListAsync();
                _logger.LogInformation("Retrieved {Count} products", products.Count);

                return Ok(ApiResponse<IEnumerable<Product>>.SuccessResponse(products, "Products retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return HandleException<IEnumerable<Product>>(ex, "An error occurred while retrieving products");
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Product>>> GetProduct(int id)
        {
            try
            {
                _logger.LogInformation("Getting product with ID: {ProductId}", id);

                var product = await _context.Products.Where(p => p.IsDelete == false && p.Id == id).FirstOrDefaultAsync();

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return NotFound(ApiResponse<Product>.NotFoundResponse($"Product with ID {id} not found"));
                }

                _logger.LogInformation("Retrieved product with ID: {ProductId}", id);
                return Ok(ApiResponse<Product>.SuccessResponse(product, "Product retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
                return HandleException<Product>(ex, $"An error occurred while retrieving product with ID {id}");
            }
        }

        // POST: api/Products
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Product>>> CreateProduct(Product product)
        {
            try
            {
                _logger.LogInformation("Creating new product: {ProductName}", product.Name);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid product data submitted");
                    return BadRequest(ApiResponse<Product>.BadRequestResponse("Invalid product data"));
                }

                var newProduct = new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    IsDelete = false,
                    CreatedBy = User.Identity?.Name ?? "system",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product created successfully with ID: {ProductId}", newProduct.Id);
                return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id },
                    ApiResponse<Product>.CreatedResponse(newProduct, "Product created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return HandleException<Product>(ex, "An error occurred while creating the product");
            }
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Product>>> UpdateProduct(int id, Product product)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", id);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid product data submitted for update");
                    return BadRequest(ApiResponse<Product>.BadRequestResponse("Invalid product data"));
                }

                var existingProduct = await _context.Products.FindAsync(id);

                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for update", id);
                    return NotFound(ApiResponse<Product>.NotFoundResponse($"Product with ID {id} not found"));
                }

                UpdateProductProperties(existingProduct, product);

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Product with ID {ProductId} updated successfully", id);
                    return NoContent();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ProductExists(id))
                    {
                        _logger.LogWarning("Product with ID {ProductId} not found after concurrency check", id);
                        return NotFound(ApiResponse<Product>.NotFoundResponse($"Product with ID {id} not found"));
                    }
                    else
                    {
                        _logger.LogError(ex, "Concurrency error while updating product with ID {ProductId}", id);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID {ProductId}", id);
                return HandleException<Product>(ex, $"An error occurred while updating product with ID {id}");
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation("Soft deleting product with ID: {ProductId}", id);

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                    return NotFound(ApiResponse<object>.NotFoundResponse($"Product with ID {id} not found"));
                }

                MarkProductAsDeleted(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product with ID {ProductId} soft deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID {ProductId}", id);
                return HandleException<object>(ex, $"An error occurred while deleting product with ID {id}");
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        private void UpdateProductProperties(Product existingProduct, Product updatedProduct)
        {
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.UpdateBy = User.Identity?.Name;
            existingProduct.UpdatedAt = DateTime.UtcNow;
        }

        private void MarkProductAsDeleted(Product product)
        {
            product.IsDelete = true;
            product.DeletedBy = User.Identity?.Name;
            product.DeletedAt = DateTime.UtcNow;
            _context.Products.Update(product);
        }

        private ActionResult<ApiResponse<T>> HandleException<T>(Exception ex, string defaultMessage)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ApiResponse<T>.ServerErrorResponse(defaultMessage));
        }
    }
}