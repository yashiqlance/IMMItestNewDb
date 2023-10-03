using IMMIWeb.Service.Models;
using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Favourite
{
    public interface IFavouriteRepository : IGenericRepository<IMMIWeb.FavouriteConsultant>
    {
        List<GetAvailableConsultantData> GetUserFavouriteConsultant(int UserId);
        int AddFavouriteConsultant(int ConsultantId,int UserId);

        int RemoveFavouriteConsultant(int ConsultantId, int UserId);
    }
}
