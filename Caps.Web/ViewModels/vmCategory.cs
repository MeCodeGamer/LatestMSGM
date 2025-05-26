using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSGM.Entity.Models;

namespace MSGM.Web.ViewModels
{
    public class vmCategory
    {
        [Required(ErrorMessage = "Category Title is Required!")]
        [StringLength(50, ErrorMessage = "Title cannot be longer than 50 characters.")]
        public string? Title { get; set; }

        [Required (ErrorMessage = "Category Description is Required!")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; }

        //Manage Category
        public int? CategoryId { get; set; }
        public string? CategoryTitle { get; set; }
        public string? CategoryDescription { get; set; }
        public bool CategoryStatus { get; set; }

        public List<Category>? categoryList { get; set; }
        public SelectList? ddcategoryList { get; set; }
    }
}