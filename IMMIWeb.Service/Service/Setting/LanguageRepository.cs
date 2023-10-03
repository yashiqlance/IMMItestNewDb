using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Setting
{
    public class LanguageRepository : GenericRepository<IMMIWeb.Language>, ILanguageRepository
    {
        public LanguageRepository(DbA976eeImmitestContext context) : base(context)
        {

        }

        public IEnumerable<Language> CommonLanguageList()
        {
            return _dbContext.Languages.Where(x => x.IsActive==true).Select(a => new Language { Id = a.Id, Name = a.Name });
        }
    }
}
