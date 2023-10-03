using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Setting
{
    public interface ICountryRepository : IGenericRepository<IMMIWeb.Country>
    {
        IEnumerable<Country> TypeOfServiceCountryList();
        IEnumerable<Country> CommonCountryList();
        IEnumerable<Country> CommonCountryMobileCodeList();

        string GetCountryNameById(int countryId);
        decimal GetExchangeRateCountryWise(int CountryId);
    }
}
