using DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.ProductRepo
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> products, int totalRecords)> GetAllProductsAsync(int pageNumber, int pageSize); Task<Product> GetProductByIdAsync(int productId);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int productId);
        Task<(IEnumerable<Product>, int)> FilterProductsAsync(
            string searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, bool? isAvailable,
            int pageNumber, int pageSize);

    }
}
