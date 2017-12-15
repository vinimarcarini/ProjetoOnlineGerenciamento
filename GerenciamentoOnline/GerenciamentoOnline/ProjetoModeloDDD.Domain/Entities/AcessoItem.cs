using Castle.ActiveRecord;
using ProjetoModeloDDD.Domain.Auxiliar;
using System;

namespace ProjetoModeloDDD.Domain.Entities
{
    [ActiveRecord(Cache = CacheEnum.ReadWrite)]
    [System.ComponentModel.DefaultPropertyAttribute("Descricao")]
    [Serializable]
    public class AcessoItem : ClasseModelo
    {
        [PrimaryKey(Generator = PrimaryKeyType.Native)]
        public override int? Id { get; set; }

        [Property]
        public CategPerfilAcesso Categoria { get; set; }

        [Property]
        public string Codigo { get; set; }

        [Property(Length = 80)]
        public string Descricao { get; set; }
    }

    public enum CategPerfilAcesso
    {
        Gerencial
    }

}