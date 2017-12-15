using Castle.ActiveRecord;
using Newtonsoft.Json;
using ProjetoModeloDDD.Domain.Auxiliar;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoModeloDDD.Domain.Entities
{
 
        [ActiveRecord(Cache = CacheEnum.ReadWrite)]
        [System.ComponentModel.DefaultPropertyAttribute("Classe")]
        [Serializable]
        public class AcessoPermissao : ClasseModelo
        {
            [PrimaryKey(Generator = PrimaryKeyType.Native)]
            public override int? Id { get; set; }

            //usado somente para mostrar no perfil de acesso a descrição do cadastro. andre 14/11/2013
            public string Descricao { get; set; }

            [BelongsTo(Fetch = FetchEnum.Select)]
            [UIHint("DropDown")]
            [JsonIgnore] // caso contrário dá erro
            public AcessoUsuario Acesso { get; set; }

            [Property]
            [JsonIgnore] // caso contrário dá erro
            private Type Classe { get; set; }

            public Type GetClasse()
            {
                return Classe;
            }

            public void SetClasse(Type classe)
            {
                Classe = classe;
            }

            public string NomeClasse { get; set; }

            [Property]
            public bool Consulta { get; set; }

            [Property]
            public bool Inclusao { get; set; }

            [Property]
            public bool Alteracao { get; set; }

            [Property]
            public bool Exclusao { get; set; }
        }
    
}
