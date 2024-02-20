using Microsoft.AspNetCore.Http;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class ClientPreferenceVM
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string? TenantName { get; set; }
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
        public string? Logo { get; set; }
        public string? Theme { get; set; }
        public string? Language { get; set; }

    }
}
