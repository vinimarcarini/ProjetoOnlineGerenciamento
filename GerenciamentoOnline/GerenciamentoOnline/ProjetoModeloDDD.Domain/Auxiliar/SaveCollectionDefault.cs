using System;

namespace ProjetoModeloDDD.Domain.Auxiliar
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false), Serializable]
    public sealed class SaveCollectionDefaultAttribute : Attribute
    {
        public SaveCollectionDefaultAttribute()
        {

        }
    }
}