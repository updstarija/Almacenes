using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;

namespace Almacenes.Controllers
{
    public class UnidadController : Controller
    {
        // GET: Unidad
        AlmacenesEntities db = new AlmacenesEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get()
        {
            string btn = "";
            List<tUnidad> lista = db.tUnidad.Where(x => x.Estado == 1).ToList();
            List<object[]> tabla = new List<object[]>();
            foreach (var item in lista)
            {
                btn = "<div class='d-flex flex-nowrap text-nowrap justify-content-center'><button type='button' onclick='Edit(" + item.Id + ")' class='btn btn-outline-primary btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalUnidad' style='font-size:12px'>Editar <i class='fas fa-pencil-alt'></i></button><button type='button' data-bs-toggle='modal' data-bs-target='#ModalEliminar' onclick='Delete(" + item.Id + ")' class='btn btn-outline-danger btn-sm m-1' style='font-size:12px'>Eliminar <i class='fas fa-trash-alt'></i></button>";
                object[] obj = { (lista.IndexOf(item) + 1).ToString(), item.Nombre.ToUpper(),item.Cantidad, btn };
                tabla.Add(obj);
            }
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAll()
        {
            List<object> lista = new List<object>();
            foreach (var item in db.tUnidad.Where(x => x.Estado == 1))
            {
                var o = new { Id = item.Id, Nombre = item.Nombre.ToUpper(),Cantidad=item.Cantidad };
                lista.Add(o);
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public object GetId(int Id)
        {
            var o = db.tUnidad.Single(x => x.Id == Id);
            var obj = new { Id = o.Id, Nombre = o.Nombre.ToUpper(), Cantidad = o.Cantidad };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(tUnidad o)
        {
            Status s = new Status();
            try
            {
                if (string.IsNullOrEmpty(o.Nombre))
                {
                    s.tipo = 4;
                    s.msj = "El nombre de la unidad no puede estar vacio.";
                }
                if (o.Cantidad<1 || o.Cantidad==null)
                {
                    s.tipo = 4;
                    s.msj = "Verifique el valor de Cantidad";
                }
                if (string.IsNullOrEmpty(s.msj))
                {
                    if (o.Id == 0)
                    {
                        o.Estado = 1;
                        db.tUnidad.Add(o);
                        db.SaveChanges();
                        s.tipo = 1;
                        s.msj = "Registro Exitoso";
                    }
                    else
                    {
                        var obj = db.tUnidad.Single(x => x.Id == o.Id);
                        obj.Nombre = o.Nombre;
                        obj.Cantidad = o.Cantidad;
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

        public ActionResult Delete(int Id)
        {
            Status s = new Status();
            try
            {
                if (string.IsNullOrEmpty(s.msj))
                {
                    var ds = db.tDetalleSolicitud.Where(x=>x.tDetalleMaterial.IdUnidad==Id).ToList();
                    if (ds.Count==0)
                    {
                        var obj = db.tUnidad.Single(x => x.Id == Id);
                        obj.Estado = -1;
                        db.SaveChanges();
                        s.tipo = 1;
                        s.msj = "Se realizo la eliminación exitosamente";
                    }
                    else
                    {
                        s.tipo = 5;
                        s.msj = "La unidad ya se encuentra asignada a un material o una solicitud";
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
    }
}