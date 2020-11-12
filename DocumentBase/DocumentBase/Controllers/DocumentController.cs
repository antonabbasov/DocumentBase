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
    /// <summary>
    /// Класс для работы с документами
    /// </summary>
    public class DocumentController : Controller
    {      
        /// <summary>
        /// Метод отображающий все документы в хранилище
        /// </summary>
        /// <returns>Список документов</returns>
        public ActionResult IndexAll()
        {
            using (ISession session = NhibernateSession.OpenSession()) 
            {
                IList<Document> documents;
                documents = session.Query<Document>().ToList();
                          
                return View(documents);
            }
        }

        /// <summary>
        /// Метод отображающий документы пользователя
        /// </summary>
        /// <returns>Список документов</returns>
        public ActionResult Index()
        {
            using (ISession session = NhibernateSession.OpenSession())
            {
                IList<Document> documents;
                User user = new User();

                user.id = long.Parse(Session["id"].ToString()); //выбираем авторизованного пользователя в бд
                user.login = Session["Login"].ToString();    
                         
                documents = session.Query<Document>().Where(a => a.author.id == user.id).ToList(); //выборка документов пользователя
                user.Documents = documents;

                return View(documents);
            }                     
        }
       
        /// <summary>
        /// Метод по созданию нового документа
        /// </summary>
        /// <returns>Форма для создания документа</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Метод создания документа
        /// </summary>
        /// <param name="document">заполненные данные по документу</param>
        /// <param name="fileupload">загруженный документ</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Document document, IEnumerable<HttpPostedFileBase> fileupload)
        {          
                string path = String.Empty;
                document.authorId = long.Parse(Session["id"].ToString());

                using (ISession session = NhibernateSession.OpenSession())
                {
                    foreach (var file in fileupload)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var extention = System.IO.Path.GetExtension(file.FileName);
                            path = System.IO.Path.Combine(Server.MapPath("~/Files/"), document.name + extention);
                            file.SaveAs(path);
                            var saveQuery = session.GetNamedQuery("FileSaveProcedure");
                            saveQuery.SetParameter("Name", document.name)
                            .SetParameter("authorId", document.authorId)
                            .SetParameter("changedate", document.changeDate)
                            .SetParameter("BinaryFile", path).ExecuteUpdate();

                            return RedirectToAction("IndexAll");
                        }
                        return View("Create");
                    }                   
                }
                return View("Create");
            }
            
        /// <summary>
        /// Метод по поиску документов
        /// </summary>
        /// <param name="sortOrder">порядок сортировки</param>
        /// <param name="searchString">поисковой запрос</param>
        /// <returns></returns>
        public ActionResult Search(string sortOrder, string searchString)
        {
            using (ISession session = NhibernateSession.OpenSession())
            {
                IList<Document> documents;
                IList<User> users;
              
                users = session.Query<User>().ToList();
                documents = session.Query<Document>().ToList();

                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : ""; //возможность сортировки по разным столбцам таблицы
                ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
                ViewBag.AutorSortParm = sortOrder == "Autor" ? "autor_desc" : "Autor";
               
                if (!String.IsNullOrEmpty(searchString))
                {
                    documents = documents.Where(s => s.name.Contains(searchString) || s.authorId.ToString().Contains(searchString)).ToList();
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        documents = documents.OrderByDescending(s => s.name).ToList();
                        break;
                    case "Date":
                        documents = documents.OrderBy(s => s.changeDate).ToList();
                        break;
                    case "date_desc":
                        documents = documents.OrderByDescending(s => s.changeDate).ToList();
                        break;
                    case "autor_desc":
                        documents = documents.OrderByDescending(s => s.author.login).ToList();
                        break;
                    case "Autor":
                        documents = documents.OrderBy(s => s.author.login).ToList();
                        break;
                    default:
                        documents = documents.OrderBy(s => s.changeDate).ToList();
                        break;
                }
                return View(documents);
            }        
        }
        
        /// <summary>
        /// Метод открытия документа
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ActionResult Open(string path)
        {
            Process.Start(path);

            return RedirectToAction("Search");
        }

       /// <summary>
       /// Метод по удалению документов из хранилища
       /// </summary>
       /// <param name="id">айди документа</param>
       /// <returns></returns>
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

       /// <summary>
       /// Метод удаления документа из бд
       /// </summary>
       /// <param name="id">айди документа</param>
       /// <param name="collection"></param>
       /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(long id)
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
