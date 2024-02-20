using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models.RequestModels
{
    public class RequestGetUsers
    {
        public int tenant { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public string search { get; set; }
        //public string[] statusFilter { get; set; }
    }
}
