using Castle.ActiveRecord;
using kardapio.Suprimentos;
using Newtonsoft.Json;
using ProjetoModeloDDD.Domain.Auxiliar;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoModeloDDD.Domain.Entities
{

    [ActiveRecord(Cache = CacheEnum.ReadWrite, BatchSize = 20)]
    [System.ComponentModel.DefaultPropertyAttribute("Descricao")]
    [Serializable]
    public class UFCidade : ClasseModelo
    {
        [PrimaryKey(Generator = PrimaryKeyType.Native)]
        public override int? Id { get; set; }

        // Descrição da UF.
        [Property(Length = 80)]
        public String Descricao { get; set; }

        // Sigla da UF.
        [Property(Length = 2)]
        public String Sigla { get; set; }

        // Código do IBGE.
        [Property(Length = 2)]
        public String CodIBGE { get; set; }

        // País.
        [UIHint("DropDown")]
        [BelongsTo(Fetch = FetchEnum.Select)]
        [JsonConverter(typeof(ClasseModeloJsonConvert))]
        public Pais Pais { get; set; }

    }
}