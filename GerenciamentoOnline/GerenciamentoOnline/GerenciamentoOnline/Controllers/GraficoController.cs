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
using static ProjetoModeloDDD.Domain.Entities.Lixeira;
using ProjetoModeloDDD.Domain.DAO.Auxiliar;

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
            string cSelect = "<select class='lixeiras'>";
            try
            {
                Lixeira[] lixeira = LixeiraDAO.FindAll();

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

        //public static string GetDropDownRegiaos()
        //{
        //    string cSelect = "";
        //    string cSelect1 = "<select class='regiao'>";
        //    string cSelect2 = "<option value=\"";
        //    string cSelect3 = "\">";
        //    string cSelect4 = "</option>";
        //    string cSelect5 = "</select>";
        //    int cSelect6 = 0;
        //    string cSelect7 = "";
        //    string cSelect9 = "";
        //    try
        //    {
        //        Regiao[] regiao = RegiaoDAO.FindAll();

        //        foreach (Regiao tipoAc in regiao)
        //        {
        //            cSelect6 = tipoAc.Id.Value;
        //            cSelect7 = tipoAc.Descricao;

        //            if (cSelect7 == cSelect9)
        //            {
        //                cSelect9 = "";
        //            }
        //            else
        //            {
        //                cSelect9 = cSelect7;
        //                cSelect = cSelect1 + cSelect2 + cSelect6 + cSelect3 + cSelect7 + cSelect4 + cSelect5;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MetodosGlobais.SaveExceptionError(e, "Home/GetDropDownRegiao");
        //    }

        //    return cSelect;
        //}

        //[AllowAnonymous]
        //public MvcHtmlString GetDropDownRegiao()
        //{
        //    string chtml = GetDropDownRegiaos();

        //    return new MvcHtmlString(chtml);
        //}

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
            string chtml = GetDropDownBairros();

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