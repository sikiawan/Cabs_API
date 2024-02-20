using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class ApplicationUser : IdentityUser
    {
        [DefaultValue("true")]
        public bool IsActive { get; set; }
        public DateTime AddedOn { get; set; }
        public int TenantId { get; set; }
        public string LocalizedName { get; set; }
        public RestaurantModel Tenant { get; set; }

    }
}
