using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Identity;
using Model.Models;

namespace Model.Entity
{

    public class User : ApplicationUser
    {
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? PhoneNo { get; set; }
    }
}
