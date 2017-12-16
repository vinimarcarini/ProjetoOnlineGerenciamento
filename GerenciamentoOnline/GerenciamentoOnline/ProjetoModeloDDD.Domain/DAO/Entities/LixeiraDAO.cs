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
    public class LixeiraDAO
    {

        public static Lixeira[] FindAll(params AbstractCriterion[] abstractCriterion)
        {
            return ActiveRecordMediator<Lixeira>.FindAll(abstractCriterion);
        }
    }
}
