using Castle.ActiveRecord;
using Newtonsoft.Json;
using ProjetoModeloDDD.Domain.Auxiliar;
using System;

namespace ProjetoModeloDDD.Domain.Entities
{
    [ActiveRecord]
    [Serializable]
    public class LogErros : ActiveRecordBase<LogErros>
    {

        [PrimaryKey(PrimaryKeyType.Native)]
        public int Id { get; set; }

        [Property]
        public bool Ativo { get; set; }

        [Property(Length = 5000)]
        public String Url { get; set; }

        [Property(Length = 10)]
        public int? Status { get; set; }

        [Property(Length = 60)]
        public String Exception { get; set; }

        [Property(Length = 5000)]
        public String Html { get; set; }

        [Property(Length = 80)]
        public String RazaoSocial { get; set; }

        [Property(Length = 80)]
        public String Fantasia { get; set; }

        [Property(Length = 60)]
        public String Usuario { get; set; }

        [Property(Length = 60)]
        public String TipoAcesso { get; set; }


        private DateTime? data;

        [Property]
        [JsonConverter(typeof(NullDateJsonConvert))]
        public DateTime? Data
        {
            get { return data; }
            set
            {
                HoraTxt = MetodosAuxiliares.HoraFromDateTime(value, HoraTxt);
                data = MetodosAuxiliares.GetDateFromData(value);
            }
        }

        [Property(Length = 8)]
        public string HoraTxt { get; set; }

        [Property]
        public int IdPedRemoteDevice { get; set; }

        [Property(Length = 10)]
        public string TipoAlerta { get; set; }

        [Property]
        public string CodSessaoUsuario { get; set; }

        [BelongsTo(Fetch = FetchEnum.Select)]
        public Estabelecimento Estabelecimento { get; set; }

        internal void Save()
        {
            SaveInstance(this);
        }

        public void Update()
        {
            Update(this);
        }

        public void UpdateAndFlush()
        {
            UpdateAndFlush(this);
        }

        /// <summary>
        /// Grava um erro no log.
        /// </summary>
        /// <param name="e">Exceção.</param>
        /// <param name="strMensagem">Mensagem de erro.</param>
        /// <param name="strLocal">Local onde ocorreu o erro (Namespace + método).</param>
        public static void GravaLog(Exception e = null, string url = "", int idEstabelecimento = 0, bool mostrarMensagem = false, string mensagem = "", string tipoAlerta = "warning")
        {

            try
            {
                Estabelecimento estab = idEstabelecimento > 0 ? ActiveRecordMediator<Estabelecimento>.FindByPrimaryKey(idEstabelecimento) : new Estabelecimento();

                LogErros log = new LogErros
                {
                    Data = MetodosAuxiliares.DateTimeNow(estab),
                    HoraTxt = MetodosAuxiliares.DateTimeNow(estab).ToString("HH:mm:ss"),
                    Fantasia = estab.Fantasia,
                    RazaoSocial = estab.RazaoSocial,
                    TipoAlerta = tipoAlerta
                };
                //log.Usuario = funcionario != null ? funcionario.Nome : usuario != null ? usuario.Login : "";
                if (idEstabelecimento > 0)
                {
                    log.Estabelecimento = estab;
                }
                log.Url = url;
                //log.TipoAcesso = usuario != null ? usuario.TipoAcesso.ToString() : "";

                if (e != null)
                {
                    log.Exception = e.Message.Length > 60 ? e.Message.Substring(0, 60) : e.Message;
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

                if (mensagem != "")
                    log.Html = mensagem;

                log.Save();
            }
            catch { }

        }

        internal static void GravaLog(Exception e, object methosBase)
        {
            throw new NotImplementedException();
        }
    }
}