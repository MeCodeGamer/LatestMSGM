using MSGM.Core;
using MSGM.Entity.Models;
using MSGM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MSGM.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {

        }
        public ApplicationDbContext CapsContext => Context as ApplicationDbContext;
        public List<Product> GetActiveProducts()
        {
            return CapsContext.Product.Where(c => c.Status == true).ToList();
        }
        public List<Product> GetAllProducts()
        {
            return CapsContext.Product.Include(p => p.Category).ToList();
        }

        public Product? GetProductById(int id)
        {
            return CapsContext.Product.FirstOrDefault(c => c.Id == id);
        }
    }
}
