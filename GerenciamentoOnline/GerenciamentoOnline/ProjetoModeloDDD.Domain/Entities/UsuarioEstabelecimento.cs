using Castle.ActiveRecord;
using Newtonsoft.Json;
using ProjetoModeloDDD.Domain.Auxiliar;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoModeloDDD.Domain.Entities
{
    [System.ComponentModel.DefaultPropertyAttribute("Estabelecimento")]
    [Serializable]
    public class UsuarioEstabelecimento
    {
        [BelongsTo(Fetch = FetchEnum.Select)]
        [UIHint("DropDown")]
        [JsonConverter(typeof(ClasseModeloJsonConvert))]
        public Estabelecimento Estabelecimento { get; set; }
    }


}