using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model.Models.UserModels
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "Please enter user name.")]
        [StringLength(50)]
        public string Username { get; set; }
        public string LocalizedName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [StringLength(50, MinimumLength = 5, ErrorMessage = "Password is required of minimum length 5.")]
        public string Password { get; set; }

        //[Required]
        //[DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
        [Required(ErrorMessage = "Tenant is required.")]
        public string TenantId { get; set; }

    }

}
