using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.ReviewRating
{
    public class ReviewRatingRepository : GenericRepository<IMMIWeb.RatingReviewConsultant>, IReviewRatingRepository
    {
        public ReviewRatingRepository(DbA976eeImmitestContext context) : base(context)
        {
        }
    }
}
