using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class VerificationHandlerViewModel
    {
        [Key]
        public int Id { get; set; }
        

        [StringLength(50)]
        public string UniqueId { get; set; } = null!;

        [StringLength(50)]
        public string Platform { get; set; } = null!;

        [StringLength(50)]
        public string PlatformDetail { get; set; } = null!;

        public int Attempt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }

        public bool IsActive { get; set; }
    }
}
