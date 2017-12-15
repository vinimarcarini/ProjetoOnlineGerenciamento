using System;

using Castle.ActiveRecord;
using ProjetoModeloDDD.Domain.Auxiliar;

namespace ProjetoModelo.Entities
{
    [ActiveRecord]
    [System.ComponentModel.DefaultPropertyAttribute("Descricao")]
    [Serializable]
    public class Bairro : ClasseModelo
    {
        [PrimaryKey(Generator = PrimaryKeyType.Native)]
        public override int? Id { get; set; }

        [Property(Length = 80)]
        public string Descricao { get; set; }
    }
}
