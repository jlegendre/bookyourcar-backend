using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.ViewModels
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Le mot de passe ne peut pas être vide.")]
        [DisplayName("Password")]
        [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
        [MinLength(6, ErrorMessage = "The password cannot be less than 6 characters")]
        public string Password
        {
            get;
            set;
        }

        [DataType(DataType.Password)]
        [DisplayName("Password confirmation")]
        [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
        [Compare("Password", ErrorMessage = "Les mots de passes ne correspondent pas.")]
        [MinLength(6, ErrorMessage = "The password cannot be less than 6 characters")]
        public string PasswordConfirmation
        {
            get;
            set;
        }
    }
}
