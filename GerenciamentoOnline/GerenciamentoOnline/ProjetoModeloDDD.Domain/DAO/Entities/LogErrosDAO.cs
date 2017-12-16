using Castle.ActiveRecord;
using NHibernate.Criterion;
using ProjetoModeloDDD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoModeloDDD.Domain.DAO.Entities
{
    public class LogErrosDAO
    {
        public static LogErros FindFirst(DetachedCriteria critErro)
        {
            return ActiveRecordMediator<LogErros>.FindFirst(critErro);
        }

        public static LogErros[] SlicedFindAll(int firstResult, int maxResults, DetachedCriteria crit)
        {
            return ActiveRecordMediator<LogErros>.SlicedFindAll(firstResult, maxResults, crit);
        }

        public static void Save(LogErros log)
        {
            log.Save();
        }

        public static bool ExisteByParams(DetachedCriteria crit)
        {
            return ActiveRecordMediator<LogErros>.Exists(crit);
        }
    }
}
