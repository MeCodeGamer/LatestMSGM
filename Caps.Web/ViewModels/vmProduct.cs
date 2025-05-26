using System.ComponentModel.DataAnnotations;

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
        public string Image { get; set; } = string.Empty;

        [Display(Name = "Available")]
        public bool Status { get; set; }
    }
}
