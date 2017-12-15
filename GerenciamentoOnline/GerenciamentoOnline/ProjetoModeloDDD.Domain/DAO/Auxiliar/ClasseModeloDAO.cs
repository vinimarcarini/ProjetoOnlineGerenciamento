using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Internal;
using NHibernate;
using NHibernate.Criterion;
using ProjetoModeloDDD.Domain.Auxiliar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjetoModeloDDD.Domain.DAO.Auxiliar
{
    public class ClasseModeloDAO<T> : RecordMediator, IDisposable
    {
        private int Empresa = 0;

        private int Estabelecimento = 0;

        private SimpleExpression FiltroEstab = null;
        private SimpleExpression FiltroEmpresa = null;
        private static Type TypeClasseModelEstab = typeof(ClasseModeloEstabelecimento);

        public static ClasseModeloDAO<T> Create(int codEmpresa)
        {
            var inst = new ClasseModeloDAO<T>() { Empresa = codEmpresa, Estabelecimento = 0 };

            inst.FiltroEmpresa = Expression.Eq("Empresa.Id", inst.Empresa);

            return inst;
        }

        private ClasseModeloDAO()
        {

        }

        public static ClasseModeloDAO<T> Create(ICustomPrincipal User)
        {
            if (User.IdEmpresa == 0)
                throw new Exception("Estabelecimento sem empresa. " + Environment.StackTrace);

            ClasseModeloDAO<T> inst = null;

            //if (typeof(T) is IEstabelecimento)
            if (typeof(T).IsSubclassOf(TypeClasseModelEstab))
            {
                inst = new ClasseModeloDAO<T>() { Empresa = User.IdEmpresa, Estabelecimento = User.IdEstab };
                inst.FiltroEstab = Expression.Eq("Estabelecimento.Id", inst.Estabelecimento);
            }
            else
            {
                inst = new ClasseModeloDAO<T>() { Empresa = User.IdEmpresa };
            }

            inst.FiltroEmpresa = Expression.Eq("Empresa.Id", inst.Empresa);

            return inst;
        }

        public static ClasseModeloDAO<T> Create(CustomClasseModelo User)
        {
            if (User.IdEmpresa == 0)
                throw new Exception("Estabelecimento sem empresa. " + Environment.StackTrace);

            var inst = new ClasseModeloDAO<T>() { Empresa = User.IdEmpresa, Estabelecimento = User.IdEstab };

            inst.FiltroEmpresa = Expression.Eq("Empresa.Id", inst.Empresa);


            if (typeof(T) is IEstabelecimento)
            {
                inst.FiltroEstab = Expression.Eq("Estabelecimento.Id", inst.Estabelecimento);

            }

            return inst;
        }

        /// <summary>
        /// Invokes the specified delegate passing a valid 
        /// NHibernate session. Used for custom NHibernate queries.
        /// </summary>
        /// <param name="call">The delegate instance</param>
        /// <param name="instance">The ActiveRecord instance</param>
        /// <returns>Whatever is returned by the delegate invocation</returns>
        public object Execute(NHibernateDelegate call, T instance)
        {
            var dados = DetachedCriteria.For<ClasseModelo>();

            var targetType = instance.GetType();

            if (targetType == null) throw new ArgumentNullException("targetType", "Target type must be informed");
            if (call == null) throw new ArgumentNullException("call", "Delegate must be passed");

            EnsureInitialized(targetType);

            var holder = ActiveRecordMediator.GetSessionFactoryHolder();

            ISession session = holder.CreateSession(targetType);

            try
            {
                return call(session, instance);
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                throw new ActiveRecordException("Error performing Execute for " + targetType.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        internal new void EnsureInitialized(Type targetType)
        {
            var holder = ActiveRecordMediator.GetSessionFactoryHolder();
            if (holder == null)
            {
                String message = String.Format("An ActiveRecord class ({0}) was used but the framework seems not " +
                                               "properly initialized. Did you forget about ActiveRecordStarter.Initialize() ?",
                                               targetType.FullName);
                throw new ActiveRecordException(message);
            }
            if (targetType != typeof(ActiveRecordBase) && ActiveRecordModel.GetModel(targetType) == null)
            {
                String message = String.Format("You have accessed an ActiveRecord class that wasn't properly initialized. " +
                                               "There are two possible explanations: that the call to ActiveRecordStarter.Initialize() didn't include {0} class, or that {0} class is not decorated with the [ActiveRecord] attribute.",
                                               targetType.FullName);
                throw new ActiveRecordException(message);
            }
        }


        /// <summary>
        /// Finds an object instance by its primary key.
        /// </summary>
        /// <param name="id">ID value</param>
        /// <param name="throwOnNotFound"><c>true</c> if you want an exception to be thrown
        /// if the object is not found</param>
        /// <exception cref="NHibernate.ObjectNotFoundException">if <c>throwOnNotFound</c> is set to 
        /// <c>true</c> and the row is not found</exception>
        public T FindByPrimaryKey(object id, bool throwOnNotFound)
        {
            //return (T)FindByPrimaryKey(typeof(T), id, throwOnNotFound);
            DetachedCriteria crit = DetachedCriteria.For<T>();
            FiltraEmpresaEstab(crit);
            crit.Add(Expression.Eq("Id", id));

            return (T)FindFirst(typeof(T), crit);
        }

        /// <summary>
        /// Finds an object instance by its primary key.
        /// </summary>
        /// <param name="id">ID value</param>
        public T FindByPrimaryKey(object id)
        {
            DetachedCriteria crit = DetachedCriteria.For<T>();
            FiltraEmpresaEstab(crit);
            crit.Add(Expression.Eq("Id", id));

            //return (T)FindByPrimaryKey(typeof(T), id, true);
            return (T)FindFirst(typeof(T), crit);

        }

        /// <summary>
        /// Searches and returns the first row.
        /// </summary>
        /// <param name="orders">The sort order - used to determine which record is the first one</param>
        /// <param name="criterias">The criteria expression</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public T FindFirst(Order[] orders, params ICriterion[] criterias)
        {
            criterias = FiltraEmpresaEstab(criterias);

            return (T)FindFirst(typeof(T), orders, criterias);
        }

        /// <summary>
        /// Efetua o filtro de empresa e estabelecimento sobre a seleção
        /// </summary>
        /// <param name="criterias">Lista de critérios para o filtro</param>
        /// <returns></returns>
        private ICriterion[] FiltraEmpresaEstab(ICriterion[] criterias)
        {
            Array.Resize(ref criterias, criterias.Length + 1);
            criterias[(criterias.Length - 1)] = FiltroEmpresa;

            if (FiltroEstab != null)
            {
                Array.Resize(ref criterias, criterias.Length + 1);
                criterias[(criterias.Length - 1)] = FiltroEstab;
            }

            return criterias;
        }

        /// <summary>
        /// Efetua o filtro de empresa e estabelecimento sobre a seleção
        /// </summary>
        /// <param name="criterias">Lista de critérios para o filtro</param>
        /// <returns></returns>
        private void FiltraEmpresaEstab(DetachedCriteria detachedCriteria)
        {
            detachedCriteria.Add(FiltroEmpresa);

            if (FiltroEstab != null)
            {
                detachedCriteria.Add(FiltroEstab);
            }
        }

        /// <summary>
        /// Searches and returns the first row.
        /// </summary>
        /// <param name="criterias">The criteria expression</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public T FindFirst(params ICriterion[] criterias)
        {
            criterias = FiltraEmpresaEstab(criterias);

            return (T)FindFirst(typeof(T), criterias);
        }

        /// <summary>
        /// Searches and returns the first row.
        /// </summary>
        /// <param name="detachedCriteria">The criteria.</param>
        /// <param name="orders">The sort order - used to determine which record is the first one.</param>
        /// <returns>A <c>targetType</c> instance or <c>null.</c></returns>
        public T FindFirst(DetachedCriteria detachedCriteria, params Order[] orders)
        {
            FiltraEmpresaEstab(detachedCriteria);
            return (T)FindFirst(typeof(T), detachedCriteria, orders);
        }

        /// <summary>
        /// Searches and returns the first row.
        /// </summary>
        /// <param name="criteria">The criteria expression</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public T FindFirst(DetachedCriteria criteria)
        {
            FiltraEmpresaEstab(criteria);

            return (T)FindFirst(typeof(T), criteria);
        }

        /// <summary>
        /// Searches and returns the first row.
        /// </summary>
        /// <param name="criterias">The criterias.</param>
        /// <returns>A instance the targetType or <c>null</c></returns>
        public T FindOne(params ICriterion[] criterias)
        {
            criterias = FiltraEmpresaEstab(criterias);
            return (T)FindOne(typeof(T), criterias);
        }

        /// <summary>
        /// Searches and returns a row. If more than one is found, 
        /// throws <see cref="ActiveRecordException"/>
        /// </summary>
        /// <param name="criteria">The criteria</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public T FindOne(DetachedCriteria criteria)
        {
            FiltraEmpresaEstab(criteria);
            return (T)FindOne(typeof(T), criteria);
        }


        /// <summary>
        /// Finds records based on a property value - automatically converts null values to IS NULL style queries. 
        /// </summary>
        /// <param name="property">A property name (not a column name)</param>
        /// <param name="value">The value to be equals to</param>
        /// <returns></returns>
        public T[] FindAllByProperty(String property, object value)
        {
            DetachedCriteria crit = DetachedCriteria.For<T>();
            FiltraEmpresaEstab(crit);
            crit.Add(Expression.Eq(property, value));
            return ActiveRecordBase<T>.FindAll(crit);
        }

        /// <summary>
        /// Finds records based on a property value - automatically converts null values to IS NULL style queries. 
        /// </summary>
        /// <param name="orderByColumn">The column name to be ordered ASC</param>
        /// <param name="property">A property name (not a column name)</param>
        /// <param name="value">The value to be equals to</param>
        /// <returns></returns>
        public T[] FindAllByProperty(String orderByColumn, String property, object value)
        {
            DetachedCriteria crit = DetachedCriteria.For<T>();
            FiltraEmpresaEstab(crit);
            crit.Add(Expression.Eq(property, value));
            crit.AddOrder(Order.Asc(orderByColumn));
            return ActiveRecordBase<T>.FindAllByProperty(orderByColumn, property, value);
        }

        /// <summary>
        /// Returns all instances found for the specified type.
        /// </summary>
        /// <returns></returns>
        public T[] FindAll()
        {
            DetachedCriteria crit = DetachedCriteria.For<T>();
            FiltraEmpresaEstab(crit);
            return (T[])FindAll(typeof(T), crit);
        }

        /// <summary>
        /// Returns all instances found for the specified type 
        /// using sort orders and criterias.
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="criterias"></param>
        /// <returns></returns>
        public T[] FindAll(Order[] orders, params ICriterion[] criterias)
        {
            criterias = FiltraEmpresaEstab(criterias);
            return (T[])FindAll(typeof(T), orders, criterias);
        }

        /// <summary>
        /// Returns all instances found for the specified type 
        /// using criterias.
        /// </summary>
        /// <param name="criterias"></param>
        /// <returns></returns>
        public T[] FindAll(params ICriterion[] criterias)
        {
            criterias = FiltraEmpresaEstab(criterias);
            return (T[])FindAll(typeof(T), criterias);
        }

        /// <summary>
        /// Returns all instances found for the specified type according to the criteria
        /// </summary>
        public T[] FindAll(DetachedCriteria detachedCriteria, params Order[] orders)
        {
            FiltraEmpresaEstab(detachedCriteria);
            return (T[])FindAll(typeof(T), detachedCriteria, orders);
        }

        /// <summary>
        /// Returns a portion of the query results (sliced)
        /// </summary>
        public T[] SlicedFindAll(int firstResult, int maxResults, Order[] orders, params ICriterion[] criterias)
        {
            criterias = FiltraEmpresaEstab(criterias);
            return (T[])SlicedFindAll(typeof(T), firstResult, maxResults, orders, criterias);
        }

        /// <summary>
        /// Returns a portion of the query results (sliced)
        /// </summary>
        public T[] SlicedFindAll(int firstResult, int maxResults, params ICriterion[] criterias)
        {
            criterias = FiltraEmpresaEstab(criterias);
            return (T[])SlicedFindAll(typeof(T), firstResult, maxResults, null, criterias);
        }

        /// <summary>
        /// Returns a portion of the query results (sliced)
        /// </summary>
        public T[] SlicedFindAll(int firstResult, int maxResults, DetachedCriteria criteria, params Order[] orders)
        {
            FiltraEmpresaEstab(criteria);
            return (T[])SlicedFindAll(typeof(T), firstResult, maxResults, orders, criteria);
        }

        public System.Collections.IList FindListByGetExecutableCriteria(DetachedCriteria criteria)
        {
            FiltraEmpresaEstab(criteria);

            return criteria.GetExecutableCriteria(MetodosAuxiliares.GetActiveRecordSession()).List();
        }

        /// <summary>
        /// Deletes all entities of <typeparamref name="T"/>.
        /// </summary>
        public void DeleteAll()
        {
            DetachedCriteria crit = DetachedCriteria.For<T>();
            FiltraEmpresaEstab(crit);
            throw new NotImplementedException("Ainda não implementado.");
            // DeleteAll(typeof(T));
        }

        /// <summary>
        /// Deletes all entities of <typeparamref name="T"/> that match the HQL where clause.
        /// </summary>
        public void DeleteAll(string where)
        {
            DetachedCriteria crit = DetachedCriteria.For<T>();
            FiltraEmpresaEstab(crit);
            throw new NotImplementedException("Ainda não implementado.");
            // DeleteAll(typeof(T), where);
        }

        /// <summary>
        /// Saves the instance to the database
        /// </summary>
        /// <param name="instance"></param>
        public void Save(T instance)
        {
            ActiveRecordMediator.Save(instance);
        }

        /// <summary>
        /// Saves a copy of the instance to the database
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>The saved instance</returns>
        public T SaveCopy(T instance)
        {
            return (T)ActiveRecordMediator.SaveCopy(instance);
        }

        /// <summary>
        /// Creates (Saves) a new instance to the database.
        /// </summary>
        /// <param name="instance"></param>
        public void Create(T instance)
        {
            ActiveRecordMediator.Create(instance);
        }

        /// <summary>
        /// Persists the modification on the instance
        /// state to the database.
        /// </summary>
        /// <param name="instance"></param>
        public void Update(T instance)
        {
            ActiveRecordMediator.Update(instance);
        }

        /// <summary>
        /// Deletes the instance from the database.
        /// </summary>
        /// <param name="instance"></param>
        public void Delete(T instance)
        {
            ActiveRecordMediator.Delete(instance);
        }

        /// <summary>
        /// Refresh the instance from the database.
        /// </summary>
        /// <param name="instance">The ActiveRecord instance to be reloaded</param>
        public void Refresh(T instance)
        {
            ActiveRecordMediator.Refresh(instance);
        }

        /// <summary>
        /// Check if the <paramref name="id"/> exists in the database.
        /// </summary>
        /// <typeparam name="PkType">The <c>System.Type</c> of the PrimaryKey</typeparam>
        /// <param name="id">The id to check on</param>
        /// <returns><c>true</c> if the ID exists; otherwise <c>false</c>.</returns>
        public bool Exists<PkType>(PkType id)
        {
            return ActiveRecordBase<T>.Exists(id);
        }

        /// <summary>
        /// Check if any instance matches the query.
        /// </summary>
        /// <param name="detachedQuery">The query expression</param>
        /// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
        public bool Exists(DetachedCriteria detachedCriteria)
        {
            FiltraEmpresaEstab(detachedCriteria);
            return Exists(typeof(T), detachedCriteria);
        }

        /// <summary>
        /// Returns the number of records of the specified 
        /// type in the database
        /// </summary>
        /// <returns>The count result</returns>
        public int Count()
        {
            DetachedCriteria crit = DetachedCriteria.For<T>();
            FiltraEmpresaEstab(crit);
            return ActiveRecordMediator.Count(typeof(T), crit);
        }

        /// <summary>
        /// Returns the number of records of the specified 
        /// type in the database that match the given critera
        /// </summary>
        /// <param name="criteria">The criteria expression</param>
        /// <returns>The count result</returns>
        public int Count(params ICriterion[] criteria)
        {
            criteria = FiltraEmpresaEstab(criteria);
            return ActiveRecordMediator.Count(typeof(T), criteria);
        }

        /// <summary>
        /// Returns the number of records of the specified 
        /// type in the database
        /// </summary>
        /// <param name="detachedCriteria">The criteria expression</param>
        /// <returns>The count result</returns>
        public int Count(DetachedCriteria detachedCriteria)
        {
            FiltraEmpresaEstab(detachedCriteria);
            return ActiveRecordMediator.Count(typeof(T), detachedCriteria);
        }

        /// <summary>
        /// Check if there is any records in the db for the target type
        /// </summary>
        /// <returns><c>true</c> if there's at least one row</returns>
        public bool Exists()
        {
            DetachedCriteria crit = DetachedCriteria.For<T>();
            FiltraEmpresaEstab(crit);
            return ActiveRecordMediator.Exists(typeof(T), crit);
        }

        /// <summary>
        /// Check if the <paramref name="id"/> exists in the database.
        /// </summary>
        /// <param name="id">The id to check on</param>
        /// <returns><c>true</c> if the ID exists; otherwise <c>false</c>.</returns>
        public bool Exists(object id)
        {
            return ActiveRecordMediator.Exists(typeof(T), id);
        }

        /// <summary>
        /// Check if any instance matches the criteria.
        /// </summary>
        /// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
        public bool Exists(params ICriterion[] criterias)
        {
            criterias = FiltraEmpresaEstab(criterias);
            return ActiveRecordMediator.Exists(typeof(T), criterias);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't 
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are. 
        ~ClasseModeloDAO()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }
        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                //if (managedResource != null)
                //{
                //    managedResource.Dispose();
                //    managedResource = null;
                //}
            }
            // free native resources if there are any.
            //if (nativeResource != IntPtr.Zero)
            //{
            //    Marshal.FreeHGlobal(nativeResource);
            //    nativeResource = IntPtr.Zero;
            //}

        }
    }
}