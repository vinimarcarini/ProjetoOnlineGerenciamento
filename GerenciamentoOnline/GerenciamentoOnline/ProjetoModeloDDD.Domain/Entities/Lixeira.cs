using Castle.ActiveRecord;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProjetoModeloDDD.Domain.Auxiliar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoModeloDDD.Domain.Entities
{
    [ActiveRecord(Cache = CacheEnum.ReadWrite, BatchSize = 20)]
    [System.ComponentModel.DefaultPropertyAttribute("Lixeira")]
    [Serializable]
    public class Lixeira : ClasseModelo
    {
        [PrimaryKey(Generator = PrimaryKeyType.Native)]
        public override int? Id { get; set; }

        [Property(Length = 80)] 
        public string Descricao { get; set; }

        [Property]
        public string DadoSensor1 { get; set; }

        [Property]
        public string DadoSensor2 { get; set; }

        [Property]
        public int TotalPorcentagem { get; set; }


        [Property]
        [UIHint("Enum")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LeituraValida leituraValida { get; set; }

        public enum LeituraValida
        {
            Aguardando,
            Lido         
        }

        public class ListaLixeira
        {
            public int Id { get; set; }
            public string Descricao { get; set; }
            public int TotalPorcentagem { get; set; }
        }

    }
}
