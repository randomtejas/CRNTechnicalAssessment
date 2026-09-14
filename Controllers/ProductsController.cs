using Microsoft.AspNetCore.Mvc;
using SDTechnicalAssessment.Application.DTOs;
using SDTechnicalAssessment.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace SDTechnicalAssessment.Controllers
{

    // ------------------------------------------------------------
    // [Authorize] means every endpoint in this controller requires
    // a valid JWT access token.
    //
    // Without a valid token, the API returns:
    // 401 Unauthorized
    // ------------------------------------------------------------
    [Authorize]

    // API controller responsible for Product endpoints.
    [ApiController]

    // Base URL will be /api/products
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IValidator<CreateProductDto> _createProductValidator;

        // ------------------------------------------------------------
        // Constructor.
        //
        // Product service and CreateProductValidator are injected
        // through Dependency Injection.
        // ------------------------------------------------------------
        public ProductsController(
            IProductService service,
            IValidator<CreateProductDto> createProductValidator)
        {
            _service = service;
            _createProductValidator = createProductValidator;
        }

        // GET: api/products
        // Returns all products.
      
        [HttpGet]
        public async Task<IActionResult> GetAll(
    int pageNumber = 1,
    int pageSize = 10)
        {
            // Get the requested page of products from the service.
            var products = await _service.GetAllAsync(
                pageNumber,
                pageSize);

            // Return the paginated result to the client.
            return Ok(products);
        }

        // GET: api/products/5
        // Returns a specific product.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/products
        // Creates a new product.
        //[HttpPost]
        //public async Task<IActionResult> Create(CreateProductDto dto)
        //{
        //    // Validate the input data
        //    var validationResult = await _createProductValidator.ValidateAsync(dto);
        //    if (!validationResult.IsValid)
        //    {
        //        return BadRequest(validationResult.Errors);
        //    }

        //    var product = await _service.CreateAsync(dto);

        //    return CreatedAtAction(
        //        nameof(GetById),
        //        new { id = product.Id },
        //        product);
        //}
        // POST: api/products
        // Creates a new product after validating the request.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            // --------------------------------------------------------
            // Run FluentValidation manually.
            //
            // This checks ProductName and CreatedBy before
            // the request is sent to the service/database.
            // --------------------------------------------------------
            var validationResult = await _createProductValidator.ValidateAsync(dto);

            // --------------------------------------------------------
            // If validation fails, return HTTP 400 Bad Request.
            //
            // The database will NOT be called when the data is invalid.
            // --------------------------------------------------------
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // --------------------------------------------------------
            // Only valid data reaches the service.
            // --------------------------------------------------------
            var product = await _service.CreateAsync(dto);

            // --------------------------------------------------------
            // Return HTTP 201 Created.
            // --------------------------------------------------------
            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id },
                product);
        }

        // PUT: api/products/5
        // Updates an existing product.
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateProductDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/products/5
        // Deletes an existing product.
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}