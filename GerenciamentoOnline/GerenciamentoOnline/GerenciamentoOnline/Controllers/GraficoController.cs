using ProjetoModeloDDD.Domain.DAO.Entities;
using ProjetoModeloDDD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Criterion;
using GerenciamentoOnline.Auxiliares;
using ProjetoModelo.Entities;
using kardapio.Suprimentos;

namespace GerenciamentoOnline.Controllers
{
    public class GraficoController : Controller
    {
        // GET: Grafico
        public ActionResult Index()
        {
            return View();
        }

        public static string GetDropDownLixeiras()
        {
            string cSelect = "<select class='lixeira'>";
            try
            {
                Lixeira[] lixeira = LixeiraDAO.FindAll(Expression.Eq("Ativo", true));

                foreach (Lixeira tipoAc in lixeira)
                {
                    cSelect += "<option value=\"" + tipoAc.Id + "\">" + tipoAc.Descricao + "</option>";
                }
            }
            catch (Exception e)
            {
                MetodosGlobais.SaveExceptionError(e, "Home/GetDropDownLixeiras");
            }
            cSelect += "</select>";

            return cSelect;
        }

        [AllowAnonymous]
        public MvcHtmlString GetDrowDownLixeira()
        {
            string chtml = GetDropDownLixeiras();

            return new MvcHtmlString(chtml);
        }

        public static string GetDropDownRegiaos()
        {
            string cSelect = "<select class='regiao'>";
            try
            {
                Regiao[] regiao = RegiaoDAO.FindAll();

                foreach (Regiao tipoAc in regiao)
                {
                    cSelect += "<option value=\"" + tipoAc.Id + "\">" + tipoAc.Descricao + "</option>";
                }
            }
            catch (Exception e)
            {
                MetodosGlobais.SaveExceptionError(e, "Home/GetDropDownRegiao");
            }
            cSelect += "</select>";

            return cSelect;
        }

        [AllowAnonymous]
        public MvcHtmlString GetDropDownRegiao()
        {
            string chtml = GetDropDownRegiaos();

            return new MvcHtmlString(chtml);
        }

        public static string GetDropDownBairros()
        {
            string cSelect = "<select class='bairro'>";
            try
            {
                Bairro[] bairro = BairroDAO.FindAll();

                foreach (Bairro tipoAc in bairro)
                {
                    cSelect += "<option value=\"" + tipoAc.Id + "\">" + tipoAc.Descricao + "</option>";
                }
            }
            catch (Exception e)
            {
                MetodosGlobais.SaveExceptionError(e, "Home/GetDropDownRegiao");
            }
            cSelect += "</select>";

            return cSelect;
        }

        [AllowAnonymous]
        public MvcHtmlString GetDropDownBairro()
        {
            string chtml = GetDropDownRegiaos();

            return new MvcHtmlString(chtml);
        }

        public static string GetDropDownCidades()
        {
            string cSelect = "<select class='cidade'>";
            try
            {
                Cidade[] cidade = CidadeDAO.FindAll();

                foreach (Cidade tipoAc in cidade)
                {
                    cSelect += "<option value=\"" + tipoAc.Id + "\">" + tipoAc.Nome + "</option>";
                }
            }
            catch (Exception e)
            {
                MetodosGlobais.SaveExceptionError(e, "Home/GetDropDownRegiao");
            }
            cSelect += "</select>";

            return cSelect;
        }

        [AllowAnonymous]
        public MvcHtmlString GetDropDownCidade()
        {
            string chtml = GetDropDownCidades();

            return new MvcHtmlString(chtml);
        }


    }
}