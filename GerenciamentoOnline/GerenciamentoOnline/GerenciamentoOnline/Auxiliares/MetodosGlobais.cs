using GerenciamentoOnline.Auxiliares.Singleton;
using GerenciamentoOnline.Models;
using ProjetoModeloDDD.Domain.DAO.Auxiliar;
using ProjetoModeloDDD.Domain.DAO.Entities;
using ProjetoModeloDDD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GerenciamentoOnline.Auxiliares
{
    public static class MetodosGlobais
    {
        private static object instance;

        public static void SaveExceptionError(Exception e = null, string url = "", CustomPrincipal usr = null, bool mostrarMensagem = false,
            string mensagem = "", string tipoAlerta = "warning", Estabelecimento estab = null)
        {
            LogErros log = null;

            try
            {

                if (estab == null && usr != null)
                {
                    
                }

                log = new LogErros()
                {
                    Data = DateTimeNow(estab),
                    HoraTxt = DateTimeNow(estab).ToString("HH:mm:ss"),
                    Fantasia = estab != null ? estab.Fantasia : "",
                    RazaoSocial = estab != null ? estab.RazaoSocial : "",
                    TipoAlerta = tipoAlerta,
                    Estabelecimento = estab,
                    Url = url
                };
                if (e != null)
                {
                    log.Exception = e.Message.Length > 60 ? e.Message.Substring(0, 60) : e.Message;
                    log.Html = e.StackTrace.ToString();
                }

                if (mostrarMensagem)
                {
                    string msg = e != null ? e.ToString() + " --- " + e.StackTrace : "";
                    string novaMsg = msg.Length > 60 ? msg.Substring(0, 60) : msg;

                    log.Exception = novaMsg;
                    log.Status = 1;
                    log.Html = msg;
                    log.Ativo = true;
                }

                if (!string.IsNullOrEmpty(mensagem))
                    log.Html = mensagem;

                LogErrosDAO.Save(log);
            }
            catch
            {
                //não gravar excessão do log para não ter perigo de entrar em loop
            }
            //catch
            //    SaveExceptionErrosConexaoManual(log)
        }

       

        public static CustomPrincipal CustomPrincipalLogado(Estabelecimento estab)
        {
            CustomClasseModelo userClasseModelo = ProjetoModeloDDD.Domain.Auxiliar.MetodosAuxiliares.CustomPrincipalLogado(estab);
            CustomPrincipal user = new CustomPrincipal("")
            {
                IdEstab = userClasseModelo.IdEstab,
                IdEmpresa = userClasseModelo.IdEmpresa
            };
            return user;
        }
        public static DateTime DateTimeNow(Estabelecimento estab)
        {
            return ProjetoModeloDDD.Domain.Auxiliar.MetodosAuxiliares.DateTimeNow(estab);
        }


    }
}