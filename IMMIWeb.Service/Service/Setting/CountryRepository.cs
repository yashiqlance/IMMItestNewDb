using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Setting
{
    public class CountryRepository : GenericRepository<IMMIWeb.Country>, ICountryRepository
    {
        public CountryRepository(DbA976eeImmitestContext context) : base(context)
        {

        }

        public IEnumerable<Country> CommonCountryList()
        {
            return _dbContext.Countries.Select(a => new Country { Id = a.Id,Name=a.Name});
        }


        public IEnumerable<Country> CommonCountryMobileCodeList()
        {
            return _dbContext.Countries.OrderBy(a => a.MobileCode).Select(a => new Country { Id = a.Id,  Name = a.MobileCode, Prefix = a.Prefix });
        }

        public IEnumerable<Country> TypeOfServiceCountryList()
        {
            var getCountrylst = _dbContext.TypeOfServices.Select(x => x.CountryId).Distinct().ToList();
            var getData = _dbContext.Countries.Where(x => getCountrylst.Contains(x.Id)).Select(a => new Country { Id = a.Id, Name = a.Name });
            return getData;
        }

        public string GetCountryNameById(int countryId)
        {
            var country = _dbContext.Countries.FirstOrDefault(x => x.Id == countryId);
            return country?.Name ?? string.Empty;
        }

        public decimal GetExchangeRateCountryWise(int CountryId)
        {
            return (decimal)_dbContext.Countries.Where(m => m.Id == CountryId).Select(b => b.ExchangeRate ?? 0).FirstOrDefault();
        }

    }
}
