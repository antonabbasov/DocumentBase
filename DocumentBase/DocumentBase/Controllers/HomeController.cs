using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentBase.Models;
using NHibernate.Linq;
using NHibernate.Dialect;
using NHibernate.Cfg;
using NHibernate.Driver;
using System.Reflection;
using NHibernate;
using System.Diagnostics;

namespace DocumentBase.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {           
            //using (ISession session = NhibernateSession.OpenSession())
            //{
            //    string scriptSql = System.IO.File.ReadAllText("H:\\DocumentBase\\SqlScript.sql");
            //    var query = session.CreateSQLQuery(scriptSql);
            //    query.ExecuteUpdate();
            //}
                          
            return View();
        }

    }
}
