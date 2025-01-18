using BAL.Services.CategoryService;
using BAL.Services.ProductService;
using DAL.DTOs;
using DAL.Models.Entities;
using DAL.Repositories.CategoryRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Presentation_Layer.Controllers
{
    [Route("products")] // Base route for the controller
    [Authorize] // Restrict access to authenticated users with the Admin role
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;


        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet("allproducts")]
        [Authorize(Roles = "UserAndAdmin")] // Both Admin and User can view products
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string searchQuery = null, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, bool? isAvailable = null)
        {

            var (products, totalRecords) = await _productService.FilterProductsAsync(
                searchTerm: searchQuery,
                categoryId: categoryId,
                minPrice: minPrice,
                maxPrice: maxPrice,
                isAvailable: isAvailable,
                pageNumber: pageNumber,
                pageSize: pageSize
            );

            // Passing the filtered data to the view
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.IsAvailable = isAvailable;

            // Populate categories for filtering
            var categories = _categoryService.GetAllCategories()
                                              .Select(c => new SelectListItem
                                              {
                                                  Value = c.CategoryId.ToString(),
                                                  Text = c.CategoryName
                                              }).ToList();

            ViewBag.Categories = categories;

            return View(products);
        }






        // GET: products/details/5
        [HttpGet("details/{id}")]
        [Authorize(Roles = "UserAndAdmin")] // Both Admin and User can view products
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpGet("create")]
        [Authorize(Roles = "AdminOnly")] // Only Admin can create a product
        public IActionResult Create()
        {
            var categories = _categoryService.GetAllCategories()
                              .Select(c => new SelectListItem
                              {
                                  Value = c.CategoryId.ToString(),
                                  Text = c.CategoryName
                              }).ToList();

            if (categories == null || categories.Count == 0)
            {
                // Optionally, add a default option in case the categories are empty
                categories.Add(new SelectListItem
                {
                    Value = "",
                    Text = "No categories available"
                });
            }

            ViewBag.Categories = categories;

            return View();
        }


        // POST: products/create
        [HttpPost("create")]
        [Authorize(Roles = "AdminOnly")] // Only Admin can edit a product
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProductDto addProductDto)
        {
            if (ModelState.IsValid)
            {
                // Pass the data (including the file) directly to the service layer
                await _productService.AddProductAsync(addProductDto);

                // Redirect to index or another view after successful creation
                return RedirectToAction(nameof(Index));
            }

            // Re-fetch categories in case of validation errors
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "CategoryId", "CategoryName");
            return View(addProductDto);
        }

        // GET: products/edit/5
        [HttpGet("edit/{id}")]
        [Authorize(Roles = "AdminOnly")] // Only Admin can create a product
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Fetch categories just like the Create action
            var categories = _categoryService.GetAllCategories()
                                      .Select(c => new SelectListItem
                                      {
                                          Value = c.CategoryId.ToString(),
                                          Text = c.CategoryName
                                      }).ToList();

            // Initialize the UpdateProductDto with the product data
            var updateProductDto = new UpdateProductDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                IsAvailable = product.IsAvailable,
                // ImageURL is not passed to the view anymore
            };

            // Pass categories and product data to the view
            ViewBag.Categories = categories;
            return View(updateProductDto);
        }

        // POST: products/edit/5
        [HttpPost("edit/{id}")]
        [Authorize(Roles = "AdminOnly")] // Only Admin can edit a product
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDto updateProductDto, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                // Call service method to update the product with the DTO
                await _productService.UpdateProductAsync(id, updateProductDto);
                return RedirectToAction(nameof(Index));
            }

            // Re-fetch categories in case of validation errors
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "CategoryId", "CategoryName");
            return View(updateProductDto);
        }



        // GET: products/delete/5
        [HttpGet("delete/{id}")]
        [Authorize(Roles = "AdminOnly")] // Only Admin can edit a product
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: products/delete/5
        [HttpPost("delete/{id}")]
        [Authorize(Roles = "AdminOnly")] // Only Admin can edit a product
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
