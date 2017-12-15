using Castle.ActiveRecord;
using Newtonsoft.Json;
using ProjetoModeloDDD.Domain.Auxiliar;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoModeloDDD.Domain.Entities
{
    [ActiveRecord(Cache = CacheEnum.ReadWrite)]
    [System.ComponentModel.DefaultPropertyAttribute("AcessoUsuario")]
    [Serializable]
    public class AcessoItemUsuario : ClasseModelo
    {
        [PrimaryKey(Generator = PrimaryKeyType.Native)]
        public override int? Id { get; set; }

        [BelongsTo(Fetch = FetchEnum.Select)]
        [JsonIgnore] // caso contrário dá erro
        [UIHint("DropDown")]
        public AcessoUsuario AcessoUsuario { get; set; }

        [BelongsTo(Fetch = FetchEnum.Select)]
        [JsonIgnore] // caso contrário dá erro
        [UIHint("DropDown")]
        public AcessoItem AcessoItem { get; set; }

        [Property]
        public bool Permite { get; set; }

        public int IdItem
        {
            get { return AcessoItem != null && AcessoItem.Id.HasValue ? AcessoItem.Id.Value : 0; }
            set
            {
                if (value > 0)
                    AcessoItem = ActiveRecordMediator<AcessoItem>.FindByPrimaryKey(value, false);
                else
                    AcessoItem = null;
            }
        }

    }
}