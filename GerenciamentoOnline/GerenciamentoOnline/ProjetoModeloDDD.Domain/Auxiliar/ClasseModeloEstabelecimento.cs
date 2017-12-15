using Castle.ActiveRecord;
using Newtonsoft.Json;
using ProjetoModeloDDD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjetoModeloDDD.Domain.Auxiliar
{
    [Serializable]
    public class ClasseModeloEstabelecimento : ClasseModelo, IEstabelecimento
    {
        [BelongsTo(Fetch = FetchEnum.Select)]
        [JsonConverter(typeof(ClasseModeloJsonConvert))]
        public Estabelecimento Estabelecimento { get; set; }

        //o new é utilizado quando quer esconder o metodo da classe herdada, por que nos dois metodos possui o mesmo tipo, "int"
        //public new virtual void Save(int codEstabelecimento)
        //{
        //    Estabelecimento estab = Estabelecimento.FindByPrimaryKey(codEstabelecimento) as Estabelecimento;
        //    this.Estabelecimento = estab;
        //    this.Empresa = estab.Empresa;
        //    SaveInstance(this);
        //}

        //aqui não utiliza new por que trocou o tipo de parametro
        public virtual void Save(Estabelecimento estab)
        {
            this.Estabelecimento = estab;
            this.Empresa = estab.Empresa;
            SaveInstance(this);
        }

        //essa metodo é somente para ocultar o metodo da classe pai, se alguem tentar utilizar vai dar erro
        [Obsolete("Para salvar classe que tem herança de ClasseModeloEstabelecimento, favor passar por parametro o Estabelecimento.", true)]
        public override void Save(Empresa empresa)
        {
            GravaLogObsolete(empresa);
        }


        public virtual void SaveAndFlush(Estabelecimento estab)
        {
            this.Estabelecimento = estab;
            this.Empresa = estab.Empresa;
            SaveAndFlush(this);
        }

        [Obsolete("Para salvar classe que tem herança de ClasseModeloEstabelecimento, favor passar por parametro o Estabelecimento.", true)]
        public override void SaveAndFlush(Empresa empresa)
        {
            GravaLogObsolete(empresa);
        }

        //aqui não utiliza new por que trocou o tipo de parametro
        public virtual void Create(Estabelecimento estab)
        {
            this.Estabelecimento = estab;
            this.Empresa = estab.Empresa;
            Create(this);
        }

        //essa metodo é somente para ocultar o metodo da classe pai, se alguem tentar utilizar vai dar erro
        [Obsolete("Para salvar classe que tem herança de ClasseModeloEstabelecimento, favor passar por parametro o Estabelecimento.", true)]
#pragma warning disable CS0809 // O membro obsoleto substitui o membro não obsoleto
        public override void Create(Empresa empresa)
#pragma warning restore CS0809 // O membro obsoleto substitui o membro não obsoleto
        {
            GravaLogObsolete(empresa);
        }

        public virtual void CreateAndFlush(Estabelecimento estab)
        {
            this.Estabelecimento = estab;
            this.Empresa = estab.Empresa;
            CreateAndFlush(this);
        }

        //essa metodo é somente para ocultar o metodo da classe pai, se alguem tentar utilizar vai dar erro
        [Obsolete("Para salvar classe que tem herança de ClasseModeloEstabelecimento, favor passar por parametro o Estabelecimento.", true)]
        public override void CreateAndFlush(Empresa empresa)
        {
            GravaLogObsolete(empresa);
        }

        public virtual void Delete(Estabelecimento estab)
        {
            this.Estabelecimento = estab;
            Delete(this);
        }

        //essa metodo é somente para ocultar o metodo da classe pai, se alguem tentar utilizar vai dar erro
        [Obsolete("Para salvar classe que tem herança de ClasseModeloEstabelecimento, favor passar por parametro o Estabelecimento.", true)]
        public override void Delete(Empresa empresa)
        {
            GravaLogObsolete(empresa);
        }

        public virtual void DeleteAndFlush(Estabelecimento estab)
        {
            this.Estabelecimento = estab;
            DeleteAndFlush(this);
        }

        //essa metodo é somente para ocultar o metodo da classe pai, se alguem tentar utilizar vai dar erro
        [Obsolete("Para salvar classe que tem herança de ClasseModeloEstabelecimento, favor passar por parametro o Estabelecimento.", true)]
        public override void DeleteAndFlush(Empresa empresa)
        {
            GravaLogObsolete(empresa);
        }

        //aqui não utiliza new por que trocou o tipo de parametro
        public virtual void Update(Estabelecimento estab)
        {
            this.Estabelecimento = estab;
            this.Empresa = estab.Empresa;
            Update(this);
        }

        //essa metodo é somente para ocultar o metodo da classe pai, se alguem tentar utilizar vai dar erro
        [Obsolete("Para salvar classe que tem herança de ClasseModeloEstabelecimento, favor passar por parametro o Estabelecimento.", true)]
        public override void Update(Empresa empresa)
        {
            GravaLogObsolete(empresa);
        }

        //aqui não utiliza new por que trocou o tipo de parametro
        public virtual void UpdateAndFlush(Estabelecimento estab)
        {
            this.Estabelecimento = estab;
            this.Empresa = estab.Empresa;
            UpdateAndFlush(this);
        }

        //essa metodo é somente para ocultar o metodo da classe pai, se alguem tentar utilizar vai dar erro
        [Obsolete("Para salvar classe que tem herança de ClasseModeloEstabelecimento, favor passar por parametro o Estabelecimento.", true)]
        public override void UpdateAndFlush(Empresa empresa)
        {
            GravaLogObsolete(empresa);
        }

        private void GravaLogObsolete(Empresa empresa)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            string caminho = "";
            foreach (var item in stackFrames)
            {
                caminho += item.GetMethod().Name + " -> ";
            }
            LogErros.GravaLog(null, "ClasseModeloEstabelecimento/GravaLogObsolete", 0, false, caminho);

            throw new Exception("O campo estabelecimento não foi alimentado");
        }
    }
}
