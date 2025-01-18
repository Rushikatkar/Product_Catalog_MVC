using DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.CategoryRepo
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
    }
}
