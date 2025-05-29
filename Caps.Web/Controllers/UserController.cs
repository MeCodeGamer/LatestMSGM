using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MSGM.Core;
using MSGM.Web.ViewModels;
using Serilog;

namespace MSGM.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IunitOfWork _unitOfWork;
        public UserController(ILogger<AccountController> logger, IunitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public async Task<IActionResult> Shop()
        {
            var model = new vmShop();
            try
            {
                model.productList = _unitOfWork.product.GetActiveProducts();
                model.categoryList = _unitOfWork.category.GetActiveCategories();

                return await Task.FromResult<IActionResult>(View(model));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return await Task.FromResult<IActionResult>(View(model));
            }
        }
    }
}
