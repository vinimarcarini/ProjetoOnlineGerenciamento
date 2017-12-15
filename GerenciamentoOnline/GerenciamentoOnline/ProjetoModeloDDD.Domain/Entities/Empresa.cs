using Castle.ActiveRecord;
using System;

namespace ProjetoModeloDDD.Domain.Entities
{
    [ActiveRecord(Cache = CacheEnum.ReadWrite)]
    [System.ComponentModel.DefaultPropertyAttribute("Nome")]
    [Serializable]
    public class Empresa : ActiveRecordBase<Empresa>
    {
        [PrimaryKey(PrimaryKeyType.Native)]
        public int Id { get; set; }

        [Property]
        public bool Ativo { get; set; }

        [Property(Length = 60)]
        public String Nome { get; set; }


        public void Create()
        {
            Create(this);
        }

        public void CreateAndFlush()
        {
            CreateAndFlush(this);
        }

        public void Save()
        {
            SaveInstance(this);
        }

        public void SaveAndFlush()
        {
            SaveAndFlush(this);
        }

        public void Update()
        {
            Update(this);
        }

        public void UpdateAndFlush()
        {
            UpdateAndFlush(this);
        }


    }
}