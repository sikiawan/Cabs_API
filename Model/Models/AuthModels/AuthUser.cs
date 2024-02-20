using System.ComponentModel.DataAnnotations;

namespace Model.Models.AuthModels
{
    public class AuthUser
    {
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        public string Password { get; set; }

    }
}
