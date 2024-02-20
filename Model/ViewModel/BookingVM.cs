using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class BookingVM
    {
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
