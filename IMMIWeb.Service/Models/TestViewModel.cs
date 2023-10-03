using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class TestViewModel
    {
        public bool status { get; set; }

        public string Description { get; set; } = null!;

    }
}
