using Castle.ActiveRecord;
using kardapio.Suprimentos;
using Newtonsoft.Json;
using ProjetoModeloDDD.Domain.Auxiliar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoModeloDDD.Domain.Entities
{
    [ActiveRecord(Cache = CacheEnum.ReadWrite)]
    [System.ComponentModel.DefaultPropertyAttribute("Descricao")]
    [Serializable]
    public class Regiao : ClasseModelo
    {
        [PrimaryKey(Generator = PrimaryKeyType.Native)]
        public override int? Id { get; set; }

        [Property(Length = 60)]
        public String Descricao { get; set; }

        [UIHint("Search")]
        [Property(Length = 10)]
        public string CEP { get; set; }

        [Property(Length = 100)]
        public string Logradouro { get; set; }

        [BelongsTo(Fetch = FetchEnum.Select)]
        [JsonConverter(typeof(ClasseModeloJsonConvert))]
        public Cidade Cidade { get; set; }


    }
}
