using Castle.ActiveRecord;
using ProjetoModeloDDD.Domain.Auxiliar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetoModeloDDD.Domain.Entities
{
    [ActiveRecord(Cache = CacheEnum.ReadWrite)]
    [System.ComponentModel.DefaultPropertyAttribute("Descricao")]
    [Serializable]
    public class AcessoUsuario : ClasseModelo
    {
        [PrimaryKey(Generator = PrimaryKeyType.Native)]
        public override int? Id { get; set; }

        [Property(Length = 60)]
        public string Descricao { get; set; }

        
        [UIHint("AcessoPermissaoEditor"), SaveCollectionDefault] // NewtonSoft.Json.JsonIgnore
        public IList<AcessoPermissao> Permissoes { get; set; }

        [UIHint("AcessoPermissaoOutrosEditor"), SaveCollectionDefault] // NewtonSoft.Json.JsonIgnore
        public IList<AcessoItemUsuario> OutrasPermissoes { get; set; }

        
        public override void Inicializa(Estabelecimento estab)
        {
            base.Inicializa(estab);
        }

        private void SalvaPermissoes(Empresa empresa)
        {
            if (Permissoes != null)
                foreach (var permissao in Permissoes)
                {
                    permissao.Acesso = this;
                    if (permissao.Id == null)
                    {
                        permissao.Create(empresa);
                    }
                    else
                    {
                        permissao.Update(empresa);
                    }
                }
        }

        private void SalvaOutrasPermissoes(Empresa empresa)
        {
            if (OutrasPermissoes != null)
                foreach (var outrasPermissao in OutrasPermissoes)
                {
                    outrasPermissao.AcessoUsuario = this;
                    if (outrasPermissao.Id == null)
                    {
                        outrasPermissao.Create(empresa);
                    }
                    else
                    {
                        outrasPermissao.Update(empresa);
                    }
                }
        }
        public override void Create(Empresa empresa)
        {
            base.Create(empresa);
            SalvaPermissoes(empresa);
            SalvaOutrasPermissoes(empresa);
        }

        public override void CreateAndFlush(Empresa empresa)
        {
            base.CreateAndFlush(empresa);
            SalvaPermissoes(empresa);
            SalvaOutrasPermissoes(empresa);
        }

        public override void SaveAndFlush(Empresa empresa)
        {
            base.SaveAndFlush(empresa);
            SalvaPermissoes(empresa);
            SalvaOutrasPermissoes(empresa);
        }

        public override void Save(Empresa empresa)
        {
            base.Save(empresa);
            SalvaPermissoes(empresa);
            SalvaOutrasPermissoes(empresa);
        }

        public override void Update(Empresa empresa)
        {
            base.Update(empresa);
            SalvaPermissoes(empresa);
            SalvaOutrasPermissoes(empresa);
        }

        public override void UpdateAndFlush(Empresa empresa)
        {
            base.UpdateAndFlush(empresa);
            SalvaPermissoes(empresa);
            SalvaOutrasPermissoes(empresa);
        }

        public override bool SalvaEmOutroModelo(ref int idRetorno, Estabelecimento estabelecimento, ICustomPrincipal user)
        {
            if (this.Id.HasValue)
            {
                Update(estabelecimento.Empresa);
            }
            else
            {
                Create(estabelecimento.Empresa);
            }

            return true;
        }

    }
}