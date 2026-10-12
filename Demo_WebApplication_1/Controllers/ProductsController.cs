
using System.ComponentModel.DataAnnotations;
using Demo_Webapplication_1.Data;
using Demo_Webapplication_1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo_Webapplication_1.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // POST: /api/products
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(
        [FromBody] ProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            Category = request.Category.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = product.Id },
            product);
    }

    // GET: /api/products?category=Books&search=pen
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
        [FromQuery] string? category,
        [FromQuery] string? search)
    {
        IQueryable<Product> query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(category))
        {
            var categoryValue = category.Trim();

            query = query.Where(p => p.Category == categoryValue);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchValue = search.Trim();

            query = query.Where(p => p.Name.Contains(searchValue));
        }

        var products = await query
            .OrderBy(p => p.Id)
            .ToListAsync();

        return Ok(products);
    }

    // GET: /api/products/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        if (id <= 0)
            return NotFound();

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound(new { message = "Product not found." });

        return Ok(product);
    }

    // PUT: /api/products/1
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> UpdateProduct(
        int id,
        [FromBody] ProductRequest request)
    {
        if (id <= 0)
            return NotFound();

        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound(new { message = "Product not found." });

        product.Name = request.Name.Trim();
        product.Description = request.Description;
        product.Category = request.Category.Trim();
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;

        await _context.SaveChangesAsync();

        return Ok(product);
    }

    // DELETE: /api/products/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (id <= 0)
            return NotFound();

        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound(new { message = "Product not found." });

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

// Request body validation
public class ProductRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
}
