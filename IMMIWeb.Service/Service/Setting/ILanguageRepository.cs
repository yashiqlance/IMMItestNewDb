﻿using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Setting
{
    public interface ILanguageRepository : IGenericRepository<IMMIWeb.Language>
    {
        IEnumerable<Language> CommonLanguageList();
    }
}
