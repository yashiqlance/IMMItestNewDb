using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserSettingAndCmsViewModel
    {
        public IEnumerable<UserSettingViewModel>? settingViewModels { get; set; }
        public IEnumerable<CMSViewModel>? cMSViewModels { get; set; }
    }
}
