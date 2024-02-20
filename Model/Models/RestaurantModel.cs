using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities;

namespace Model.Models
{
    public class RestaurantModel : ActiveEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public bool IsActive { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<ClientPreference> ClientPreference { get; set; }

    }
}
