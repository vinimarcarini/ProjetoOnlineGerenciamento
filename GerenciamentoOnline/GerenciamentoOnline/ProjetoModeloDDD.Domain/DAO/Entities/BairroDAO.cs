using Castle.ActiveRecord;
using NHibernate.Criterion;
using ProjetoModelo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoModeloDDD.Domain.DAO.Entities
{
    public class BairroDAO
    {
        public static Bairro[] FindAll(params AbstractCriterion[] abstractCriterion)
        {
            return ActiveRecordMediator<Bairro>.FindAll(abstractCriterion);
        }
    }
}
