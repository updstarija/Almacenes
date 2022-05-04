using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;

namespace Almacenes.Controllers
{
    public class SolicitudController : Controller
    {
        // GET: Solicitud
        AlmacenesEntities db = new AlmacenesEntities();
        public ActionResult Index()
        {
            return View();
        }
        public List<object[]> loadTable(List<tSolicitud> lista)
        {
            List<object[]> tabla = new List<object[]>();
            foreach (var item in lista)
            {
                string btn = "<div class='d-flex flex-nowrap text-nowrap justify-content-center'>";
                string fecha = "";
                string estado = "";
                switch (item.Estado)
                {
                    case 1:
                        estado = "RECHAZADA";
                        fecha = item.Fecha.Value.ToShortDateString();
                        btn += "<button type='button' onclick='detail(" + item.Id + ")' class='btn btn-outline-warning btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalSolicitud' style='font-size:12px'>Ver <i class='fas fa-eye'></i></button>";
                        break;
                    case 2:
                        estado = "GUARDADA";
                        fecha = item.Fecha == null ? "" : item.Fecha.Value.ToShortDateString();
                        btn += "<button type='button' onclick='Edit(" + item.Id + ")' class='btn btn-outline-primary btn-sm m-1 tooltip-test' data-bs-toggle='modal' data-bs-target='#ModalSolicitud' style='font-size:12px' title='Editar'><i class='fas fa-pencil-alt'></i></button><button type='button' data-bs-toggle='modal' data-bs-target='#ModalEliminar' onclick='Delete(" + item.Id + ")' class='btn btn-outline-danger btn-sm m-1 tooltip-test' style='font-size:12px' title='Eliminar'><i class='fas fa-trash-alt'></i></button><button type='button' onclick='Send(" + item.Id + ")' class='btn btn-outline-success btn-sm m-1 tooltip-test' style='font-size:12px' title='Enviar'><i class='fas fa-share-square'></i></button>";
                        break;
                    case 3:
                        estado = "ENVIADA";
                        fecha = item.Fecha.Value.ToShortDateString();
                        btn += "<button type='button' onclick='detail(" + item.Id + ")' class='btn btn-outline-warning btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalSolicitud' style='font-size:12px'>Ver <i class='fas fa-eye'></i></button>";
                        break;
                    case 4:
                        estado = "APROBADA";
                        fecha = item.Fecha.Value.ToShortDateString();
                        btn += "<button type='button' onclick='detail(" + item.Id + ")' class='btn btn-outline-warning btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalSolicitud' style='font-size:12px'>Ver <i class='fas fa-eye'></i></button>";
                        break;
                }
                btn += "</div>";
                var fechaRespuesta = estado == "APROBADA" || estado == "RECHAZADA" ? "<div class='text-center' style='cursor: default'>" + string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.FechaRespuesta) + "</div>" : "<div class='text-center' style='cursor: default'><i class='fas fa-hourglass-half'></i></div>";
                var Descrip = item.Descripcion.Length > 35 ? item.Descripcion.Substring(0, 35) + "..." : item.Descripcion;
                object[] obj = { (lista.IndexOf(item) + 1).ToString(), Descrip.ToUpper(), fecha, item.tTipoSolicitud.Nombre.ToUpper(), estado, fechaRespuesta, btn };
                tabla.Add(obj);
            }
            return tabla;
        }
        public ActionResult Get()
        {
            var usuario = db.tUsuario.Single(x => x.Nomusu == User.Identity.Name);
            List<tSolicitud> lista = db.tSolicitud.OrderByDescending(x => x.Id).Where(x => x.Idusuario == usuario.Id && x.Estado != 0).ToList();
            List<object[]> tabla = new List<object[]>();
            tabla = loadTable(lista);
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListByState(int filter)
        {
            var usuario = db.tUsuario.Single(x => x.Nomusu == User.Identity.Name);
            List<object[]> tabla = new List<object[]>();
            List<tSolicitud> lista = new List<tSolicitud>();
            if (filter == 4 || filter == 5)
            {
                var state = filter == 4 ? 4 : 2;
                var list = filter == 1 || filter == 5 || filter == 4 ? db.tSolicitud.OrderByDescending(x => x.FechaRespuesta).Where(x => x.Idusuario == usuario.Id && x.Estado ==4).ToList(): db.tSolicitud.OrderByDescending(x => x.Fecha).Where(x => x.Idusuario == usuario.Id && x.Estado == 4).ToList();

                foreach (var item in list)
                {
                    bool verif = filter == 5 ? false : true;
                    foreach (var itemDS in item.tDetalleSolicitud)
                    {
                        if (filter == 4)
                        {
                            if (itemDS.Estado != state)
                            {
                                verif = false;
                            }
                        }
                        else if (itemDS.Estado == state)
                        {
                            verif = true;
                        }
                    }
                    if (verif == true) { lista.Add(item); }
                }
            }
            else
            {
                lista = db.tSolicitud.OrderByDescending(x => x.Fecha).Where(x => x.Idusuario == usuario.Id && x.Estado == filter).ToList();
            }
            tabla = loadTable(lista);
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(tSolicitud soli)
        {
            Status s = new Status();
            try
            {
                if (string.IsNullOrEmpty(soli.Descripcion))
                {
                    s.tipo = 4;
                    s.msj = "El nombre del material no puede estar vacio.";
                }
                if (soli.Idtiposolicitud == 0)
                {
                    s.tipo = 4;
                    s.msj = "Debe seleccionar un tipo de solicitud.";
                }
                if (soli.tDetalleSolicitud.Count <= 0)
                {
                    s.tipo = 4;
                    s.msj = "Debe seleccionar al menos un material";
                }
                if (string.IsNullOrEmpty(s.msj))
                {

                    if (soli.Id == 0)
                    {
                        int IdUsuario = db.tUsuario.Single(x => x.Nomusu == User.Identity.Name).Id;
                        soli.Estado = 2;
                        soli.Idusuario = IdUsuario;
                        soli.Fecha = DateTime.Now;
                        db.tSolicitud.Add(soli);
                        db.SaveChanges();
                        foreach (var item in soli.tDetalleSolicitud)
                        {
                            item.Idsolicitud = soli.Id;
                            item.Estado = 1;
                        }
                        db.SaveChanges();
                        s.tipo = 1;
                        s.msj = "Registro Exitoso";
                    }
                    else
                    {
                        var obj = db.tSolicitud.Single(x => x.Id == soli.Id);
                        obj.Descripcion = soli.Descripcion;
                        obj.Idtiposolicitud = soli.Idtiposolicitud;
                        obj.Fecha = DateTime.Now;
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

        public ActionResult SaveSend(tSolicitud soli)
        {
            Status s = new Status();
            try
            {
                if (string.IsNullOrEmpty(soli.Descripcion))
                {
                    s.tipo = 4;
                    s.msj = "El nombre del material no puede estar vacio.";
                }
                if (soli.Idtiposolicitud == 0)
                {
                    s.tipo = 4;
                    s.msj = "Debe seleccionar un tipo de solicitud.";
                }
                if (soli.tDetalleSolicitud.Count <= 0)
                {
                    s.tipo = 4;
                    s.msj = "Debe seleccionar al menos un material";
                }
                if (string.IsNullOrEmpty(s.msj))
                {
                    if (soli.Id == 0)
                    {
                        int IdUsuario = db.tUsuario.Single(x => x.Nomusu == User.Identity.Name).Id;
                        soli.Estado = 3;
                        soli.Idusuario = IdUsuario;
                        soli.Fecha = DateTime.Now;
                        db.tSolicitud.Add(soli);
                        db.SaveChanges();
                        foreach (var item in soli.tDetalleSolicitud)
                        {
                            item.Idsolicitud = soli.Id;
                            item.Estado = 1;
                        }
                        db.SaveChanges();
                        s.tipo = 1;
                        s.msj = "Se envío la solicitud con exito";
                    }
                    else
                    {
                        var obj = db.tSolicitud.Single(x => x.Id == soli.Id);
                        obj.Descripcion = soli.Descripcion;
                        obj.Idtiposolicitud = soli.Idtiposolicitud;
                        obj.Estado = 3;
                        obj.Fecha = DateTime.Now;
                        db.SaveChanges();
                        s.tipo = 1;
                        s.msj = "Se envío la solicitud con exito";
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
                var soli = db.tSolicitud.Single(x => x.Id == Id);
                soli.Estado = 0;
                db.SaveChanges();
                s.tipo = 1;
                s.msj = "Se eliminó la solicitud con exito.";
            }
            catch (Exception e)
            {
                s.tipo = 3;
                s.msj = e.Message;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMaterialEdit(int Id)
        {
            var o = db.tDetalleSolicitud.Single(x => x.Id == Id);
            o.Estado = 0;
            db.SaveChanges();
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Send(int Id)
        {
            Status s = new Status();
            try
            {
                var soli = db.tSolicitud.Single(x => x.Id == Id);
                soli.Estado = 3;
                soli.Fecha = DateTime.Now;
                db.SaveChanges();
                s.tipo = 1;
                s.msj = "Se envió la solicitud con exito.";

            }
            catch (Exception e)
            {
                s.tipo = 3;
                s.msj = e.Message;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public object GetId(int Id)
        {
            var o = db.tSolicitud.Single(x => x.Id == Id);
            var obj = new { Id = o.Id, Descripcion = o.Descripcion, Idtiposolicitud = o.Idtiposolicitud };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMateriales(int IdSolicitud)
        {
            List<object> lista = new List<object>();
            var o = db.tDetalleSolicitud.Where(x => x.Idsolicitud == IdSolicitud && x.Estado >= 1).ToList();
            int estado = Convert.ToInt32(db.tSolicitud.Single(x => x.Id == IdSolicitud).Estado);
            foreach (var item in o)
            {
                string Material = db.tMaterial.Single(x => x.Id == item.tDetalleMaterial.IdMaterial).Nombre.ToUpper();
                var unid = db.tUnidad.Single(x => x.Id == item.tDetalleMaterial.IdUnidad);
                string cantUnid = unid.Cantidad != 1 ? " - " + unid.Cantidad : "";

                var materiales = new { Id = item.Id, Material = Material, NombreUnidad = (unid.Nombre.ToUpper() + cantUnid), CantidadSolicitada = item.CantidadSolicitada, CantidadAprobada = item.CantidadAprobada, Estado = estado, EstadoDM = item.Estado };
                lista.Add(materiales);
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddMateriales(tDetalleSolicitud ds)
        {
            ds.Estado = 1;
            db.tDetalleSolicitud.Add(ds);
            db.SaveChanges();
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnidad(int Id)
        {
            var unidades = db.tUnidad.Where(x => x.Estado == 1);
            string opciones = "";
            foreach (var item in unidades)
            {
                var unidadSeleccionada = db.tDetalleMaterial.SingleOrDefault(x => x.IdMaterial == Id && x.IdUnidad == item.Id && x.Estado == 1);
                if (unidadSeleccionada != null)
                {
                    string cantUnid = item.Cantidad != 1 ? " - " + item.Cantidad : "";
                    opciones += "<option value='" + item.Id + "'>" + item.Nombre + cantUnid + "</option>";
                }
            }
            return Json(opciones, JsonRequestBehavior.AllowGet);
        }
    }
}