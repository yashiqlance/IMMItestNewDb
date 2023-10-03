using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserCardDetails
    {
        public string CardName { get; set; } = null!;

        public string CardNumber { get; set; } = null!;
        public string ExpMonth { get; set; }

        public string ExpYear { get; set; }

        public int Cvv { get; set; }
        public int? Id { get; set; }

        public bool IsPrimary { get; set; }

        public int ConsultantId { get; set; } = 0!;

        public List<UserCardsDetail> CardList { get; set; }
    }
    public class Removecarddetailparam
    {
        public int id { get; set; }

        public int Cardid { get; set; }

    }
}
