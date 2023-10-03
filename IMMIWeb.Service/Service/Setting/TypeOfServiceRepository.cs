using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Setting
{
    public class TypeOfServiceRepository : GenericRepository<IMMIWeb.TypeOfService>, ITypeOfServiceRepository
    {

        public TypeOfServiceRepository(DbA976eeImmitestContext context) : base(context)
        {

        }

        public IEnumerable<TypeOfService> CommonTypeOfServiceList()
        {
            return _dbContext.TypeOfServices.Where(a => a.IsActive).Select(a => new TypeOfService { Id = a.Id, Name = a.Name ,CountryId=a.CountryId });
        }
    }
}
