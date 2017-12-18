using GerenciamentoOnline.Auxiliares;
using GerenciamentoOnline.Helper;
using GerenciamentoOnline.Models;
using Newtonsoft.Json;
using NHibernate.Criterion;
using ProjetoModeloDDD.Domain.DAO.Auxiliar;
using ProjetoModeloDDD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using static ProjetoModeloDDD.Domain.Entities.Lixeira;

namespace GerenciamentoOnline.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        /// <summary>
        /// Busca todos os cartões cadastrados.
        /// </summary>
        /// <returns>Json String</returns>
        public List<ListaLixeira> BuscaListaDados()
        {

            string strMensagem = "Não foi possível buscar os dados cadastrados.";
            List<ListaLixeira> lsCartoes = new List<ListaLixeira>();
            CustomPrincipal user = User as CustomPrincipal;
            try
            {

                // Busca todos os cartões cadastrados.
                DetachedCriteria criteria = DetachedCriteria.For<Lixeira>();
                //criteria.AddOrder(Order.Asc("Descricao"));
                criteria.Add(Expression.Eq("leituraValida", LeituraValida.Aguardando));

                ClasseModeloDAO<Lixeira> dao = ClasseModeloDAO<Lixeira>.Create(1);

                Lixeira[] cartoes = dao.FindAll();

                // Adiciona cada cartão a lista.
                foreach (Lixeira cartao in cartoes)
                {
                    ListaLixeira lc = new ListaLixeira()
                    {
                        Id = Convert.ToInt32(cartao.Id),
                        Descricao = cartao.Descricao,
                        TotalPorcentagem = cartao.TotalPorcentagem
                    };
                    if (lsCartoes.Any(x => x.Descricao == lc.Descricao) == true)
                    {
                        if (lsCartoes.Any(x => x.TotalPorcentagem != lc.TotalPorcentagem) == true)
                        {
                            foreach (var itemCarros in lsCartoes)
                            {
                                if (itemCarros != lc)
                                {
                                    lsCartoes.Remove(itemCarros);
                                    lsCartoes.Add(lc);
                                }
                                else
                                {
                                    lsCartoes.Add(lc);
                                }

                            }
                        }
                    }
                    else
                    {
                        lsCartoes.Add(lc);
                    }

                }


                strMensagem = "OK";
            }
            catch (Exception e)
            {
                MetodosGlobais.SaveExceptionError(e);

                strMensagem = "Erro: " + e.Message;
            }
            //, Lixieras = lsCartoes


            return lsCartoes;

        } // BuscaCartoesCadastrados

        public JsonResult GetPeopleData()
        {
            System.Threading.Thread.Sleep(1000);
            return Json(BuscaListaDados(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPeopleSearch(int offset, int limit, string search, string sort, string order)
        {
            var people = BuscaListaDados().AsQueryable()
                .WhereIf(!string.IsNullOrEmpty(search), o =>
                    o.Descricao.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(sort ?? "Descricao", order)
                .ToList();

            var model = new
            {
                total = people.Count(),
                rows = people.Skip((offset / limit) * limit).Take(limit),
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}