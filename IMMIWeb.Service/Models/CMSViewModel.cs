using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class CMSViewModel
    {
        public int Id { get; set; }

        public string Description { get; set; } = null!;

        public int Module { get; set; }

        public bool IsActive { get; set; }

        public int UserRole { get; set; }

        public IEnumerable<Module> lstModules { get; set; }
    }
}
