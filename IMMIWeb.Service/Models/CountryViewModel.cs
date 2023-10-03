using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class CountryViewModel
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? MobileCode { get; set; }
    }

    public partial class ImmiCountry
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;      

    }
}
