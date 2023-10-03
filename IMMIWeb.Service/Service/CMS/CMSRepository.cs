using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.CMS
{
    public class CMSRepository : GenericRepository<IMMIWeb.Cm>, ICMSRepository
    {
        public CMSRepository(DbA976eeImmitestContext context) : base(context)
        {
        }
    }
}
