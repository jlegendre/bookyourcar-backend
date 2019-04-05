using System.ComponentModel.DataAnnotations;

namespace TestAuthentification.ViewModels
{
    public class PoleViewModel
    {
        [Key]
        public int PoleId { get; set; }
        [Required]
        [Display(Name = "Nom")]
        public string PoleName { get; set; }
        [Required]
        [Display(Name = "Ville")]
        public string PoleCity { get; set; }
        [Display(Name = "Adresse")]
        public string PoleAddress { get; set; }
        [Required]
        [StringLength(5, ErrorMessage = "Le {0} doit comporter {2} caractères.", MinimumLength = 5)]
        [Display(Name = "Code Postale")]
        public string PoleCp { get; set; }

    }
}
