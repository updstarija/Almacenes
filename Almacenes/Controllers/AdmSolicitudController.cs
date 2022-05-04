using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;

namespace Almacenes.Controllers
{
    public class AdmSolicitudController : Controller
    {
        // GET: AdmSolicitud
        AlmacenesEntities db = new AlmacenesEntities();
        public ActionResult Index()
        {
            return View();
        }

        public List<object[]> loadTable(List<tSolicitud> lista,int EstadoFiltro)
        {
            string btn = "";
            List<object[]> tabla = new List<object[]>();
            foreach (var item in lista)
            {
                btn = "<div class='d-flex flex-nowrap text-nowrap justify-content-center'>";
                string paterno = "";
                string materno = "";
                var p = db.tUsuario.Single(x => x.Id == item.Idusuario).tPersona;
                if (p.Paterno != null)
                {
                    paterno = p.Paterno.ToUpper();
                }
                if (p.Materno != null)
                {
                    materno = p.Materno.ToUpper();
                }
                string estado = "";
                switch (item.Estado)
                {
                    case 1:
                        estado = "RECHAZADA";
                        btn += "<button type='button' onclick='Detail(" + item.Id + ")' class='text-nowrap btn btn-outline-warning btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalSolicitud' style='font-size:12px'>Ver <i class='fas fa-eye'></i></button>";
                        break;
                    case 3:
                        estado = "PENDIENTE";
                        btn += "<button type='button' onclick='Check(" + item.Id + ")' class='text-nowrap btn btn-outline-primary btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalSolicitud' style='font-size:12px'>Ver <i class='fas fa-eye'></i></button>";
                        break;
                    case 4:
                        estado = "APROBADA";
                        btn += "<button type='button' onclick='Detail(" + item.Id + ")' class='text-nowrap btn btn-outline-warning btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalSolicitud' style='font-size:12px'>Ver <i class='fas fa-eye'></i></button>";
                        break;
                }
                var fechaFin = item.FechaRespuesta?.AddDays(1);
                var fechaActual = DateTime.Now;
                var btnDisabl = estado == "APROBADA" && EstadoFiltro != 4 && fechaActual <= fechaFin ? "" : "disabled";
                var btn2 = " <button id='btnEstSolic" + item.Id + "' type='button' class='btn btn-outline-primary btn-sm m-1 tooltip-test' onclick='UpdateStateSolicitud(" + item.Id + ")' " + btnDisabl + " title='Cambiar a Pendiente'><i class='fas fa-sync'></i></button>";
                btn += btn2 + "</div>";
                var fechaRespuesta = estado == "APROBADA" || estado == "RECHAZADA" ? "<div class='text-center' style='cursor: default'>" + string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.FechaRespuesta) + "</div>" : "<div class='text-center' style='cursor: default'><i class='fas fa-hourglass-half'></i></div>";
                var Descrip = item.Descripcion.Length > 30 ? item.Descripcion.Substring(0, 30) + "..." : item.Descripcion;
                object[] obj = { (lista.IndexOf(item) + 1).ToString(), p.Nombre.ToUpper() + " " + paterno + " " + materno, item.Fecha.Value.ToShortDateString(), Descrip, estado, fechaRespuesta, btn };
                tabla.Add(obj);
            }
            return tabla;
        }
        public ActionResult Get()
        {
            List<object[]> tabla = new List<object[]>();
            List<tSolicitud> lista = db.tSolicitud.OrderByDescending(x => x.Fecha).Where(x => (x.Estado == 1 || x.Estado == 3 || x.Estado == 4)).ToList();
            tabla = loadTable(lista,-1);
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListByState(int filter)
        {
            List<object[]> tabla = new List<object[]>();
            List<tSolicitud> lista = new List<tSolicitud>();
            if (filter == 4 || filter == 5)
            {
                var state = filter == 4 ? 4 : 2;
                var list = filter == 1 || filter == 5 || filter == 4? db.tSolicitud.OrderByDescending(x => x.FechaRespuesta).Where(x => x.Estado == 4).ToList() : db.tSolicitud.OrderByDescending(x => x.Fecha).Where(x => x.Estado == 4).ToList();
                
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
                lista = db.tSolicitud.OrderByDescending(x => x.Fecha).Where(x => x.Estado == filter).ToList();
            }
            tabla = loadTable(lista,filter);
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetId(int Id)
        {
            var o = db.tSolicitud.Single(x => x.Id == Id);
            string paterno = "";
            string materno = "";
            var p = db.tUsuario.Single(x => x.Id == o.Idusuario).tPersona;
            if (p.Paterno != null)
            {
                paterno = p.Paterno.ToUpper();
            }
            if (p.Materno != null)
            {
                materno = p.Materno.ToUpper();
            }
            var obj = new
            {
                Id = o.Id,
                Fecha = o.Fecha.Value.ToShortDateString(),
                Solicitante = p.Nombre.ToUpper() + " " + paterno + " " + materno,
                Descripcion = o.Descripcion,
                Respuesta = o.Respuesta
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMateriales(int Id)
        {
            int estado = Convert.ToInt32(db.tSolicitud.SingleOrDefault(x => x.Id == Id).Estado);
            string fila = "";
            if (estado == 3)
            {
                var materiales = db.tDetalleSolicitud.Where(x => x.Idsolicitud == Id && x.Estado >= 1).ToList();
                int c = 0;
                foreach (var item in materiales)
                {
                    //int cantidad = Convert.ToInt32(item.CantidadSolicitada);
                    int cantidad = Convert.ToInt32(item.CantidadAprobada);
                    string check = item.Estado == 2 ? "checked" : "";
                    string btn = "<div class='d-flex justify-content-center pt-2'><input type='checkbox' id='Check" + item.Id + "' name='CheckCantidadAprobada[]' onclick='Accept(" + item.Id + ")' class='style-checkbox tooltip-test' title='Rechazar/Aprobar' " + check + "></div>";
                    c++;
                    string material = db.tMaterial.Single(x => x.Id == item.tDetalleMaterial.IdMaterial).Nombre.ToUpper();
                    var unid = db.tUnidad.Single(x => x.Id == item.tDetalleMaterial.IdUnidad);
                    string cantUnid = unid.Cantidad != 1 ? "- " + unid.Cantidad : "";
                    string disabBTN = item.Estado == 2 ? "" : "disabled";
                    string div = "<div class='d-flex justify-content-center align-items-center'><p id='Especific" + item.Id + "' class='mb-0'>" + item.Especificacion + "  </p><button id='btnEspecif" + item.Id + "' type='button' class='btn btn-outline-primary btn-sm m-1 tooltip-test' onclick='CambiarEspecificacion(" + item.Id + ")' " + disabBTN + " title='Cambiar'><i class='fas fa-sync'></i></button></div>";
                    fila += "<tr>" +
                                   "<td>" + c + "</td>" +
                                   "<td>" + material + "</td>" +
                                   "<td>" + unid.Nombre + " " + cantUnid + "</td>" +
                                   "<td>" + item.CantidadSolicitada + "</td>" +
                                   "<td><input type='number' min='1' class='form-control' id='Cantidad" + item.Id + "' value='" + cantidad + "' onfocusout='UpdateCantidad(" + item.Id + ")'/></td>" +
                                   "<td>" + btn + "</td>" +
                                   "<td>" + div + "</td>" +
                           "</tr>";
                }
            }
            else if (estado == 1 || estado == 4)
            {
                var materiales = db.tDetalleSolicitud.Where(x => x.Idsolicitud == Id && x.Estado >= 1).ToList();
                int c = 0;
                foreach (var item in materiales)
                {
                    c++;
                    string btn = "";
                    if (estado == 4)
                    {
                        string check = item.Estado == 4 ? "checked" : "";
                        btn = "<div class='d-flex justify-content-center pt-2'><input type='checkbox' id='CheckMaterialState" + item.Id + "' name='CheckEntrega[]' onclick='UpdateMaterialState(" + item.Id + ")' class='style-checkbox-blue tooltip-test' title='Pendiente/Entregado' " + check + "></div>";
                    }
                    string material = db.tMaterial.Single(x => x.Id == item.tDetalleMaterial.IdMaterial).Nombre.ToUpper();
                    var unid = db.tUnidad.Single(x => x.Id == item.tDetalleMaterial.IdUnidad);
                    string cantUnid = unid.Cantidad != 1 ? " - " + unid.Cantidad : "";

                    fila += "<tr>" +
                                   "<td>" + c + "</td>" +
                                   "<td>" + material + "</td>" +
                                   "<td>" + unid.Nombre + cantUnid + "</td>" +
                                   "<td>" + item.CantidadSolicitada + "</td>" +
                                   "<td>" + Convert.ToInt32(item.CantidadAprobada) + "</td>" +
                                   "<td>" + btn + "</td>" +
                           "</tr>";
                }
            }
            return Json(fila, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Accept(int Id, int CantidadAprobada, int Estado)
        {
            var o = db.tDetalleSolicitud.Single(x => x.Id == Id);
            o.CantidadAprobada = Estado == 3 ? 0 : CantidadAprobada;
            if (Estado == 2 && CantidadAprobada > 0)
            {
                o.CantidadAprobada = CantidadAprobada;
            }
            else if (Estado == 2 && CantidadAprobada <= 0)
            {
                o.CantidadAprobada = o.CantidadSolicitada;
            }
            o.Estado = Estado;
            db.SaveChanges();
            return Json(o.CantidadAprobada, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangueEspecificacion(int Id)
        {
            var o = db.tDetalleSolicitud.Single(x => x.Id == Id);
            o.Especificacion = o.Especificacion == "ALMACEN" ? "COMPRAR" : "ALMACEN";
            db.SaveChanges();
            return Json(o.Especificacion, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmSend(int Id, string Respuesta)
        {
            Status s = new Status();
            s.msj = "";
            var detalle = db.tDetalleSolicitud.Where(x => x.Idsolicitud == Id && x.Estado == 1).ToList();
            foreach (var item in detalle)
            {
                if (Convert.ToInt32(item.CantidadAprobada) == 0)
                {
                    string material = db.tMaterial.Single(x => x.Id == item.tDetalleMaterial.IdMaterial).Nombre.ToUpper();
                    s.tipo = 4;
                    s.msj = "Debe aprobar la cantidad de " + material + ".";
                    break;
                }
            }
            if (Respuesta.Length>=250)
            {
                s.tipo = 5;
                s.msj = "El texto debe tener como maximo 250 caracteres";
            }
            if (string.IsNullOrEmpty(s.msj))
            {
                var o = db.tSolicitud.Single(x => x.Id == Id);
                o.Estado = 4;
                if (o.FechaRespuesta == null)
                {
                    o.FechaRespuesta = DateTime.Now;
                }
                o.Respuesta = Respuesta;
                db.SaveChanges();
                s.tipo = 1;
                s.msj = "La solicitud fue aprobada con exito.";
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmDeny(int Id, string Respuesta)
        {
            Status s = new Status();
            var o = db.tSolicitud.Single(x => x.Id == Id);
            o.Estado = 1;
            o.Respuesta = Respuesta;
            o.FechaRespuesta = DateTime.Now;
            db.SaveChanges();
            s.tipo = 1;
            s.msj = "La solicitud fue rechazada con exito.";
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateStateSolicitud(int Id)
        {
            Status s = new Status();
            var o = db.tSolicitud.Single(x => x.Id == Id);
            o.Estado = 3;
            db.SaveChanges();
            s.tipo = 1;
            s.msj = "ACTUALIZADO";
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateMaterialState(int Id, int Estado)
        {
            var o = db.tDetalleSolicitud.Single(x => x.Id == Id);
            o.Estado = Estado;
            db.SaveChanges();
            return Json(o.Estado, JsonRequestBehavior.AllowGet);
        }
    }
}