using Castle.ActiveRecord;
using NHibernate.Criterion;
using ProjetoModeloDDD.Domain.Entities;
using System.Linq;

namespace ProjetoModeloDDD.Domain.DAO.Entities
{
    public class EstabelecimentoDAO
    {
        public static Estabelecimento[] FindAll(DetachedCriteria crit)
        {
            return ActiveRecordMediator<Estabelecimento>.FindAll(crit);
        }

        public static Estabelecimento[] FindAll(params AbstractCriterion[] abstractCriterion)
        {
            return ActiveRecordMediator<Estabelecimento>.FindAll(abstractCriterion);
        }

        public static Estabelecimento BuscaEstabPorUrl(string urlEmpresa)
        {
            return ActiveRecordMediator<Estabelecimento>.FindFirst(Expression.Eq("UrlAcesso", urlEmpresa));
        }

        public static Estabelecimento FindFirst(DetachedCriteria crit)
        {
            return ActiveRecordMediator<Estabelecimento>.FindFirst(crit);
        }

        public static Estabelecimento FindByPrimaryKey(int idEstab)
        {
            return ActiveRecordMediator<Estabelecimento>.FindByPrimaryKey(idEstab, false);
        }

        public static Estabelecimento[] BuscaTodosOsEstabelecimentosAtivos(bool ativo = true)
        {
            return ActiveRecordMediator<Estabelecimento>.FindAll(Expression.Eq("Ativo", ativo));
        }

        public static Estabelecimento FindByCNPJ(string cnpj)
        {
            DetachedCriteria crit = DetachedCriteria.For<Estabelecimento>();
            crit.Add(Expression.Eq("CNPJ", cnpj));

            return ActiveRecordMediator<Estabelecimento>.FindFirst(crit);
        }

        public static Estabelecimento FindFirst(params SimpleExpression[] criteria)
        {
            return ActiveRecordMediator<Estabelecimento>.FindFirst(criteria);
        }

        public static int Count(params SimpleExpression[] criteria)
        {
            return ActiveRecordMediator<Estabelecimento>.Count(criteria);
        }

        public static Estabelecimento[] BuscaTodosOsEstabXEmpresaAtivos(Empresa empresa)
        {
            return ActiveRecordMediator<Estabelecimento>.FindAll(Expression.Eq("Ativo", true), Expression.Eq("Empresa", empresa));
        }

        public static Estabelecimento[] SlicedFindAllCriteria(int qtde, int recordsCount, DetachedCriteria crit)
        {
            return ActiveRecordMediator<Estabelecimento>.SlicedFindAll(qtde, recordsCount, crit);
        }
    }
}