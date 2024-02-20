using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? WhatsAppWithCC { get; set; }
        public string? VehicleType { get; set; }
        public string? PickUpLocation { get; set; }
        public string? Destination { get; set; }
        public string? Status { get; set; }
        public DateTime? BookingDate { get; set; }
    }
}
