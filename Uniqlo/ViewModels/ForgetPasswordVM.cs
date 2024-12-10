﻿using System.ComponentModel.DataAnnotations;

namespace Uniqlo.ViewModels
{
    public class ForgetPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
