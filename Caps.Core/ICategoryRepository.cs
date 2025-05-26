using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSGM.Entity.Models;

namespace MSGM.Core
{
    public interface ICategoryRepository : IRepository<Category>
    {
        List<Category> GetActiveCategories();
        public Category? GetCategoryById(int id);
    }
}