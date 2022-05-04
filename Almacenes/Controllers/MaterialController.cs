using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;
namespace Almacenes.Controllers
{
    public class MaterialController : Controller
    {
        // GET: Material
        AlmacenesEntities db = new AlmacenesEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Get()
        {
            string btn = "";
            List<tMaterial> lista = db.tMaterial.Where(x => x.Estado == 1).ToList();
            List<object[]> tabla = new List<object[]>();
            foreach (var item in lista)
            {
                string unidades = "";
                var detalle = db.tDetalleMaterial.Where(x => x.IdMaterial == item.Id && x.tUnidad.Estado==1 && x.Estado==1);
                foreach (var item2 in detalle)
                {
                    var unid = db.tUnidad.Single(x => x.Id == item2.IdUnidad);
                    string cantUnid = unid.Cantidad != 1 ? " - " + unid.Cantidad : "";
                    unidades += unid.Nombre.ToUpper()+cantUnid + ", ";
                }
                unidades = unidades.Substring(0,unidades.Length-2);
                btn = "<div class='d-flex flex-nowrap text-nowrap justify-content-center'><button type='button' onclick='Edit(" + item.Id + ")' class='btn btn-outline-primary btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalMaterial' style='font-size:12px'>Editar <i class='fas fa-pencil-alt'></i></button><button type='button' data-bs-toggle='modal' data-bs-target='#ModalEliminar' onclick='Delete(" + item.Id + ")' class='btn btn-outline-danger btn-sm m-1' style='font-size:12px'>Eliminar <i class='fas fa-trash-alt'></i></button></div>";
                object[] obj = { (lista.IndexOf(item) + 1).ToString(), item.Nombre.ToUpper(), unidades, btn };
                tabla.Add(obj);
            }
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }
        public object GetId(int Id)
        {
            var o = db.tMaterial.Single(x => x.Id == Id);
            var obj = new { Id = o.Id, Nombre = o.Nombre.ToUpper() };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public object GetDetail(int Id)
        {
            var unidades = db.tUnidad.Where(x => x.Estado == 1);
            string opciones = "";
            foreach (var item in unidades)
            {
                var unidadSeleccionada = db.tDetalleMaterial.SingleOrDefault(x => x.IdMaterial == Id && x.IdUnidad == item.Id && x.Estado==1 && x.tUnidad.Estado==1);
                string cantUnid = item.Cantidad != 1 ? " - " + item.Cantidad : "";
                if (unidadSeleccionada != null)
                {
                    opciones += "<option value='" + item.Id + "' selected='selected'>" + item.Nombre.ToUpper()+cantUnid + "</option>";
                }
                else
                {
                    opciones += "<option value='" + item.Id + "'>" + item.Nombre.ToUpper() + cantUnid + "</option>";
                }
            }
            return Json(opciones, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(int id, string nombre, List<object> unidades)
        {
            Status s = new Status();
            try
            {
                if (string.IsNullOrEmpty(nombre))
                {
                    s.tipo = 4;
                    s.msj = "El nombre del material no puede estar vacio.";
                }
                if (unidades == null)
                {
                    s.tipo = 4;
                    s.msj = "Debe seleccionar al menos una unidad para el material.";
                }
                if (string.IsNullOrEmpty(s.msj))
                {

                    if (id == 0)
                    {
                        tMaterial m = new tMaterial();
                        m.Estado = 1;
                        m.Nombre = nombre;
                        db.tMaterial.Add(m);
                        db.SaveChanges();
                        tDetalleMaterial dm = new tDetalleMaterial();
                        foreach (var item in unidades.ToList())
                        {
                            dm.IdMaterial = Convert.ToInt32(m.Id);
                            dm.IdUnidad = Convert.ToInt32(item);
                            dm.Estado = 1;
                            db.tDetalleMaterial.Add(dm);
                            db.SaveChanges();
                        }
                        s.tipo = 1;
                        s.msj = "Registro Exitoso";
                    }
                    else
                    {
                        var obj = db.tMaterial.Single(x => x.Id == id);
                        obj.Nombre = nombre;
                        db.SaveChanges();
                        
                        var detalleAnterior = db.tDetalleMaterial.Where(x => x.IdMaterial == obj.Id).ToList();
                        foreach (var item in detalleAnterior)
                        {
                            tDetalleMaterial detalle = db.tDetalleMaterial.SingleOrDefault(x=> x.Id==item.Id);
                            detalle.Estado=-1;
                            db.SaveChanges();
                        }
                        foreach (var item in unidades.ToList())
                        {
                            int IdUnidad = Convert.ToInt32(item);
                            int IdMaterial = Convert.ToInt32(obj.Id);
                            tDetalleMaterial detMat = db.tDetalleMaterial.SingleOrDefault(x=>x.IdMaterial== IdMaterial && x.IdUnidad== IdUnidad);
                            tDetalleMaterial dm = detMat!=null?detMat: new tDetalleMaterial();
                            dm.IdMaterial = IdMaterial;
                            dm.IdUnidad = IdUnidad;
                            dm.Estado = 1;
                            if (detMat == null)
                            {
                                db.tDetalleMaterial.Add(dm);
                            }
                            db.SaveChanges();
                        }
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
                var dm = db.tDetalleSolicitud.Where(x=>x.tDetalleMaterial.IdMaterial==Id).ToList();
                if (dm.Count==0)
                {
                    var obj = db.tMaterial.Single(x => x.Id == Id);
                    obj.Estado = -1;
                    db.SaveChanges();
                    s.tipo = 1;
                    s.msj = "Se realizo la eliminación exitosamente";
                }
                else
                {
                    s.tipo = 4;
                    s.msj = "El material esta asignado a una solicitud";
                }
            }
            catch (Exception e)
            {
                s.tipo = 3;
                s.msj = e.Message;
            }
            return Json(s, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Buscar(string texto)
        {

            List<tMaterial> lista = db.tMaterial.SqlQuery("Select top 3 * from tMaterial m where m.Nombre like '%" + texto + "%' and m.Estado=1").ToList();
            var datos = "";
            if (lista != null && texto != "")
            {
                foreach (var item in lista)
                {
                    datos += "<a href='#' class='list-group-item list-group-item-action'  onclick='SelectMaterial(" + item.Id + ")'>" + item.Nombre + " </li>";
                }
            }
            return Json(datos, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetIdDetailMaterial(int IdUnidad, int IdMaterial)
        {
            int IdDetalleMaterial = db.tDetalleMaterial.Single(x => x.IdMaterial == IdMaterial && x.IdUnidad == IdUnidad && x.Estado==1).Id;
            return Json(IdDetalleMaterial, JsonRequestBehavior.AllowGet);
        }

    }
}