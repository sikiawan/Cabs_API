using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? RestaurantId { get; set; }
        public string? Name { get; set; }
        public string? LocalizedName { get; set; }
        public string? SurName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PasswordOld { get; set; }
        public string? House { get; set; }
        public string? DoorNum { get; set; }
        public string? Town { get; set; }
        public string? Street { get; set; }
        public string? Zipcode { get; set; }
        public string? Address { get; set; }
        public string? Comments { get; set; }
        public string? BillName { get; set; }
        public string? BillSurname { get; set; }
        public string? BillZipcode { get; set; }
        public string? BillTown { get; set; }
        public string? BillStreet { get; set; }
        public string? BillEmail { get; set; }
        public string? Addon { get; set; }
        public string? Addby { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdateBy { get; set; }
        public string? Status { get; set; }
        public string? BillPhone { get; set; }
        public string? BillDoorNum { get; set; }
        public string? CustomerComments { get; set; }
        public string? StoreCard { get; set; }
        public string? BrainTreeToken { get; set; }
        public string? RememberCard { get; set; }
        public bool? IsSocial { get; set; }
        public string? SocialUserId { get; set; }
        public string? PassResetRequest { get; set; }
        public int? PassResetCode { get; set; }
        public string? ApiToken { get; set; }
        public string? AccountType { get; set; }
    }
}
