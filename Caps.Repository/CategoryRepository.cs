using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSGM.Core;
using MSGM.Entity;
using MSGM.Entity.Models;

namespace MSGM.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {

        }
        public ApplicationDbContext CapsContext => Context as ApplicationDbContext;
        public List<Category> GetActiveCategories()
        {
            return CapsContext.Category.Where(c => c.Status == true).ToList();
        }

        public Category? GetCategoryById(int id)
        {
            return CapsContext.Category.FirstOrDefault(c => c.Id == id);
        }
    }
}