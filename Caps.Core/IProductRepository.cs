using MSGM.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSGM.Core
{
    public interface IProductRepository : IRepository<Product>
    {
        List<Product> GetActiveProducts();
        public Product? GetProductById(int id);
    }
}
