using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;

namespace Almacenes.Controllers
{
    public class TipoController : Controller
    {
        // GET: Tipo

        AlmacenesEntities db = new AlmacenesEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            List<object> lista = new List<object>();
            foreach (var item in db.tTipo.Where(x=>x.Estado==1))
            {
                var o = new { Id = item.Id, Nombre = item.Nombre.ToUpper() };
                lista.Add(o);
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
    }
}