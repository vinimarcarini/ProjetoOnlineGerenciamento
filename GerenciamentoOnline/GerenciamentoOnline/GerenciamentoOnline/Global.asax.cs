using Castle.ActiveRecord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GerenciamentoOnline
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Inicializa(GetAssembly(), Server.MapPath("~/appconfig.xml"));
            Castle.ActiveRecord.ActiveRecordStarter.UpdateSchema();
        }

        public static Assembly[] GetAssembly()
        {
            return new Assembly[] {
                System.Reflection.Assembly.GetAssembly(typeof(kardapio.Suprimentos.Usuario)),
            };
        }

        public static void Inicializa(Assembly[] arAs, string fileName)
        {
            Castle.ActiveRecord.Framework.Config.XmlConfigurationSource source = new Castle.ActiveRecord.Framework.Config.XmlConfigurationSource(fileName);
            Type[] ar = new Type[] { typeof(string) };

            ActiveRecordStarter.Initialize(arAs, source, ar);
        }
    }
}
