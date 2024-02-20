using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Model.Models
{
    public class ModifiedEntity
    {
        public DateTimeOffset CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
    public class ActiveEntity : ModifiedEntity
    {
        public bool IsDeleted { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}

