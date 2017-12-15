using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Queries;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetoModeloDDD.Domain.DAO.Auxiliar
{
    /// <summary>
    /// Allow programmers to use the
    /// ActiveRecord functionality without direct reference
    /// to <see cref="ActiveRecordBase"/>
    /// </summary>
    public class RecordMediator
    {
        private static ISessionFactoryHolder holder
        {
            get
            {
                return ActiveRecordMediator.GetSessionFactoryHolder();
            }
        }

        internal static void EnsureInitialized(Type type)
        {
            if (holder == null)
            {
                String message = String.Format("An ActiveRecord class ({0}) was used but the framework seems not " +
                                               "properly initialized. Did you forget about ActiveRecordStarter.Initialize() ?",
                                               type.FullName);
                throw new ActiveRecordException(message);
            }
        }

        /// <summary>
        /// Finds an object instance by its primary key.
        /// </summary>
        /// <param name="targetType">The AR subclass type</param>
        /// <param name="id">ID value</param>
        /// <param name="throwOnNotFound"><c>true</c> if you want an exception to be thrown
        /// if the object is not found</param>
        /// <exception cref="ObjectNotFoundException">if <c>throwOnNotFound</c> is set to
        /// <c>true</c> and the row is not found</exception>
        public static object FindByPrimaryKey(Type targetType, object id, bool throwOnNotFound)
        {
            EnsureInitialized(targetType);
            bool hasScope = holder.ThreadScopeInfo.HasInitializedScope;
            ISession session = holder.CreateSession(targetType);

            try
            {
                object loaded;
                // Load() and Get() has different semantics with regard to the way they
                // handle null values, Get() _must_ check that the value exists, Load() is allowed
                // to return an uninitialized proxy that will throw when you access it later.
                // in order to play well with proxies, we need to use this approach.
                if (throwOnNotFound)
                {
                    loaded = session.Load(targetType, id);
                }
                else
                {
                    loaded = session.Get(targetType, id);
                }
                //If we are not in a scope, we want to initialize the entity eagerly, since other wise the 
                //user will get an exception when it access the entity's property, and it will try to lazy load itself and find that
                //it has no session.
                //If we are in a scope, it is the user responsability to keep the scope alive if he wants to use 
                if (!hasScope)
                {
                    NHibernateUtil.Initialize(loaded);
                }
                return loaded;
            }
            catch (ObjectNotFoundException ex)
            {
                holder.FailSession(session);

                String message = String.Format("Could not find {0} with id {1}", targetType.Name, id);
                throw new NotFoundException(message, ex);
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                throw new ActiveRecordException("Could not perform FindByPrimaryKey for " + targetType.Name + ". Id: " + id, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Finds an object instance by its primary key.
        /// </summary>
        /// <param name="targetType">The AR subclass type</param>
        /// <param name="id">ID value</param>
        public static object FindByPrimaryKey(Type targetType, object id)
        {
            return FindByPrimaryKey(targetType, id, true);
        }

        /// <summary>
        /// Searches and returns the first row.
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="orders">The sort order - used to determine which record is the first one</param>
        /// <param name="criterias">The criteria expression</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public static object FindFirst(Type targetType, Order[] orders, params ICriterion[] criterias)
        {
            EnsureInitialized(targetType);

            ISession session = holder.CreateSession(targetType);

            try
            {
                ICriteria sessionCriteria = session.CreateCriteria(targetType);

                sessionCriteria.SetMaxResults(1);

                foreach (ICriterion cond in criterias)
                {
                    sessionCriteria.Add(cond);
                }

                AddOrdersToCriteria(sessionCriteria, orders);

                return SupportingUtils.BuildArray(targetType, sessionCriteria.List());
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                throw new ActiveRecordException("Could not perform FindAll for " + targetType.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        private static void AddOrdersToCriteria(ICriteria criteria, IEnumerable<NHibernate.Criterion.Order> orders)
        {
            if (orders != null)
            {
                foreach (Order order in orders)
                {
                    criteria.AddOrder(order);
                }
            }
        }

        /// <summary>
        /// Searches and returns the first row.
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="criterias">The criteria expression</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public static object FindFirst(Type targetType, params ICriterion[] criterias)
        {
            Array obj = (Array)FindFirst(targetType, null, criterias);

            return obj.Length > 0 ? obj.GetValue(0) : null;
        }

        /// <summary>
        /// Searches and returns the first row.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="detachedCriteria">The criteria.</param>
        /// <param name="orders">The sort order - used to determine which record is the first one.</param>
        /// <returns>A <c>targetType</c> instance or <c>null.</c></returns>
        public static object FindFirst(Type targetType, DetachedCriteria detachedCriteria, params Order[] orders)
        {
            Array result = SlicedFindAll(targetType, 0, 1, orders, detachedCriteria);
            return (result != null && result.Length > 0 ? result.GetValue(0) : null);
        }

        /// <summary>
        /// Searches and returns the first row.
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="criteria">The criteria expression</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public static object FindFirst(Type targetType, DetachedCriteria criteria)
        {
            Array result = SlicedFindAll(targetType, 0, 1, criteria);

            if (result.Length > 1)
            {
                throw new ActiveRecordException(targetType.Name + ".FindOne returned " + result.Length +
                                                " rows. Expecting one or none");
            }

            return (result.Length == 0) ? null : result.GetValue(0);
        }


        /// <summary>
        /// Searches and returns the a row. If more than one is found,
        /// throws <see cref="ActiveRecordException"/>
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="criterias">The criteria expression</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public static object FindOne(Type targetType, params ICriterion[] criterias)
        {
            Array result = SlicedFindAll(targetType, 0, 1, criterias);

            if (result.Length > 1)
            {
                throw new ActiveRecordException(targetType.Name + ".FindOne returned " + result.Length +
                                                " rows. Expecting one or none");
            }

            return (result.Length == 0) ? null : result.GetValue(0);
        }

        /// <summary>
        /// Searches and returns a row. If more than one is found,
        /// throws <see cref="ActiveRecordException"/>
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="criteria">The criteria</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public static object FindOne(Type targetType, DetachedCriteria criteria)
        {
            Array result = SlicedFindAll(targetType, 0, 2, criteria);

            if (result.Length > 1)
            {
                throw new ActiveRecordException(targetType.Name + ".FindOne returned " + result.Length +
                                                " rows. Expecting one or none");
            }

            return (result.Length == 0) ? null : result.GetValue(0);
        }

        /// <summary>
        /// Returns a portion of the query results (sliced)
        /// </summary>
        public static Array SlicedFindAll(Type targetType, int firstResult, int maxresults,
                                          Order[] orders, params ICriterion[] criterias)
        {
            EnsureInitialized(targetType);

            ISession session = holder.CreateSession(targetType);

            try
            {
                ICriteria sessionCriteria = session.CreateCriteria(targetType);

                foreach (ICriterion cond in criterias)
                {
                    sessionCriteria.Add(cond);
                }

                if (orders != null)
                {
                    foreach (Order order in orders)
                    {
                        sessionCriteria.AddOrder(order);
                    }
                }

                sessionCriteria.SetFirstResult(firstResult);
                sessionCriteria.SetMaxResults(maxresults);

                return SupportingUtils.BuildArray(targetType, sessionCriteria.List());
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                throw new ActiveRecordException("Could not perform SlicedFindAll for " + targetType.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Returns a portion of the query results (sliced)
        /// </summary>
        public static Array SlicedFindAll(Type targetType, int firstResult, int maxresults, params ICriterion[] criterias)
        {
            return SlicedFindAll(targetType, firstResult, maxresults, null, criterias);
        }

        /// <summary>
        /// Returns a portion of the query results (sliced)
        /// </summary>
        public static Array SlicedFindAll(Type targetType, int firstResult, int maxResults,
                                          Order[] orders, DetachedCriteria criteria)
        {
            EnsureInitialized(targetType);

            ISession session = holder.CreateSession(targetType);

            try
            {
                ICriteria executableCriteria = criteria.GetExecutableCriteria(session);
                AddOrdersToCriteria(executableCriteria, orders);
                executableCriteria.SetFirstResult(firstResult);
                executableCriteria.SetMaxResults(maxResults);

                return SupportingUtils.BuildArray(targetType, executableCriteria.List());
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;
            }
            catch (InvalidCastException ex)
            {
                holder.FailSession(session);

                throw new ActiveRecordException("Could not perform SlicedFindAll for " + targetType.Name, ex);
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                throw new ActiveRecordException("Could not perform SlicedFindAll for " + targetType.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Returns a portion of the query results (sliced)
        /// </summary>
        public static Array SlicedFindAll(Type targetType, int firstResult, int maxResults,
                                          DetachedCriteria criteria)
        {
            return SlicedFindAll(targetType, firstResult, maxResults, null, criteria);
        }


        /// <summary>
        /// Returns all instances found for the specified type.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static Array FindAll(Type targetType)
        {
            return FindAll(targetType, (Order[])null);
        }

        /// <summary>
        /// Returns all instances found for the specified type
        /// using sort orders and criterias.
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="orders"></param>
        /// <param name="criterias"></param>
        /// <returns></returns>
        public static Array FindAll(Type targetType, Order[] orders, params ICriterion[] criterias)
        {
            EnsureInitialized(targetType);

            ISession session = holder.CreateSession(targetType);

            try
            {
                ICriteria sessionCriteria = session.CreateCriteria(targetType);

                foreach (ICriterion cond in criterias)
                {
                    sessionCriteria.Add(cond);
                }

                AddOrdersToCriteria(sessionCriteria, orders);

                return SupportingUtils.BuildArray(targetType, sessionCriteria.List());
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                throw new ActiveRecordException("Could not perform FindAll for " + targetType.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Returns all instances found for the specified type
        /// using criterias.
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="criterias"></param>
        /// <returns></returns>
        public static Array FindAll(Type targetType, params ICriterion[] criterias)
        {
            return FindAll(targetType, null, criterias);
        }

        /// <summary>
        /// Returns all instances found for the specified type according to the criteria
        /// </summary>
        public static Array FindAll(Type targetType, DetachedCriteria detachedCriteria, params Order[] orders)
        {
            EnsureInitialized(targetType);

            ISession session = holder.CreateSession(targetType);

            try
            {
                ICriteria criteria = detachedCriteria.GetExecutableCriteria(session);

                AddOrdersToCriteria(criteria, orders);

                return SupportingUtils.BuildArray(targetType, criteria.List());
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;
            }
            catch (Exception ex)
            {
                holder.FailSession(session);
                throw ex;
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Returns all instances found for the specified type according to the criteria
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="detachedQuery">The query expression</param>
        /// <returns>The <see cref="Array"/> of results.</returns>
        public static Array FindAll(Type targetType, IDetachedQuery detachedQuery)
        {
            EnsureInitialized(targetType);

            ISession session = holder.CreateSession(targetType);
            try
            {
                IQuery executableQuery = detachedQuery.GetExecutableQuery(session);
                return SupportingUtils.BuildArray(targetType, executableQuery.List());
            }
            catch (ValidationException)
            {
                holder.FailSession(session);
                throw;
            }
            catch (Exception exception)
            {
                holder.FailSession(session);
                throw new ActiveRecordException("Could not perform FindAll for " + targetType.Name, exception);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Finds records based on a property value - automatically converts null values to IS NULL style queries.
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="property">A property name (not a column name)</param>
        /// <param name="value">The value to be equals to</param>
        /// <returns></returns>
        public static Array FindAllByProperty(Type targetType, String property, object value)
        {
            ICriterion criteria = (value == null) ? Expression.IsNull(property) : Expression.Eq(property, value);
            return FindAll(targetType, criteria);
        }

        /// <summary>
        /// Finds records based on a property value - automatically converts null values to IS NULL style queries.
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="orderByColumn">The column name to be ordered ASC</param>
        /// <param name="property">A property name (not a column name)</param>
        /// <param name="value">The value to be equals to</param>
        /// <returns></returns>
        public static Array FindAllByProperty(Type targetType, String orderByColumn, String property, object value)
        {
            ICriterion criteria = (value == null) ? Expression.IsNull(property) : Expression.Eq(property, value);
            return FindAll(targetType, new Order[] { Order.Asc(orderByColumn) }, criteria);
        }

        /// <summary>
        /// Executes the query
        /// </summary>
        /// <param name="q">The query</param>
        /// <returns></returns>
        public static object ExecuteQuery(IActiveRecordQuery q)
        {
            return ActiveRecordBase.ExecuteQuery(q);
        }

        /// <summary>
        /// Returns the number of records of the specified
        /// type in the database
        /// </summary>
        /// <example>
        /// <code>
        /// [ActiveRecord]
        /// public class User : ActiveRecordBase
        /// {
        ///   ...
        ///
        ///   public static int CountUsers()
        ///   {
        ///     return Count(typeof(User));
        ///   }
        /// }
        /// </code>
        /// </example>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>The count result</returns>
        protected internal static int Count(Type targetType)
        {
            CountQuery query = new CountQuery(targetType);

            return (int)ExecuteQuery(query);
        }

        /// <summary>
        /// Returns the number of records of the specified
        /// type in the database
        /// </summary>
        /// <example>
        /// <code>
        /// [ActiveRecord]
        /// public class User : ActiveRecordBase
        /// {
        ///   ...
        ///
        ///   public static int CountUsersLocked()
        ///   {
        ///     return Count(typeof(User), "IsLocked = ?", true);
        ///   }
        /// }
        /// </code>
        /// </example>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="filter">A sql where string i.e. Person=? and DOB &gt; ?</param>
        /// <param name="args">Positional parameters for the filter string</param>
        /// <returns>The count result</returns>
        public static int Count(Type targetType, string filter, params object[] args)
        {
            CountQuery query = new CountQuery(targetType, filter, args);

            return (int)ExecuteQuery(query);
        }

        /// <summary>
        /// Returns the number of records of the specified
        /// type in the database
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="criteria">The criteria expression</param>
        /// <returns>The count result</returns>
        public static int Count(Type targetType, params ICriterion[] criteria)
        {
            CountQuery query = new CountQuery(targetType, criteria);

            return (int)ExecuteQuery(query);
        }

        /// <summary>
        /// Returns the number of records of the specified
        /// type in the database
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="detachedCriteria">The criteria expression</param>
        /// <returns>The count result</returns>
        public static int Count(Type targetType, DetachedCriteria detachedCriteria)
        {
            CountQuery query = new CountQuery(targetType, detachedCriteria);

            return (int)ExecuteQuery(query);
        }

        /// <summary>
        /// Check if there is any records in the db for the target type
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns><c>true</c> if there's at least one row</returns>
        public static bool Exists(Type targetType)
        {
            return Count(targetType) > 0;
        }


        /// <summary>
        /// Check if there is any records in the db for the target type
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="filter">A sql where string i.e. Person=? and DOB &gt; ?</param>
        /// <param name="args">Positional parameters for the filter string</param>
        /// <returns><c>true</c> if there's at least one row</returns>
        public static bool Exists(Type targetType, string filter, params object[] args)
        {
            return Count(targetType, filter, args) > 0;
        }

        /// <summary>
        /// Check if the <paramref name="id"/> exists in the database.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="id">The id to check on</param>
        /// <returns><c>true</c> if the ID exists; otherwise <c>false</c>.</returns>
        public static bool Exists(Type targetType, object id)
        {
            EnsureInitialized(targetType);
            ISession session = holder.CreateSession(targetType);

            try
            {
                return session.Get(targetType, id) != null;
            }
            catch (Exception ex)
            {
                throw new ActiveRecordException("Could not perform Exists for " + targetType.Name + ". Id: " + id, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Check if any instance matches the criteria.
        /// </summary>
        /// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
        public static bool Exists(Type targetType, params ICriterion[] criterias)
        {
            return Count(targetType, criterias) > 0;
        }

        /// <summary>
        /// Check if any instance matching the criteria exists in the database.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="detachedCriteria">The criteria expression</param>
        /// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
        public static bool Exists(Type targetType, DetachedCriteria detachedCriteria)
        {
            return Count(targetType, detachedCriteria) > 0;
        }

        /// <summary>
        /// Saves the instance to the database
        /// </summary>
        /// <param name="instance">The ActiveRecord instance to be deleted</param>
        public static void Save(object instance)
        {
            InternalSave(instance, false);
        }

        /// <summary>
		/// Saves the instance to the database. If the primary key is unitialized
		/// it creates the instance on the database. Otherwise it updates it.
		/// <para>
		/// If the primary key is assigned, then you must invoke <see cref="Create()"/>
		/// or <see cref="Update()"/> instead.
		/// </para>
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be saved</param>
		/// <param name="flush">if set to <c>true</c>, the operation will be followed by a session flush.</param>
		private static void InternalSave(object instance, bool flush)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            EnsureInitialized(instance.GetType());

            ISession session = holder.CreateSession(instance.GetType());

            try
            {
                session.SaveOrUpdate(instance);

                if (flush)
                {
                    session.Flush();
                }
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                // NHibernate catches our ValidationException on Create so it could be the innerexception here
                if (ex.InnerException is ValidationException)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw new ActiveRecordException("Could not perform Save for " + instance.GetType().Name, ex);
                }
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Saves the instance to the database and flushes the session. If the primary key is unitialized
        /// it creates the instance on the database. Otherwise it updates it.
        /// <para>
        /// If the primary key is assigned, then you must invoke <see cref="Create(object)"/>
        /// or <see cref="Update(object)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="instance">The ActiveRecord instance to be saved</param>
        public static void SaveAndFlush(object instance)
        {
            InternalSave(instance, true);
        }

        /// <summary>
        /// Saves a copy of instance to the database
        /// </summary>
        /// <param name="instance">The transient instance to be copied</param>
        /// <returns>The saved ActiveRecord instance</returns>
        public static object SaveCopy(object instance)
        {
            return InternalSaveCopy(instance, false);
        }

        /// <summary>
        /// Saves a copy of the instance to the database. If the primary key is unitialized
        /// it creates the instance on the database. Otherwise it updates it.
        /// <para>
        /// If the primary key is assigned, then you must invoke <see cref="Create()"/>
        /// or <see cref="Update()"/> instead.
        /// </para>
        /// </summary>
        /// <param name="instance">The transient instance to be saved</param>
        /// <param name="flush">if set to <c>true</c>, the operation will be followed by a session flush.</param>
        /// <returns>The saved ActiveRecord instance.</returns>
        private static object InternalSaveCopy(object instance, bool flush)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            EnsureInitialized(instance.GetType());

            ISession session = holder.CreateSession(instance.GetType());

            try
            {
                object persistent = session.Merge(instance);

                if (flush)
                {
                    session.Flush();
                }

                return persistent;
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                // NHibernate catches our ValidationException on Create so it could be the innerexception here
                if (ex.InnerException is ValidationException)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw new ActiveRecordException("Could not perform SaveCopy for " + instance.GetType().Name, ex);
                }
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Saves a copy of the instance to the database and flushes the session. If the primary key is uninitialized
        /// it creates the instance in the database. Otherwise it updates it.
        /// <para>
        /// If the primary key is assigned, then you must invoke <see cref="Create(object)"/>
        /// or <see cref="Update(object)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="instance">The transient instance to be copied</param>
        /// <returns>The saved ActiveRecord instance</returns>
        public static object SaveCopyAndFlush(object instance)
        {
            return InternalSaveCopy(instance, true);
        }

        /// <summary>
        /// Creates (Saves) a new instance to the database.
        /// </summary>
        /// <param name="instance">The ActiveRecord instance to be deleted</param>
        public static void Create(object instance)
        {
            InternalCreate(instance, false);
        }

        /// <summary>
        /// Creates (Saves) a new instance to the database and flushes the session.
        /// </summary>
        /// <param name="instance">The ActiveRecord instance to be created on the database</param>
        public static void CreateAndFlush(object instance)
        {
            InternalCreate(instance, true);
        }

        /// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be created on the database</param>
		/// <param name="flush">if set to <c>true</c>, the operation will be followed by a session flush.</param>
		private static void InternalCreate(object instance, bool flush)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            EnsureInitialized(instance.GetType());

            ISession session = holder.CreateSession(instance.GetType());

            try
            {
                session.Save(instance);

                if (flush)
                {
                    session.Flush();
                }
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                // NHibernate catches our ValidationException, and as such it is the innerexception here
                if (ex is ValidationException)
                    throw;

                if (ex.InnerException is ValidationException)
                    throw ex.InnerException;

                throw new ActiveRecordException("Could not perform Create for " + instance.GetType().Name, ex);

            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Persists the modification on the instance
        /// state to the database.
        /// </summary>
        /// <param name="instance">The ActiveRecord instance to be deleted</param>
        public static void Update(object instance)
        {
            InternalUpdate(instance, false);
        }

        /// <summary>
        /// Persists the modification on the instance
        /// state to the database and flushes the session.
        /// </summary>
        /// <param name="instance">The ActiveRecord instance to be updated on the database</param>
        public static void UpdateAndFlush(object instance)
        {
            InternalUpdate(instance, true);
        }

        /// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be updated on the database</param>
		/// <param name="flush">if set to <c>true</c>, the operation will be followed by a session flush.</param>
		private static void InternalUpdate(object instance, bool flush)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            EnsureInitialized(instance.GetType());

            ISession session = holder.CreateSession(instance.GetType());

            try
            {
                session.Update(instance);

                if (flush)
                {
                    session.Flush();
                }
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                throw new ActiveRecordException("Could not perform Update for " + instance.GetType().Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Deletes the instance from the database.
        /// </summary>
        /// <param name="instance">The ActiveRecord instance to be deleted</param>
        public static void Delete(object instance)
        {
            InternalDelete(instance, false);
        }

        /// <summary>
        /// Deletes the instance from the database and flushes the session.
        /// </summary>
        /// <param name="instance">The ActiveRecord instance to be deleted</param>
        public static void DeleteAndFlush(object instance)
        {
            InternalDelete(instance, true);
        }

        /// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be deleted</param>
		/// <param name="flush">if set to <c>true</c>, the operation will be followed by a session flush.</param>
		private static void InternalDelete(object instance, bool flush)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            EnsureInitialized(instance.GetType());

            ISession session = holder.CreateSession(instance.GetType());

            try
            {
                session.Delete(instance);

                if (flush)
                {
                    session.Flush();
                }
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;

            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                throw new ActiveRecordException("Could not perform Delete for " + instance.GetType().Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Refresh the instance from the database.
        /// </summary>
        /// <param name="instance">The ActiveRecord instance to be reloaded</param>
        public static void Refresh(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            EnsureInitialized(instance.GetType());

            ISession session = holder.CreateSession(instance.GetType());

            try
            {
                session.Refresh(instance);
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                // NHibernate catches our ValidationException, and as such it is the innerexception here
                if (ex.InnerException is ValidationException)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw new ActiveRecordException("Could not perform Refresh for " + instance.GetType().Name, ex);
                }
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Testing hock only.
        /// </summary>
        public static ISessionFactoryHolder GetSessionFactoryHolder()
        {
            return ActiveRecordMediator.GetSessionFactoryHolder();
        }

        /// <summary>
        /// From NHibernate documentation:
        /// Persist all reachable transient objects, reusing the current identifier
        /// values. Note that this will not trigger the Interceptor of the Session.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="replicationMode">The replication mode.</param>
        public static void Replicate(object instance, ReplicationMode replicationMode)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            EnsureInitialized(instance.GetType());

            ISession session = holder.CreateSession(instance.GetType());

            try
            {
                session.Replicate(instance, replicationMode);
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                // NHibernate catches our ValidationException, and as such it is the innerexception here
                if (ex.InnerException is ValidationException)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw new ActiveRecordException("Could not perform Replicate for " + instance.GetType().Name, ex);
                }
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Evicts the specified instance from the first level cache (session level).
        /// </summary>
        /// <param name="instance">The instance.</param>
        public static void Evict(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            if (SessionScope.Current != null)
            {
                SessionScope.Current.Evict(instance);
            }
        }

        /// <summary>
        /// Evicts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        public static void GlobalEvict(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            ISessionFactory factory = GetFactory(type);

            factory.Evict(type);
        }

        /// <summary>
        /// Evicts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The id.</param>
        public static void GlobalEvict(Type type, object id)
        {
            if (type == null) throw new ArgumentNullException("type");

            ISessionFactory factory = holder.GetSessionFactory(type);

            if (factory == null)
            {
                throw new ActiveRecordException("Could not find registered session factory for type " + type.FullName);
            }

            factory.Evict(type, id);
        }

        /// <summary> 
        /// From NH docs: Evict all entries from the second-level cache. This method occurs outside
        /// of any transaction; it performs an immediate "hard" remove, so does not respect
        /// any transaction isolation semantics of the usage strategy. Use with care.
        /// </summary>
        public static void EvictEntity(string entityName)
        {
            ISessionFactory[] factories = holder.GetSessionFactories();

            foreach (ISessionFactory factory in factories)
            {
                factory.EvictEntity(entityName);
            }
        }

        /* NOT AVAILABLE ON NH 2.0.x

		/// <summary>
		/// From NH docs: Evict an entry from the second-level  cache. This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy. Use with care.
		/// </summary>
		/// <param name="entityName">Name of the entity.</param>
		/// <param name="id">The id.</param>
		public static void EvictEntity(string entityName, object id)
		{
			ISessionFactory[] factories = ActiveRecordBase.holder.GetSessionFactories();

			foreach(ISessionFactory factory in factories)
			{
				factory.EvictEntity(entityName, id);
			}
		}
		*/

        /// <summary>
        /// From NH docs: Evict all entries from the process-level cache.  This method occurs outside
        /// of any transaction; it performs an immediate "hard" remove, so does not respect
        /// any transaction isolation semantics of the usage strategy.  Use with care.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        public static void EvictCollection(string roleName)
        {
            ISessionFactory[] factories = holder.GetSessionFactories();

            foreach (ISessionFactory factory in factories)
            {
                factory.EvictCollection(roleName);
            }
        }

        /// <summary>
        /// From NH docs: Evict an entry from the process-level cache.  This method occurs outside
        /// of any transaction; it performs an immediate "hard" remove, so does not respect
        /// any transaction isolation semantics of the usage strategy.  Use with care.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="id">The id.</param>
        public static void EvictCollection(string roleName, object id)
        {
            ISessionFactory[] factories = holder.GetSessionFactories();

            foreach (ISessionFactory factory in factories)
            {
                factory.EvictCollection(roleName, id);
            }
        }

        /// <summary>
        /// From NH docs: Evict any query result sets cached in the named query cache region.
        /// </summary>
        /// <param name="cacheRegion">The cache region.</param>
        public static void EvictQueries(string cacheRegion)
        {
            ISessionFactory[] factories = holder.GetSessionFactories();

            foreach (ISessionFactory factory in factories)
            {
                factory.EvictQueries(cacheRegion);
            }
        }

        private static ISessionFactory GetFactory(Type type)
        {
            ISessionFactory factory = holder.GetSessionFactory(type);

            if (factory == null)
            {
                throw new ActiveRecordException("Could not find registered session factory for type " + type.FullName);
            }
            return factory;
        }
    }
}