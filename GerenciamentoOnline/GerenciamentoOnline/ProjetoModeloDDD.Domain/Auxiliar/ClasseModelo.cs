using Castle.ActiveRecord;
using Newtonsoft.Json;
using NHibernate.Criterion;
using ProjetoModeloDDD.Domain.Entities;
using System;
using System.Reflection;

namespace ProjetoModeloDDD.Domain.Auxiliar
{
    //[ActiveRecord]
    [Serializable]
    public abstract class ClasseModelo : ActiveRecordBase<ClasseModelo>, IDisposable
    {
        public virtual int? Id { get; set; }

        [Property]
        public virtual bool Ativo { get; set; }


        [BelongsTo(Fetch = FetchEnum.Select)]
        [JsonProperty(Order = 1)]
        public virtual Empresa Empresa { get; set; }

        public ClasseModelo()
        {
            Ativo = true;
        }

        protected override void OnUpdate()
        {
            System.Reflection.PropertyInfo[] propriedades = GetType().GetProperties();

            MetodosAuxiliares.SeparaDataHora(this, propriedades);

            base.OnUpdate();
        }

        protected override bool BeforeSave(System.Collections.IDictionary state)
        {
            // elimina as classes que não contém chave salva.
            System.Reflection.PropertyInfo[] propriedades = this.GetType().GetProperties();

            MetodosAuxiliares.SeparaDataHora(this, propriedades);

            foreach (PropertyInfo prop in propriedades)
            {
                if (prop.PropertyType.IsClass && (prop.PropertyType.IsSubclassOf(typeof(ClasseModelo))))
                {
                    if (prop.GetValue(this, null) != null)
                    {
                        var objProp = prop.GetValue(this, null);
                        var value = objProp.GetType().GetProperty("Id").GetValue(objProp, null);
                        if (value == null || ((int)value == 0))
                            prop.SetValue(this, null, null);
                    }
                }
                else if (prop.PropertyType == typeof(decimal?))
                {
                    decimal? valor = (decimal?)prop.GetValue(this, null);

                    if (valor.HasValue)
                    {
                        prop.SetValue(this, Math.Round(valor.Value, 5), null);
                    }
                }
            }
            return base.BeforeSave(state);
        }

        public void PrepareForEdit()
        {
            // cria as classes para os objetos que são classes
            System.Reflection.PropertyInfo[] propriedades = this.GetType().GetProperties();
            foreach (PropertyInfo prop in propriedades)
            {
                try
                {
                    if (prop.PropertyType.IsSubclassOf(typeof(ClasseModelo)) ||
                        (prop.PropertyType.IsSubclassOf(typeof(ActiveRecordBase)) &&
                        prop.PropertyType.IsClass &&
                        prop.PropertyType.GetMember("Id").Length == 1))
                    {
                        if (prop.GetValue(this, null) == null)
                        {
                            ConstructorInfo[] c = prop.PropertyType.GetConstructors();

                            if (c.Length > 0)
                            {
                                prop.SetValue(this, c[0].Invoke(null), null);
                            }
                        }
                    }
                }
                catch
                {
                    //
                }
            }
        }

        public override string ToString()
        {
            // obtem a propriedade padrao
            object[] atributos = this.GetType().GetCustomAttributes(typeof(System.ComponentModel.DefaultPropertyAttribute), false);
            string propDesc = "Descricao";
            if (atributos.Length >= 1)
                propDesc = ((System.ComponentModel.DefaultPropertyAttribute)atributos[0]).Name;

            System.Reflection.PropertyInfo propNome = this.GetType().GetProperty(propDesc);
            if (propNome != null)
            {
                object obj = propNome.GetValue(this, null);
                string objText = obj != null ? obj.ToString() : "";
                return objText;
            }
            else
                return "";
        }

        public virtual bool SalvaEmOutroModelo(ref int idRetorno, Estabelecimento estabelecimento, ICustomPrincipal user)
        {
            return false;
        }

        public virtual void Inicializa(Estabelecimento estab)
        {
            this.Ativo = true;
        }

        public void Dispose()
        {

        }

        public virtual void Save(Empresa empresa)
        {
            this.Empresa = empresa;
            SaveInstance(this);
        }

        public virtual void SaveAndFlush(Empresa empresa)
        {
            this.Empresa = empresa;
            SaveAndFlush(this);
        }

        public virtual void Create(Empresa empresa)
        {
            this.Empresa = empresa;
            Create(this);
        }

        public virtual void CreateAndFlush(Empresa empresa)
        {
            this.Empresa = empresa;
            CreateAndFlush(this);
        }

        public virtual void Update(Empresa empresa)
        {
            this.Empresa = empresa;
            Update(this);
        }

        public virtual void UpdateAndFlush(Empresa empresa)
        {
            this.Empresa = empresa;
            UpdateAndFlush(this);
        }

        public virtual void Delete(Empresa empresa)
        {
            Delete(this);
        }

        public virtual void DeleteAndFlush(Empresa empresa)
        {
            DeleteAndFlush(this);
        }
    }
}