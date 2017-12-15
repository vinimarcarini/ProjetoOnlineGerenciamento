using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using kardapio.Suprimentos;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using ProjetoModeloDDD.Domain.DAO.Auxiliar;
using ProjetoModeloDDD.Domain.DAO.Entities;
using ProjetoModeloDDD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ProjetoModeloDDD.Domain.Auxiliar
{
    public static class MetodosAuxiliares
    {
        public static string VersaoAtual()
        {
            return "17.4.7";
        }

        public static bool ExecutaSQL(string sql)
        {
            bool success = true;
            var connection = DAOStarter.GetConexao();

            try
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                success = false;
                LogErros.GravaLog(e, MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return success;
        }

        /// Remove todos os espaços em branco da string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveWhitespace(string text)
        {
            return new string(text.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public static bool ValidaCnpj(string cnpj)
        {
            bool valid = false;

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
            {
                return false;
            }

            try
            {
                tempCnpj = cnpj.Substring(0, 12);
                soma = 0;
                for (int i = 0; i < 12; i++)
                {
                    soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
                }
                resto = (soma % 11);
                if (resto < 2)
                {
                    resto = 0;
                }
                else
                {
                    resto = 11 - resto;
                }
                digito = resto.ToString();
                tempCnpj = tempCnpj + digito;
                soma = 0;
                for (int i = 0; i < 13; i++)
                {
                    soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
                }
                resto = (soma % 11);
                if (resto < 2)
                {
                    resto = 0;
                }
                else
                {
                    resto = 11 - resto;
                }

                digito = digito + resto.ToString();

                valid = cnpj.EndsWith(digito);
            }
            catch (Exception e)
            {
                LogErros.GravaLog(e, "MetodosAuxiliares/" + MethodBase.GetCurrentMethod().Name);
            }

            return valid;
        }

        internal static int SqlCount(string sql)
        {
            int total = 0;
            IDbConnection connection = null;
            try
            {
                connection = DAOStarter.GetConexao();
                var command = connection.CreateCommand();
                command.CommandText = sql;
                try
                {
                    var dr = command.ExecuteReader();

                    try
                    {
                        while (dr.Read())
                        {
                            total = Convert.ToInt32(dr["total"]);
                        }
                    }
                    catch (Exception e)
                    {
                        LogErros.GravaLog(e, "MetodosAuxiliares/SqlCount");
                    }
                    finally
                    {
                        if (!dr.IsClosed)
                        {
                            dr.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    LogErros.GravaLog(e, "MetodosAuxiliares/SqlCount");
                }
                finally
                {
                    if (connection != null && connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                LogErros.GravaLog(e, "MetodosAuxiliares/SqlCount");
            }
            return total;
        }

        public static bool IsNumeric(string s)
        {
            float output;

            return float.TryParse(s, out output);
        }

        public static Usuario UsuarioLogado(ICustomPrincipal user)
        {
            Usuario usuario = null;

            try
            {
                ClasseModeloDAO<Usuario> daoUsuario = ClasseModeloDAO<Usuario>.Create(user);
                usuario = daoUsuario.FindByPrimaryKey(user.IdUsuario);
                daoUsuario.Dispose();
            }
            catch (Exception e)
            {
                LogErros.GravaLog(e, MethodBase.GetCurrentMethod().Name);
            }

            return usuario;
        }

        public static bool IsSQLServer()
        {
            bool isSQLServer = false;

            string appconfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appconfig.xml");

            var fs = new FileStream(appconfig, FileMode.Open, FileAccess.Read);

            using (var sr = new StreamReader(fs))
            {
                string content = sr.ReadToEnd();

                isSQLServer = content.Contains("Data Source") && content.Contains("Catalog");
            }

            return isSQLServer;
        }

        public static void SeparaDataHora(object obj, PropertyInfo[] propriedades)
        {
            PropertyInfo[] propsTemp = propriedades.Where(
                p => p.PropertyType == typeof(DateTime?) ||
                     p.PropertyType == typeof(DateTime)).ToArray();

            //foreach (PropertyInfo prop in propriedades)
            foreach (PropertyInfo prop in propsTemp)
            {
                if (prop.PropertyType == typeof(DateTime?))
                {
                    DateTime? valor = (DateTime?)prop.GetValue(obj, null);
                    if (valor.HasValue && valor != new DateTime())
                    {
                        prop.SetValue(obj, SeparaData(valor), null);
                    }
                    else if (valor == new DateTime())
                    {
                        prop.SetValue(obj, null, null);
                    }
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    DateTime valor = (DateTime)prop.GetValue(obj, null);
                    if (valor != new DateTime())
                    {
                        prop.SetValue(obj, SeparaData(valor), null);
                    }
                    else
                    {
                        prop.SetValue(obj, null, null);
                    }
                }
            }
        }

        public static DateTime? SeparaData(DateTime? valor)
        {
            DateTime? ret = null;
            if (valor.HasValue && valor.Value != new DateTime())
            {
                ret = valor.Value.Date;
            }
            return ret;
        }

        public static string CriptografaString(string senhastrCriptografar)
        {
            try
            {
                SHA1CryptoServiceProvider SHA1 = new SHA1CryptoServiceProvider();
                byte[] byteV = System.Text.Encoding.UTF8.GetBytes(senhastrCriptografar);
                byte[] byteH = SHA1.ComputeHash(byteV);
                SHA1.Clear();
                return Convert.ToBase64String(byteH);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Obtem a sessão ativa quando utilizado em web
        /// </summary>
        /// <returns>Sessão a ser usada</returns>
        public static ISession GetActiveRecordSession()
        {
            ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
            ISessionScope activeScope = holder.ThreadScopeInfo.GetRegisteredScope();
            ISession session = null;
            var key = holder.GetSessionFactory(typeof(ActiveRecordBase));
            if (activeScope == null)
            {
                session = holder.CreateSession(typeof(ActiveRecordBase));
            }
            else
            {
                if (activeScope.IsKeyKnown(key))
                    session = activeScope.GetSession(key);
                else
                    session = holder.GetSessionFactory(typeof(ActiveRecordBase)).OpenSession();
            }
            return session;
        }

        public static DateTime? GetDateFromData(DateTime? value)
        {
            DateTime? ret = null;
            if (value.HasValue && (value != new DateTime()))
            {
                ret = value.Value.Date;
            }
            return ret;
        }

        public static string HoraFromDateTime(DateTime? date, string horaTxt)
        {
            string ret = "";
            if (string.IsNullOrEmpty(horaTxt) || horaTxt == "00:00:00")
            {
                //se tem valor
                if (date.HasValue)
                {
                    //se o valor for diferente de 00:00:00
                    if (date.Value.ToString("HH:mm:ss") != "00:00:00")
                        ret = date.Value.ToString("HH:mm:ss");
                    //se a data está certo
                    else if (date != new DateTime())
                        ret = "00:00:00";  //DateTime.Now.ToString("HH:mm:ss")
                }
            }
            else
            {
                ret = horaTxt;
            }
            return ret;
        }

        public static DataTable GetConsultaSQL(string cSql)
        {
            IDataReader dr = null;
            DataTable dt = null;
            IDbConnection connection = null;

            try
            {
                connection = DAOStarter.GetConexao();
                var cmd = connection.CreateCommand();
                cmd.CommandText = cSql;

                dr = cmd.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);

            }
            catch (Exception e)
            {
                LogErros.GravaLog(e, MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }
                if (connection != null && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return dt;
        }

        public static List<object> GetObjectsFromDataTable(DataTable table, Type tipo)
        {
            List<object> lsObjects = new List<object>();

            PropertyInfo prop;

            try
            {
                foreach (DataRow row in table.Rows)
                {
                    object obj = Activator.CreateInstance(tipo);

                    foreach (DataColumn column in table.Columns)
                    {
                        prop = tipo.GetProperty(column.ColumnName);

                        //se o tipo for uma classe, não pode adicionar porque foi retornado somente o Id na consulta
                        if (!prop.PropertyType.IsClass)
                        {
                            //se a propriedade não é nula e retornou valor do banco
                            if (prop != null && row[column].GetType().Name != "DBNull")
                            {
                                if (row[column].GetType().Name == "Double" && prop.PropertyType == typeof(decimal?))
                                {
                                    prop.SetValue(obj, Convert.ToDecimal(row[column]));
                                }
                                else
                                {
                                    prop.SetValue(obj, row[column]);
                                }
                            }
                        }
                    }

                    lsObjects.Add(obj);
                }
            }
            catch (Exception e)
            {
                LogErros.GravaLog(e, MethodBase.GetCurrentMethod().Name);
            }

            return lsObjects;
        }

        public static CustomClasseModelo CustomPrincipalLogado(Estabelecimento estab)
        {
            CustomClasseModelo user = new CustomClasseModelo()
            {
                IdEstab = estab.Id.Value,
                IdEmpresa = estab.Empresa.Id
            };
            return user;
        }

        public static CustomClasseModelo CustomPrincipalLogado(ICustomPrincipal usr)
        {
            CustomClasseModelo user = new CustomClasseModelo()
            {
                IdEstab = usr.IdEstab,
                IdEmpresa = usr.IdEmpresa
            };
            return user;
        }

        public static DateTime DateTimeNow(Estabelecimento estab)
        {
            //O fuso horário será configurado com base no fuso selecionado no estabelecimento,
            //Caso não possua fuso selecionado devolve a hora do servidor
            return DateTime.Now;
        }

        public static string RemoveAcentos(string strTexto)
        {
            string strRetorno;
            if (string.IsNullOrEmpty(strTexto))
            {
                strRetorno = String.Empty;
            }
            else
            {
                // # Cristiano, 29/10/2014.
                // O código ninja abaixo remove os acentos da string.
                // Acho que só Deus sabe como funciona isso, mas juro que funciona.
                byte[] bytes = System.Text.Encoding.GetEncoding("iso-8859-8").GetBytes(strTexto);
                strRetorno = System.Text.Encoding.UTF8.GetString(bytes);
            }

            return strRetorno;
        }

        public static ISession GetARSession()
        {
            ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
            var factory = holder.GetSessionFactory(typeof(ActiveRecordBase));
            return factory.OpenSession();
        }

        public static Estabelecimento EstabelecimentoLogado(ICustomPrincipal user)
        {
            ClasseModeloDAO<Estabelecimento> dao = ClasseModeloDAO<Estabelecimento>.Create(user);
            Estabelecimento es = dao.FindFirst();
            dao.Dispose();
            return es;
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = null;

            bytes = Convert.FromBase64String(str);

            return bytes;

        }

        public static string GetString(byte[] bytes)
        {
            string result = "";

            result = Convert.ToBase64String(bytes);

            return result;
        }

        public static T ClonarObjeto<T>(T source)
        {
            string value = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static AbstractCriterion CriaFiltroAfterId_DataHora(DateTime data, string horaTxt, int id)
        {
            AbstractCriterion crit = Expression.And(Expression.And(Expression.Eq("Data", data), Expression.Eq("HoraTxt", horaTxt)), Expression.Gt("Id", id));
            return crit;
        }

        public static AbstractCriterion CriaFiltroBeforeId_DataHora(DateTime data, string horaTxt, int id)
        {
            AbstractCriterion crit = Expression.And(Expression.And(Expression.Eq("Data", data), Expression.Eq("HoraTxt", horaTxt)), Expression.Lt("Id", id));
            return crit;
        }

        public static AbstractCriterion CriaFiltroAfterData(DateTime data, string horaTxt)
        {
            AbstractCriterion crit1 = Expression.Gt("Data", data);
            AbstractCriterion crit2 = Expression.And(Expression.Ge("Data", data), Expression.Gt("HoraTxt", horaTxt));
            AbstractCriterion crit = Expression.Or(crit1, crit2);
            return crit;
        }

        public static AbstractCriterion CriaFiltroAfterOrEqData(DateTime data, string horaTxt)
        {
            AbstractCriterion crit1 = Expression.Gt("Data", data);
            AbstractCriterion crit2 = Expression.And(Expression.Ge("Data", data), Expression.Ge("HoraTxt", horaTxt));
            AbstractCriterion crit = Expression.Or(crit1, crit2);
            return crit;
        }

        public static AbstractCriterion CriaFiltroBeforeData(DateTime data, string horaTxt)
        {
            AbstractCriterion crit1 = Expression.Lt("Data", data);
            AbstractCriterion crit2 = Expression.And(Expression.Le("Data", data), Expression.Lt("HoraTxt", horaTxt));
            AbstractCriterion crit = Expression.Or(crit1, crit2);
            return crit;
        }

        public static AbstractCriterion CriaFiltroBeforeOrEqData(DateTime data, string horaTxt)
        {
            AbstractCriterion crit1 = Expression.Lt("Data", data);
            AbstractCriterion crit2 = Expression.And(Expression.Le("Data", data), Expression.Le("HoraTxt", horaTxt));
            AbstractCriterion crit = Expression.Or(crit1, crit2);
            return crit;
        }

        public static void OrdenaDescPorDataHora(DetachedCriteria crit)
        {
            crit.AddOrder(Order.Desc("Data")).AddOrder(Order.Desc("HoraTxt")).AddOrder(Order.Desc("Id"));
        }

        public static void OrdenaAscPorDataHora(DetachedCriteria crit)
        {
            crit.AddOrder(Order.Asc("Data")).AddOrder(Order.Asc("HoraTxt")).AddOrder(Order.Asc("Id"));
        }

        public static string OnlyNunbers(string str)
        {
            string ret = "";
            foreach (var item in str)
            {
                if (Char.IsNumber(item))
                {
                    ret += item;
                }
            }
            return ret;
        }

    }
}
