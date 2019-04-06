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
        //
        // GET: /Login/

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
                    user = session.Query<User>().Where(a => a.Login.Equals(objUser.Login) && a.Password.Equals(objUser.Password)).FirstOrDefault();                    
                    if (user != null)
                    {
                        Session["id"] = user.id.ToString();
                        Session["Login"] = user.Login.ToString();                      
                        ViewBag.Message = user;

                        return RedirectToAction("UserDashBoard", user);
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
