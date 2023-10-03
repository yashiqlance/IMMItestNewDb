using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserDocuments
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string Extensions { get; set; }
        public byte Size { get; set; }

    }

    public class UserAddDocument
    {
        public string File { get; set; } = null!;
        public string Filename { get; set; } = null!;
        public string Extensions { get; set; } = null!;
    }



    public class RemoveUserDocumentparam
    {
        public int UserId { get; set; }

        public string Filename { get; set; }

        public string Extensions { get; set; }

    }
}
