using System;
using System.Collections.Generic;
using Castle.ActiveRecord;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using NHibernate.Criterion;
using ProjetoModeloDDD.Domain.Auxiliar;
using ProjetoModeloDDD.Domain.Entities;
using ProjetoModeloDDD.Domain.DAO.Entities;

namespace kardapio.Suprimentos
{
    [ActiveRecord(Cache = CacheEnum.ReadWrite, BatchSize = 20)]
    [System.ComponentModel.DefaultPropertyAttribute("Login")]
    [Serializable]
    public class Usuario : ClasseModelo
    {
        [PrimaryKey(Generator = PrimaryKeyType.Native)]
        public override int? Id { get; set; }

        [Property(Length = 60)]
        public string Login { get; set; }

        [Property(Length = 60)]
        [UIHint("PasswordEditor")]
        public string Senha { get; set; }

        //utiliza no paf
        [Property(Length = 60)]
        public string Temp { get; set; }

        [Property(Column = "Acesso")]
        [JsonProperty(Order = 60)]
        public int CodAcesso { get; set; }

        private AcessoUsuario acesso;

        [UIHint("DropDown")]
        [JsonConverter(typeof(ClasseModeloJsonConvert))]
        [JsonProperty(Order = 50)]
        public AcessoUsuario Acesso
        {
            get
            {
                if (acesso == null && this.Id.HasValue)
                {
                    if (CodAcesso > 0)
                    {
                        acesso = AcessoUsuarioDAO.FindByPrimaryKey(CodAcesso);
                    }
                }
                if(acesso != null && acesso.Id.HasValue)
                {
                    CodAcesso = acesso.Id.Value;
                }
                return acesso;
            }
            set
            {
                acesso = value;
                CodAcesso = (value != null && value.Id.HasValue) ? value.Id.Value : 0;
            }
        }
        
        public TipoAcesso TipoAcesso { get; set; }

        [HasMany(DependentObjects = true, MapType = typeof(UsuarioEstabelecimento), Table = "Estabelecimentosusuario", ColumnKey = "Usuario", Fetch = FetchEnum.Select, BatchSize = 20)]
        public IList<UsuarioEstabelecimento> EstabelecimentosUsuario { get; set; }

        [UIHint("UsuarioTipoAcessoEditor"), SaveCollectionDefault]
        [HasMany(DependentObjects = true, MapType = typeof(UsuarioTipoAcesso), Table = "Tiposacessousuario", ColumnKey = "Usuario", Fetch = FetchEnum.Select, BatchSize = 20)]
        public IList<UsuarioTipoAcesso> TipoAcessoUsuario { get; set; }

        [Property(Length = 60)]
        [UIHint("PasswordEditor")]
        public string CodigoUsuario { get; set; }

        [Property(Length = 250)]
        public string Email { get; set; }

        public override void Inicializa(Estabelecimento estab)
        {
            base.Inicializa(estab);
            EstabelecimentosUsuario = new List<UsuarioEstabelecimento>();
            TipoAcessoUsuario = new List<UsuarioTipoAcesso>();
        }

        public override void Update(Empresa empresa)
        {
            IList<TipoAcesso> newList = new List<TipoAcesso>();

            foreach (var item in TipoAcessoUsuario)
            {
                if (!newList.Contains(item.TipoAcesso))
                {
                    newList.Add(item.TipoAcesso);
                }
            }

            IList<UsuarioTipoAcesso> newListAcesso = new List<UsuarioTipoAcesso>();

            foreach (var item in newList)
            {
                UsuarioTipoAcesso n = new UsuarioTipoAcesso();
                n.TipoAcesso = item;
                newListAcesso.Add(n);
            }

            TipoAcessoUsuario = newListAcesso;

            base.Update(empresa);
        }
    }

    public enum TipoAcesso
    {
        Gerencial,
        Comanda,
        Painel,
        ControleSaida
    }

}
