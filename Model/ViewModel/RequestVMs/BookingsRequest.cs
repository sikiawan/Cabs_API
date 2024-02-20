using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel.RequestVMs
{
    public class BookingsRequest : SortingPaginationRequest
    {
        public string? NameSearch { get; set; }
    }
}
