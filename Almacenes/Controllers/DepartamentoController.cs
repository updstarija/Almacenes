using Almacenes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Almacenes.Controllers
{
    public class DepartamentoController : Controller
    {
        // GET: Departamento
        AlmacenesEntities db = new AlmacenesEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Get()
        {
            string btn = "";
            List<tDepartamento> lista = db.tDepartamento.Where(x => x.Estado == 1).ToList();
            List<object[]> tabla = new List<object[]>();
            foreach (var item in lista)
            {
                btn = "<div class='d-flex flex-nowrap text-nowrap justify-content-center'><button type='button' onclick='Edit(" + item.Id + ")' class='btn btn-outline-primary btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalDepartamento' style='font-size:12px'>Editar <i class='fas fa-pencil-alt'></i></button><button type='button' onclick='Delete(" + item.Id + ")' class='btn btn-outline-danger btn-sm m-1' style='font-size:12px'>Eliminar <i class='fas fa-trash-alt'></i></button></div>";
                var Descrip = item.Descripcion.Length > 50? item.Descripcion.Substring(0, 50) + "...":item.Descripcion;
                var listPersonasActicas = db.tPersona.Where(x=> x.IdDepartamento==item.Id && x.Estado==1);
                object[] obj = { (lista.IndexOf(item) + 1).ToString(), item.Nombre.ToUpper(), Descrip, listPersonasActicas.Count(), btn };
                tabla.Add(obj);
            }
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAll()
        {
            List<object> lista = new List<object>();
            foreach (var item in db.tDepartamento.Where(x => x.Estado == 1))
            {
                var o = new { Id = item.Id, Nombre = item.Nombre.ToUpper() };
                lista.Add(o);
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public object GetId(int Id)
        {
            var o = db.tDepartamento.Single(x => x.Id == Id);
            var obj = new { Id = o.Id, Nombre = o.Nombre.ToUpper(),Descripcion=o.Descripcion };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(int id, string nombre, string descripcion)
        {
            Status s = new Status();
            try
            {
                if (string.IsNullOrEmpty(nombre))
                {
                    s.tipo = 4;
                    s.msj = "El nombre del departamento no puede estar vacio.";
                }
                if (string.IsNullOrEmpty(descripcion))
                {
                    s.tipo = 4;
                    s.msj = "La descripcion del departamento no puede estar vacio.";
                }

                if (string.IsNullOrEmpty(s.msj))
                {

                    if (id == -1)
                    {
                        tDepartamento obj = new tDepartamento();
                        obj.Estado = 1;
                        obj.Nombre = nombre;
                        obj.Descripcion = descripcion;
                        db.tDepartamento.Add(obj);
                        db.SaveChanges();
                        s.tipo = 1;
                        s.msj = "Registro Exitoso";
                    }
                    else
                    {
                        var obj = db.tDepartamento.Single(x => x.Id == id);
                        obj.Nombre = nombre;
                        obj.Descripcion = descripcion;
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
                var obj = db.tDepartamento.Single(x => x.Id == Id);
                obj.Estado = 0;
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