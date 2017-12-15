using System;
using Castle.ActiveRecord;
using ProjetoModeloDDD.Domain.Auxiliar;

namespace kardapio.Suprimentos
{

    [ActiveRecord(Cache = CacheEnum.ReadWrite, BatchSize = 20)]
    [System.ComponentModel.DefaultPropertyAttribute("Descricao")]
    //[Menu(Descricao = "País", MenuSistema = GrupoMenuSistema.Cadastros.Locais, OrdemMenu = 50, PlanoWeb = NivelPlanoWeb.Promocional)]
    [Serializable]
    public class Pais : ClasseModelo
    {
        [PrimaryKey(Generator = PrimaryKeyType.Native)]
        public override int? Id { get; set; }

        // Descrição.
        [Property(Length = 80)]
        public String Descricao { get; set; }

        // Código do IBGE.
        [Property(Length = 10)]
        public int CodIBGE { get; set; }
    } // Pais

}
