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
    public class DocumentController : Controller
    {
        //
        // GET: /Document/

        public ActionResult IndexAll()
        {
            IList<Document> documents;

            using (ISession session = NhibernateSession.OpenSession())  
            {
                documents = session.Query<Document>().ToList();             
            }

            return View(documents);
        }

        public ActionResult Index()
        {         
            IList<Document> documents;
            User user= new User();
            user.id = long.Parse(Session["id"].ToString());

            using (ISession session = NhibernateSession.OpenSession())
            {
                documents = session.Query<Document>().Where(a => a.AuthorId == user.id).ToList();
            }

            return View(documents);
        }

        // GET: Document/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Document/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Document document, IEnumerable<HttpPostedFileBase> fileupload)
        {          
                string path = String.Empty;
                document.AuthorId = long.Parse(Session["id"].ToString());

                using (ISession session = NhibernateSession.OpenSession())
                {
                    foreach (var file in fileupload)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var extention = System.IO.Path.GetExtension(file.FileName);
                            path = System.IO.Path.Combine(Server.MapPath("~/Files/"), document.Name + extention);
                            file.SaveAs(path);
                            var saveQuery = session.GetNamedQuery("FileSaveProcedure");
                            saveQuery.SetParameter("Name", document.Name)
                            .SetParameter("authorId", document.AuthorId)
                            .SetParameter("changedate", document.ChangeDate)
                            .SetParameter("BinaryFile", path).ExecuteUpdate();

                            return RedirectToAction("IndexAll");
                        }
                        return View("Create");
                    }                   
                }
                return View("Create");
            }
            
        
        //Document/Search
        public ActionResult Search(string sortOrder, string searchString)
        {
            IList<Document> documents;

            using (ISession session = NhibernateSession.OpenSession())
            {               
                documents = session.Query<Document>().ToList();
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
                ViewBag.AutorSortParm = sortOrder == "Autor" ? "autor_desc" : "Autor";
               
                if (!String.IsNullOrEmpty(searchString))
                {
                    documents = documents.Where(s => s.Name.Contains(searchString) || s.AuthorId.ToString().Contains(searchString)).ToList();
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        documents = documents.OrderByDescending(s => s.Name).ToList();
                        break;
                    case "Date":
                        documents = documents.OrderBy(s => s.ChangeDate).ToList();
                        break;
                    case "date_desc":
                        documents = documents.OrderByDescending(s => s.ChangeDate).ToList();
                        break;
                    case "autor_desc":
                        documents = documents.OrderByDescending(s => s.AuthorId).ToList();
                        break;
                    case "Autor":
                        documents = documents.OrderBy(s => s.AuthorId).ToList();
                        break;
                    default:
                        documents = documents.OrderBy(s => s.ChangeDate).ToList();
                        break;
                }
            }
            return View(documents);
        }

        public ActionResult Open(string path)
        {
            Process.Start(path);

            return RedirectToAction("Search");
        }

       
        // GET: Document/Delete
        public ActionResult Delete(int id)
        {           
            Document document = new Document();

            using (ISession session = NhibernateSession.OpenSession())
            {
                document = session.Query<Document>().Where(b => b.id == id).FirstOrDefault();
            }
            ViewBag.SubmitAction = "Confirm delete";

            return View("Edit", document);
        }

        // POST: Document/Delete
        [HttpPost]
        public ActionResult Delete(long id, FormCollection collection)
        {
            try
            {               
                using (ISession session = NhibernateSession.OpenSession())
                {
                    Document document = session.Get<Document>(id);

                    using (ITransaction trans = session.BeginTransaction())
                    {
                        session.Delete(document);
                        trans.Commit();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

    }
}
