using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;

namespace Almacenes.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        AlmacenesEntities db = new AlmacenesEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Top5()
        {
            var solicitudes = db.tSolicitud.SqlQuery("select top 5 * from tSolicitud where Estado = 1 or Estado = 3 or Estado = 4 order by Fecha desc").ToList();
            string lista = "";
            foreach (var item in solicitudes)
            {
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
                string solicitante = p.Nombre.ToUpper() + " " + paterno + " " + materno;
                string icono = "";
                switch (item.Estado)
                {
                    case 1:
                        icono = "<i class='fas fa-thumbs-down text-danger'></i>";
                        break;
                    case 3:
                        icono = "<i class='fas fa-hourglass-start text-primary'></i>";
                        break;
                    case 4:
                        icono = "<i class='fas fa-thumbs-up text-success'></i>";
                        break;
                }
                lista += "<li class='list-group-item d-flex justify-content-between align-items-center'>" + solicitante  + icono +"</li>";
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Statistics()
        {
            string lista = "";
            int aprobadas = db.tSolicitud.Count(x => x.Estado == 4);
            int pendientes = db.tSolicitud.Count(x => x.Estado == 3);
            int rechazadas = db.tSolicitud.Count(x => x.Estado == 1);
            int total = aprobadas + pendientes + rechazadas;
            if (total == 0)
            {
                lista += "<label>Aprobadas</label>" +
                         "<div class='progress mb-4'>" +
                            "<div class='progress-bar bg-success' role='progressbar' style='width: 0%' aria-valuenow='0' aria-valuemin='0' aria-valuemax='100'>0%</div>" +
                         "</div>";
                lista += "<label>Pendientes</label>" +
                         "<div class='progress mb-4'>" +
                            "<div class='progress-bar bg-primary' role='progressbar' style='width: 0%' aria-valuenow='0' aria-valuemin='0' aria-valuemax='100'>0%</div>" +
                         "</div>";
                lista += "<label>Rechazadas</label>" +
                          "<div class='progress mb-4'>" +
                            "<div class='progress-bar bg-danger' role='progressbar' style='width: 0%' aria-valuenow='0' aria-valuemin='0' aria-valuemax='100'>0%</div>" +
                          "</div>";
            }
            else
            {
                double porcentajeAprobadas = (aprobadas * 100) / total;
                double porcentajePendientes = (pendientes * 100) / total;
                double porcentajeRechazadas = (rechazadas * 100) / total;

                lista += "<label>Aprobadas</label>" +
                         "<div class='progress mb-4'>" +
                            "<div class='progress-bar bg-success' role='progressbar' style='width: " + porcentajeAprobadas + "%' aria-valuenow='" + porcentajeAprobadas + "' aria-valuemin='0' aria-valuemax='100'>" + porcentajeAprobadas + "%</div>" +
                         "</div>";
                lista += "<label>Pendientes</label>" +
                         "<div class='progress mb-4'>" +
                            "<div class='progress-bar bg-primary' role='progressbar' style='width: " + porcentajePendientes + "%' aria-valuenow='" + porcentajePendientes + "' aria-valuemin='0' aria-valuemax='100'>" + porcentajePendientes + " %</div>" +
                         "</div>";
                lista += "<label>Rechazadas</label>" +
                          "<div class='progress mb-4'>" +
                            "<div class='progress-bar bg-danger' role='progressbar' style='width: " + porcentajeRechazadas + "%' aria-valuenow='" + porcentajeRechazadas + "' aria-valuemin='0' aria-valuemax='100'>" + porcentajeRechazadas + "%</div>" +
                          "</div>";
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CantAprobadas()
        {
            int cantidad = db.tSolicitud.Count(x => x.Estado == 4);
            return Json(cantidad, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CantPendientes()
        {
            int cantidad = db.tSolicitud.Count(x => x.Estado == 3);
            return Json(cantidad, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CantRechazadas()
        {
            int cantidad = db.tSolicitud.Count(x => x.Estado == 1);
            return Json(cantidad, JsonRequestBehavior.AllowGet);
        }
    }
}