using System.ComponentModel.DataAnnotations;
using MSGM.Entity.Models;

namespace MSGM.Web.ViewModels
{
    public class vmProduct
    {
        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CatId { get; set; }

        [Required(ErrorMessage = "Product title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        [Display(Name = "Product Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "Product Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Product image is required.")]
        [Display(Name = "Image URL")]
        public IFormFile Image { get; set; }

        [Display(Name = "Available")]
        public bool Status { get; set; }

        //

        public int productId { get; set; }
        public int productCategory { get; set; }
        public string? productTitle { get; set; }
        public string? productDescription { get; set; }
        public double productPrice { get; set; }
        public string? productImage { get; set; }
        public bool productStatus { get; set; }



        public List<Product>? productlist { get; set; }
        public Category? category{ get; set; }
    }
}
