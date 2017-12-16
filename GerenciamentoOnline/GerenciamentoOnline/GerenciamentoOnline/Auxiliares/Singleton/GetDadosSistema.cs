using GerenciamentoOnline.Models;
using kardapio.Suprimentos;
using NHibernate.Criterion;
using ProjetoModeloDDD.Domain.DAO.Auxiliar;
using ProjetoModeloDDD.Domain.DAO.Entities;
using ProjetoModeloDDD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace GerenciamentoOnline.Auxiliares.Singleton
{
    public class GetDadosSistema
    {
        static GetDadosSistema instance = null;
        static readonly object padlock = new object();
        Dictionary<int, DadosSistema> ListConfig = new Dictionary<int, DadosSistema>();
        Dictionary<int, Estabelecimento> ListEstabelecimentos = new Dictionary<int, Estabelecimento>();
        Dictionary<int, List<Usuario>> dictUsuarios = new Dictionary<int, List<Usuario>>();

        private GetDadosSistema()
        {

        }

        public class DadosSistema
        {
            public int idEstabelecimento { get; set; }
        }

        public void AtualizaObjetosSingleton(int idEstab = 0)
        {
            CarregaConfig(idEstab);
            AtualizaEstabelecimentos(idEstab);
        }
        private void ReloadEstabelecimento(Estabelecimento estab)
        {
            if (estab != null)
            {
                int idEstab = estab.Id.Value;
                if (ListEstabelecimentos.ContainsKey(idEstab))
                {
                    ListEstabelecimentos.Remove(idEstab);
                }
                ListEstabelecimentos.Add(idEstab, estab);
            }
        }
        private void AtualizaEstabelecimentos(int idEstab)
        {
            try
            {
                if (idEstab == 0)
                {
                    Estabelecimento[] estabs = EstabelecimentoDAO.BuscaTodosOsEstabelecimentosAtivos();
                    foreach (var item in estabs)
                    {
                        ReloadEstabelecimento(item);
                    }
                }
                else
                {
                    Estabelecimento estab = EstabelecimentoDAO.FindByPrimaryKey(idEstab);
                    ReloadEstabelecimento(estab);
                }
            }
            catch (Exception e)
            {
                MetodosGlobais.SaveExceptionError(e, "GetDadosSistema/AtualizaEstabelecimentos");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void AdicionaDadosDictConfig(Estabelecimento estab)
        {
            DadosSistema Dados = new DadosSistema();
            Dados.idEstabelecimento = estab.Id.Value;
            CustomPrincipal user = MetodosGlobais.CustomPrincipalLogado(estab);

            try
            {
                    ListConfig.Add(estab.Id.Value, Dados);
            }
            catch (Exception e)
            {
                MetodosGlobais.SaveExceptionError(e, "GetDadosSistema/AdicionaDadosDictConfig");
            }
        }

        private void CarregaConfig(int idEstab)
        {
            try
            {
                if (idEstab == 0)
                {
                    ListConfig.Clear();

                    Estabelecimento[] estabs = EstabelecimentoDAO.BuscaTodosOsEstabelecimentosAtivos();

                    foreach (var estab in estabs)
                    {
                        AdicionaDadosDictConfig(estab);
                    }
                }
                else
                {
                    Estabelecimento estab = EstabelecimentoDAO.FindByPrimaryKey(idEstab);
                    if (estab != null)
                    {
                        ResetConfiguracoes(estab);
                        AdicionaDadosDictConfig(estab);
                    }
                }
            
            }
            catch (Exception e)
            {
                MetodosGlobais.SaveExceptionError(e, "GetDadosSistema/CarregaConfig");
            }
        }

        public void ResetConfiguracoes(Estabelecimento estab)
        {
            if (estab == null)
            {
                ListConfig.Clear();
            }
            else
            {
                if (ListConfig.ContainsKey(estab.Id.Value))
                {
                    ListConfig.Remove(estab.Id.Value);
                }
            }
        }

        public static GetDadosSistema GetInstance
        {
            get
            {

                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new GetDadosSistema();
                            instance.AtualizaObjetosSingleton();
                        }
                    }
                }

                return instance;
            }
        }
    }
}