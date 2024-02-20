using Model.Models;
using Model.Models.UserModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class RestaurantVM : ActiveEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public bool IsActive { get; set; }
        [StringLength(50)]
        [EmailAddress]
        public string? Email { get; set; }
        [StringLength(50, MinimumLength = 5)]
        public string? Password { get; set; }
        [StringLength(50, MinimumLength = 5)]
        public string? ConfirmPassword { get; set; }
    }
}
