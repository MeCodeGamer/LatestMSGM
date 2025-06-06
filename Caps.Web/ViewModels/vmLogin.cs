﻿using System.ComponentModel.DataAnnotations;

namespace MSGM.Web.ViewModels
{
    public class vmLogin
    {
        [Required]
        public string? UserName { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name ="Remember me?")]
        public bool RememberMe { get; set; } = false;
    }
}