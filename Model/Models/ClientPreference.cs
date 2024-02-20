using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class ClientPreference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TenantId { get; set; }
        public RestaurantModel Tenant { get; set; }
        public string? Name { get; set; }
        public string? Logo { get; set; }
        public string? Theme { get; set; }
        public string? Language { get; set; }
    }
}
