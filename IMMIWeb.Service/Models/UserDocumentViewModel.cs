using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserDocumentViewModel
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string Size { get; set; }
        public string FileExtension { get; set; }
        public int RetainId { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public string FileUrl { get; set; }
    }
}
