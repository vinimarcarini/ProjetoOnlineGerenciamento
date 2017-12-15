using Castle.ActiveRecord;
using NHibernate.Criterion;
using ProjetoModeloDDD.Domain.Entities;

namespace ProjetoModeloDDD.Domain.DAO.Entities
{
    public class EmpresaDAO
    {
        public static int Count(params AbstractCriterion[] abstractCriterion)
        {
            return ActiveRecordMediator<Empresa>.Count(abstractCriterion);
        }

        public static int Count(DetachedCriteria criteria)
        {
            return ActiveRecordMediator<Empresa>.Count(criteria);
        }

        public static Empresa FindFirst(params AbstractCriterion[] abstractCriterion)
        {
            return ActiveRecordMediator<Empresa>.FindFirst(abstractCriterion);
        }

        public static void CreateAndFlush(Empresa empresa)
        {
            empresa.CreateAndFlush();
        }

        public static void Save(Empresa empresa)
        {
            empresa.Save();
        }

        public static Empresa BuscaEmpresaPorUrl(string urlEmpresa)
        {
            DetachedCriteria crit = DetachedCriteria.For<Empresa>();
            crit.Add(Expression.Eq("UrlAcesso", urlEmpresa));
            crit.Add(Expression.Eq("Ativo", true));
            return ActiveRecordMediator<Empresa>.FindFirst(crit);
        }

        public static Empresa FindByPrimaryKey(int idEmpresa, bool throwOnNotFound = true)
        {
            return ActiveRecordMediator<Empresa>.FindByPrimaryKey(idEmpresa, throwOnNotFound);
        }

        public static Empresa[] FindAll(DetachedCriteria criteria)
        {
            return ActiveRecordMediator<Empresa>.FindAll(criteria);
        }
    }
}