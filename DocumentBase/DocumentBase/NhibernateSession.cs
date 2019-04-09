using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Cfg;

namespace DocumentBase
{
    public class NhibernateSession
    {
        public static ISession OpenSession()
        {
            var configuration = new Configuration();
            var configurationPath = HttpContext.Current.Server.MapPath(@"~\Models\hibernate.cfg.xml");
            configuration.Configure(configurationPath);
            var userConfigurationFile = HttpContext.Current.Server.MapPath(@"~\Mappings\User.hbm.xml");
            var userConfigurationFileDoc = HttpContext.Current.Server.MapPath(@"~\Mappings\Document.hbm.xml");
            var userConfigurationFileSP = HttpContext.Current.Server.MapPath(@"~\Mappings\StoreProcedure.hbm.xml");
            configuration.AddFile(userConfigurationFile)
                         .AddFile(userConfigurationFileSP)
                         .AddFile(userConfigurationFileDoc);

            return configuration.BuildSessionFactory().OpenSession();
        }
    }
}