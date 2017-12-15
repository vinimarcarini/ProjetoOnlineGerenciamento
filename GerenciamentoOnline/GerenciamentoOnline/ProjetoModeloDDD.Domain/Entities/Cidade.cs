using System;
using Castle.ActiveRecord;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProjetoModeloDDD.Domain.Auxiliar;
using ProjetoModeloDDD.Domain.Entities;

namespace kardapio.Suprimentos
{

    [ActiveRecord(Cache = CacheEnum.ReadWrite, BatchSize = 20)]
    [System.ComponentModel.DefaultPropertyAttribute("Nome")]
    [Serializable]
    public class Cidade : ClasseModelo
    {
        [PrimaryKey(Generator = PrimaryKeyType.Native)]
        public override int? Id { get; set; }

        [UIHint("Search")]
        public string Pesquisa { get; set; }

        // Nome.
        [Property(Length = 80)]
        public String Nome { get; set; }

        // Código da cidade do IBGE.
        [Property(Length = 10)]
        public string CodIBGE { get; set; }

        // UF.
        [UIHint("DropDownEstado")]
        [BelongsTo(Fetch = FetchEnum.Select)]
        [JsonConverter(typeof(ClasseModeloJsonConvert))]
        public UFCidade UFCidade { get; set; }

        // Fuso Horário.
        [Property]
        [UIHint("Enum")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FusoHorario FusoHorario { get; set; }

    }

    public class CidadeRetornoApi
    {
        public string Descricao { get; set; }
        public int CodigoIBGE  { get; set; }
        public EstadoRetornoApi Estado { get; set; }

    }

    public class EstadoRetornoApi
    {
        public string Descricao { get; set; }
        public string Sigla { get; set; }
        public int CodigoIBGE { get; set; }
    }

    /// <summary>
    /// Define os fuso horários disponíveis no Brasil.
    /// </summary>
    public enum FusoHorario
    {
        UTC2,       // UTC-2 (Atol das Rocas, Fernando de Noronha, São Pedro e São Paulo, Trindade e Martim Vaz.
        UTC3,       // UTC-3 (Horário de Brasília - DF, Sul, Sudeste, Nordeste, GO, TO, PA, AP).
        UTC4,       // UTC-4 (MT, MS, RO, RR e dois terços do Amazonas).
        UTC5        // UTC-5 (Acre e 13 municípios do Amazonas).
    }
}
