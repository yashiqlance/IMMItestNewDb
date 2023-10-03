using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Setting
{
    public class AppLanguageRepository : GenericRepository<IMMIWeb.AppLanguage>, IAppLanguageRepository
    {
        public AppLanguageRepository(DbA976eeImmitestContext context) : base(context)
        {

        }

        public IEnumerable<AppLanguage> CommonAppLanguageList()
        {
            return _dbContext.AppLanguages.Select(a => new AppLanguage { Id = a.Id, Name = a.Name });
        }
    }
}
