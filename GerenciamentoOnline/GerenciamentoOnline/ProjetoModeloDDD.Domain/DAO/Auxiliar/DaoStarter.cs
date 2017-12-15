using Castle.ActiveRecord;
using NHibernate;
using System;
using System.Data;
using System.Reflection;

namespace ProjetoModeloDDD.Domain.DAO.Auxiliar
{
    public class DAOStarter
    {
        public static void Inicializa(Assembly[] arAs, string fileName)
        {
            Castle.ActiveRecord.Framework.Config.XmlConfigurationSource source = new Castle.ActiveRecord.Framework.Config.XmlConfigurationSource(fileName);
            Type[] ar = new Type[] { typeof(string) };

            ActiveRecordStarter.Initialize(arAs, source, ar);
        }

        // retorna 
        public static ISessionFactory CreateSessionFactory()
        {
            var holder = ActiveRecordMediator.GetSessionFactoryHolder();
            if (!ActiveRecordStarter.IsInitialized)
            {
                throw new Exception("Necessário inicialzar o ActiveRecord.");
            }

            return holder.GetSessionFactory(typeof(ActiveRecordBase));
        }

        public static System.Data.IDbConnection GetConexao()
        {
            return ((NHibernate.Impl.SessionFactoryImpl)ActiveRecordMediator.GetSessionFactoryHolder().
                    GetSessionFactory(typeof(ActiveRecordBase))).ConnectionProvider.GetConnection();
        }

        public static IDbDataParameter CreateCommandParameter(IDbCommand cmd, string name, object value)
        {
            IDbDataParameter parameter = cmd.CreateParameter();

            parameter.ParameterName = name;
            parameter.Value = value;

            return parameter;
        }
    }
}