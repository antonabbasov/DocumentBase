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

namespace DocumentBase.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User objUser)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                
                using (ISession session = NhibernateSession.OpenSession())
                {                  
                    user = session.Query<User>().Where(a => a.login.Equals(objUser.login) && a.password.Equals(objUser.password)).FirstOrDefault();
                  
                    if (user != null)
                    {
                        Session["id"] = user.id.ToString();
                        Session["Login"] = user.login.ToString();                      
                        ViewBag.Message = user;
                        Session["documents"] = user.Documents;
                        return RedirectToAction("UserDashBoard"/*, user*/);
                    }
                }
            }
            return View(objUser);
        }

        public ActionResult UserDashBoard()
        {
            if (Session["id"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

    }
}
