using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;

namespace Almacenes.Controllers
{
    public class CargoController : Controller
    {
        // GET: Cargo
        AlmacenesEntities db = new AlmacenesEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Get()
        {
            string btn = "";
            List<tCargo> lista = db.tCargo.Where(x => x.Estado == 1).ToList();
            List<object[]> tabla = new List<object[]>();
            foreach (var item in lista)
            {
                btn = "<div class='d-flex flex-nowrap text-nowrap justify-content-center'><button type='button' onclick='Edit(" + item.Id + ")' class='btn btn-outline-primary btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalCargo' style='font-size:12px'>Editar <i class='fas fa-pencil-alt'></i></button><button type='button' data-bs-toggle='modal' data-bs-target='#ModalEliminar' onclick='Delete(" + item.Id + ")' class='btn btn-outline-danger btn-sm m-1' style='font-size:12px'>Eliminar <i class='fas fa-trash-alt'></i></button></div>";
                object[] obj = { (lista.IndexOf(item) + 1).ToString(), item.Nombre.ToUpper(), btn };
                tabla.Add(obj);
            }
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }

        public object GetId(int Id)
        {
            var o = db.tCargo.Single(x => x.Id == Id);
            var obj = new { Id = o.Id, Nombre = o.Nombre.ToUpper()};
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAll()
        {
            List<object> lista = new List<object>();
            foreach (var item in db.tCargo.Where(x => x.Estado == 1))
            {
                var o = new { Id = item.Id, Nombre = item.Nombre.ToUpper() };
                lista.Add(o);
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(tCargo o)
        {
            Status s = new Status();
            try
            {
                if (string.IsNullOrEmpty(o.Nombre))
                {
                    s.tipo = 4;
                    s.msj = "El nombre del cargo no puede estar vacio.";
                }
                if (string.IsNullOrEmpty(s.msj))
                {
                    if (o.Id==0)
                    {
                        o.Estado = 1;
                        db.tCargo.Add(o);
                        db.SaveChanges();
                        s.tipo = 1;
                        s.msj = "Registro Exitoso";
                    }
                    else
                    {
                        var obj = db.tCargo.Single(x => x.Id == o.Id);
                        obj.Nombre = o.Nombre;
                        db.SaveChanges();
                        s.tipo = 1;
                        s.msj = "Modificación Exitosa";
                    }
                   
                }  
            }
            catch (Exception e) 
            {
                s.tipo = 3;
                s.msj = e.Message;

            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveRole(tPersona o)
        {
            Status s = new Status();
            try
            {
                if (o.Idcargo <= 0)
                {
                    s.tipo = 4;
                    s.msj = "Debe seleccionar un cargo para la persona";
                }
                if (string.IsNullOrEmpty(s.msj))
                {
                    var p = db.tPersona.Single(x => x.Id == o.Id && x.Estado == 1);
                    p.Idcargo = o.Idcargo;
                    db.SaveChanges();
                    s.tipo = 1;
                    s.msj = "Registro Exitoso";
                }
            }
            catch (Exception e)
            {
                s.tipo = 3;
                s.msj = e.Message;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int Id)
        {
            Status s = new Status();
            try
            {
                foreach (var item in db.tPersona.Where(x=>x.Idcargo==Id))
                {
                    item.Idcargo = null;
                }
                db.SaveChanges();
                var obj = db.tCargo.Single(x => x.Id == Id);
                obj.Estado = -1;
                db.SaveChanges();
                s.tipo = 1;
                s.msj = "Se realizo la eliminación exitosamente";
            }
            catch (Exception e)
            {
                s.tipo = 3;
                s.msj = e.Message;
            }
            return Json(s, JsonRequestBehavior.AllowGet);

        }
    }
}