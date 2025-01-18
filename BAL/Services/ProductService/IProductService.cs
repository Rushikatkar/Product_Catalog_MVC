using DAL.DTOs;
using DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.ProductService
{
    public interface IProductService
    {
        Task<(IEnumerable<Product> products, int totalRecords)> GetAllProductsAsync(int pageNumber, int pageSize); 
        Task<Product> GetProductByIdAsync(int productId);
        Task AddProductAsync(AddProductDto addProductDto);
        Task UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task DeleteProductAsync(int productId);
        Task<(IEnumerable<Product>, int)> FilterProductsAsync(
            string searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, bool? isAvailable,
            int pageNumber, int pageSize);

    }
}
