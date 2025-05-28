using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using MSGM.Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MSGM.Entity.Models;
using MSGM.Core;

namespace MSGM.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IunitOfWork _unitOfWork;
        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILogger<AccountController> logger, IunitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(vmLogin model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return View("Dashboard");
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid UserName or Password!";
                    @ViewBag.Notification = "Invalid UserName or Password!";
                }

                if (result.IsLockedOut)
                {
                    TempData["ErrorMessage"] = "This User is Locked!";
                    @ViewBag.Notification = "This User is Locked!";
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                return await Task.FromResult<IActionResult>(View());
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return await Task.FromResult<IActionResult>(View());
            }
        }

        //<==================================================================User Working ===========================================================>

        public async Task<IActionResult> NewUser()
        {
            try
            {
                return await Task.FromResult<IActionResult>(View());
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return await Task.FromResult<IActionResult>(View());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(vmUsers model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("NewUser", model);

                var user = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "User created successfully!";
                }
                foreach (var error in result.Errors)
                {
                    TempData["ErrorMessage"] = "User Creation Failed,Error : " + error.Description;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error While Adding User, Error : " + ex.Message);
                TempData["ErrorMessage"] = "Error While Adding User, Error : " + ex.Message;
            }
            return View("NewUser", model);
        }
        public async Task<IActionResult> ManageUser()
        {
            try
            {
                var users = await Task.FromResult(_userManager.Users.ToList());
                return await Task.FromResult<IActionResult>(View(users));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                TempData["ErrorMessage"] = "Could not retrieve users.";
                return await Task.FromResult<IActionResult>(View(new List<IdentityUser>()));
            }
        }
        public async Task<IActionResult> EditUser(string Id)
        {
            try
            {
                var model = new vmManageUser();
                if (User == null || !User.Identity.IsAuthenticated)
                {
                    TempData["ErrorMessage"] = "Unauthorized access!";
                    return RedirectToAction("Logout", "Account");
                }

                var decodedBytes = Convert.FromBase64String(Id);
                var decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);

                if (!Guid.TryParse(decodedString, out Guid userGuid))
                {
                    TempData["ErrorMessage"] = "Invalid User ID format!";
                    return await Task.FromResult<ActionResult>(View(model));
                }

                model.UserId = userGuid.ToString();
                var userDetails = await _userManager.FindByIdAsync(model.UserId);
                if (userDetails == null)
                {
                    TempData["ErrorMessage"] = "Failed to get the selected user's detail!";
                    return await Task.FromResult<IActionResult>(View(model));
                }
                model.UserName = userDetails.UserName;
                model.Email = userDetails.Email;

                return await Task.FromResult<IActionResult>(View(model));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return await Task.FromResult<IActionResult>(View());
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUser(vmManageUser model)
        {
            if (!string.IsNullOrWhiteSpace(model.UserName) && !string.IsNullOrWhiteSpace(model.Email))
            {
                try
                {
                    var userDetails = await _userManager.FindByIdAsync(model.UserId);
                    if (userDetails == null)
                    {
                        return Json(new { success = false, message = "Failed to get the selected user's details!" });
                    }
                    userDetails.UserName = model.UserName;
                    userDetails.Email = model.Email;
                    var result = await _userManager.UpdateAsync(userDetails);
                    if (result.Succeeded)
                    {
                        return Json(new { success = true, message = "User updated successfully!" });
                    }
                    else
                    {
                        var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                        return Json(new { success = false, message = $"Update failed: {errors}" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error: {ex.Message}" });
                }
            }
            return Json(new { success = false, message = "Update failed. Please check the input." });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, message = "Invalid user ID." });
                }

                var userDetails = await _userManager.FindByIdAsync(id);
                if (userDetails == null)
                {
                    return Json(new { success = false, message = "Failed to get the selected user's details!" });
                }
                var result = await _userManager.DeleteAsync(userDetails);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "User deleted successfully!" });
                }
                else
                {
                    var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                    return Json(new { success = false, message = $"Delete failed: {errors}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        //<==================================================================Category Working ===========================================================>

        public async Task<IActionResult> NewCategory()
        {
            try
            {
                return await Task.FromResult<IActionResult>(View());
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return await Task.FromResult<IActionResult>(View());
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(vmCategory model)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                {
                    TempData["ErrorMessage"] = "Unauthorized access!";
                    return RedirectToAction("Logout", "Account");
                }

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Required Fields are Empty!";
                    return await Task.FromResult<IActionResult>(View("NewCategory", model));
                }

                var category = new Category
                {
                    Title = model.Title,
                    Description = model.Description,
                    Status = true,
                    CreatedBy = User?.Identity?.Name,
                    CreatedOn = DateTime.Now
                };

                _unitOfWork.category.Add(category);
                int result = await _unitOfWork.RtCompleteAsync();
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Category added successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to save category. Please try again.";
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error While Adding Category, Error : " + ex.Message);
                TempData["ErrorMessage"] = "Error While Adding Category, Error : " + ex.Message;
            }
            return await Task.FromResult<IActionResult>(View("NewCategory", model));
        }
        public async Task<IActionResult> ManageCategory()
        {
            try
            {
                var model = new vmCategory();
                if (User == null || !User.Identity.IsAuthenticated)
                {
                    TempData["ErrorMessage"] = "Unauthorized access!";
                    return RedirectToAction("Logout", "Account");
                }

                model.categoryList = _unitOfWork.category.GetAll().ToList();
                if (model.categoryList == null || model.categoryList.Count == 0)
                {
                    TempData["ErrorMessage"] = "No active Categories Found!";
                    return await Task.FromResult<IActionResult>(View(model));
                }

                return await Task.FromResult<IActionResult>(View(model));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return await Task.FromResult<IActionResult>(View());
            }
        }
        public async Task<IActionResult> EditCategory(string Id)
        {
            try
            {
                var model = new vmCategory();
                if (User == null || !User.Identity.IsAuthenticated)
                {
                    TempData["ErrorMessage"] = "Unauthorized access!";
                    return RedirectToAction("Logout", "Account");
                }

                var decodedBytes = Convert.FromBase64String(Id);
                var decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);

                //ID validation and setting in Model
                if (!int.TryParse(decodedString, out int CategoryID))
                {
                    TempData["ErrorMessage"] = "Invalid Category Id Format!";
                    return await Task.FromResult<ActionResult>(View(model));
                }
                model.CategoryId = CategoryID;

                var CategoryDetails = _unitOfWork.category.GetCategoryById(CategoryID);
                if (CategoryDetails == null)
                {
                    TempData["ErrorMessage"] = "Failed to get the selected Category's detail!";
                    return await Task.FromResult<IActionResult>(View(model));
                }
                model.CategoryTitle = CategoryDetails.Title;
                model.CategoryDescription = CategoryDetails.Description;
                model.CategoryStatus = CategoryDetails.Status;

                return await Task.FromResult<IActionResult>(View(model));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return await Task.FromResult<IActionResult>(View());
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(vmCategory model)
        {
            if (!string.IsNullOrWhiteSpace(model.CategoryTitle) && !string.IsNullOrWhiteSpace(model.CategoryDescription)
                && model.CategoryId.HasValue && model.CategoryId.Value > 0)
            {
                try
                {
                    var UpdateCategory = _unitOfWork.category.GetCategoryById(model.CategoryId.Value);
                    if (UpdateCategory == null)
                    {
                        return Json(new { success = false, message = "Failed to get the selected Category's detail!" });
                    }
                    UpdateCategory.Title = model.CategoryTitle;
                    UpdateCategory.Description = model.CategoryDescription;
                    UpdateCategory.Status = model.CategoryStatus;
                    UpdateCategory.ModifiedBy = User?.Identity?.Name;
                    UpdateCategory.ModifiedOn = DateTime.Now;
                    _unitOfWork.category.Update(UpdateCategory);
                    int result = await _unitOfWork.RtCompleteAsync();
                    if (result == 0)
                    {
                        return Json(new { success = false, message = "Failed to update category. Please try again." });
                    }
                    return Json(new { success = true, message = "Category updated successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error: {ex.Message}" });
                }
            }
            return Json(new { success = false, message = "Update failed. Please check the input." });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { success = false, message = "Invalid category ID." });
                }

                var category = _unitOfWork.category.GetCategoryById(id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Category not found." });
                }
                _unitOfWork.category.Remove(category);
                int result = await _unitOfWork.RtCompleteAsync();
                if (result == 0)
                {
                    return Json(new { success = false, message = "Failed to delete category. Please try again." });
                }

                return Json(new { success = true, message = "Category deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }


        //<==================================================================Product Working ===========================================================>
        
        public void LoadLov()
        {
            ViewBag.Categories = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "0",
                        Text = "-- Select a Category --"
                    }
                }.Concat(_unitOfWork.category.GetActiveCategories().ToList().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Title.ToUpper()}"
                })), "Value", "Text");
        }
        public async Task<IActionResult> NewProduct()
        {
            try
            {
                LoadLov();
                return await Task.FromResult<IActionResult>(View());
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return await Task.FromResult<IActionResult>(View());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(vmProduct model)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                {
                    TempData["ErrorMessage"] = "Unauthorized access!";
                    return RedirectToAction("Logout", "Account");
                }

                LoadLov();

                string FileName = Path.GetFileName(model.Image.FileName);
                string Ext = Path.GetExtension(model.Image.FileName);

                if (Ext.ToLower() == ".jpg" || Ext.ToLower() == ".png" || Ext.ToLower() == ".jpeg")
                {
                    string FilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\DataFiles", FileName);

                    if (System.IO.File.Exists(FilePath))
                    {
                        TempData["ErrorMessage"] = "A file with the same name already exists. Please rename the file and try again.";
                        return await Task.FromResult<IActionResult>(View("NewProduct", model));
                    }

                    using (var fs = new FileStream(FilePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fs);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please select a valid image file (jpg, png, jpeg).";
                    return await Task.FromResult<IActionResult>(View("NewProduct", model));
                }


                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Required Fields are Empty!";
                    return await Task.FromResult<IActionResult>(View("NewProduct", model));
                }

                var product = new Product
                {
                    CatId = model.CatId,
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    Image = model.Image.FileName.ToString(),
                    Status = model.Status,
                    CreatedBy = User?.Identity?.Name,
                    CreatedOn = DateTime.Now
                };

                _unitOfWork.product.Add(product);
                int result = await _unitOfWork.RtCompleteAsync();
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Product added successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to save product. Please try again.";
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error While Adding Category, Error : " + ex.Message);
                TempData["ErrorMessage"] = "Error While Adding Product, Error : " + ex.Message;
            }
            return await Task.FromResult<IActionResult>(View("NewProduct", model));
        }

        public async Task<IActionResult> ManageProduct()
        {
            try
            {
                var model = new vmProduct();
                if (User == null || !User.Identity.IsAuthenticated)
                {
                    TempData["ErrorMessage"] = "Unauthorized access!";
                    return RedirectToAction("Logout", "Account");
                }

                model.productlist = _unitOfWork.product.GetAllProducts();
                if (model.productlist == null || model.productlist.Count == 0)
                {
                    TempData["ErrorMessage"] = "No Active Products Found!";
                    return await Task.FromResult<IActionResult>(View(model));
                }

                return await Task.FromResult<IActionResult>(View(model));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return await Task.FromResult<IActionResult>(View());
            }
        }

        public async Task<IActionResult> EditProduct(string Id)
        {
            try
            {
                var model = new vmProduct();
                if (User == null || !User.Identity.IsAuthenticated)
                {
                    TempData["ErrorMessage"] = "Unauthorized access!";
                    return RedirectToAction("Logout", "Account");
                }

                var decodedBytes = Convert.FromBase64String(Id);
                var decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);

                //ID validation and setting in Model
                if (!int.TryParse(decodedString, out int ProductID))
                {
                    TempData["ErrorMessage"] = "Invalid Product Id Format!";
                    return await Task.FromResult<ActionResult>(View(model));
                }
                model.productId = ProductID;
                LoadLov();

                var ProductDetails = _unitOfWork.product.GetProductById(ProductID);
                if (ProductDetails == null)
                {
                    TempData["ErrorMessage"] = "Failed to get the selected Product's detail!";
                    return await Task.FromResult<IActionResult>(View(model));
                }
                model.productCategory = ProductDetails.CatId;
                model.productTitle = ProductDetails.Title;
                model.productDescription = ProductDetails.Description;
                model.productPrice = ProductDetails.Price;
                model.productStatus = ProductDetails.Status;
                model.productImage  = ProductDetails.Image;

                return await Task.FromResult<IActionResult>(View(model));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return await Task.FromResult<IActionResult>(View());
            }
        }

        //<==================================================================Product Working ===========================================================>
    }
}