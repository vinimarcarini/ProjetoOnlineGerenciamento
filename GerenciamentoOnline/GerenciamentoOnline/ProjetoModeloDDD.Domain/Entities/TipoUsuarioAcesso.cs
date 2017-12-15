using Castle.ActiveRecord;
using kardapio.Suprimentos;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoModeloDDD.Domain.Entities
{
    [System.ComponentModel.DefaultPropertyAttribute("TipoAcesso")]
    [Serializable]
    public class UsuarioTipoAcesso
    {
        [Property]
        [UIHint("Enum")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TipoAcesso TipoAcesso { get; set; }
    }

}