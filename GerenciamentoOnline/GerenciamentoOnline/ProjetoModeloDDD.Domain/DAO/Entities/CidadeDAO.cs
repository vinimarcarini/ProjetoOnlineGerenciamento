using Castle.ActiveRecord;
using kardapio.Suprimentos;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoModeloDDD.Domain.DAO.Entities
{
    public class CidadeDAO
    {
        public static Cidade[] FindAll(params AbstractCriterion[] abstractCriterion)
        {
            return ActiveRecordMediator<Cidade>.FindAll(abstractCriterion);
        }
    }
}
