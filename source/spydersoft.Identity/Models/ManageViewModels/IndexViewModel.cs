﻿using System.ComponentModel.DataAnnotations;

namespace spydersoft.Identity.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public bool IsPhoneConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }


        public string StatusMessage { get; set; }
    }
}