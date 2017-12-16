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
    public class RegiaoDAO
    {
        public static Regiao[] FindAll(params AbstractCriterion[] abstractCriterion)
        {
            return ActiveRecordMediator<Regiao>.FindAll(abstractCriterion);
        }
    }
}
