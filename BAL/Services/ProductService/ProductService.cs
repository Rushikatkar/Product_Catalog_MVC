using DAL.Models.Entities;
using DAL.Repositories.ProductRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTOs;

namespace BAL.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<(IEnumerable<Product>, int)> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            return await _productRepository.GetAllProductsAsync(pageNumber, pageSize);
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _productRepository.GetProductByIdAsync(productId);
        }

        public async Task AddProductAsync(AddProductDto addProductDto)
        {
            var product = new Product
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                Price = addProductDto.Price,
                CategoryId = addProductDto.CategoryId,
                IsAvailable = addProductDto.IsAvailable
            };

            if (addProductDto.Image != null)
            {
                // Get the path to the wwwroot folder
                var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // Ensure the Images folder exists
                var imagesFolderPath = Path.Combine(wwwRootPath, "Images");
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                // Construct the full file path
                var filePath = Path.Combine(imagesFolderPath, addProductDto.Image.FileName);

                // Save the file to the Images folder
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await addProductDto.Image.CopyToAsync(stream);
                }

                // Store the relative path in the database
                product.ImageURL = Path.Combine("Images", addProductDto.Image.FileName).Replace("\\", "/"); // Use forward slashes for URLs
            }


            await _productRepository.AddProductAsync(product);
        }


        public async Task UpdateProductAsync(int id,UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product != null)
            {
                product.Name = updateProductDto.Name;
                product.Description = updateProductDto.Description;
                product.Price = updateProductDto.Price;
                product.CategoryId = updateProductDto.CategoryId;
                product.IsAvailable = updateProductDto.IsAvailable;

                if (updateProductDto.Image != null)
                {
                    var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }
                    var filePath = Path.Combine(imagesFolder, updateProductDto.Image.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await updateProductDto.Image.CopyToAsync(stream);
                    }
                    product.ImageURL = Path.Combine("Images", updateProductDto.Image.FileName).Replace("\\", "/");
                }


                await _productRepository.UpdateProductAsync(product);
            }
        }

        public async Task DeleteProductAsync(int productId)
        {
            await _productRepository.DeleteProductAsync(productId);
        }

        public async Task<(IEnumerable<Product>, int)> FilterProductsAsync(
            string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, bool? isAvailable,
            int pageNumber, int pageSize)
        {
            return await _productRepository.FilterProductsAsync(
                searchTerm, categoryId, minPrice, maxPrice, isAvailable, pageNumber, pageSize);
        }

       

    }
}
