using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;

namespace Almacenes.Controllers
{
    public class HomeUsuarioController : Controller
    {
        // GET: HomeUsuario

        AlmacenesEntities db = new AlmacenesEntities();
        public ActionResult Index()
        {
            return View();
        }

        public string loadCard(List<tSolicitud> solicitudes, int Estado)
        {
            string contenedor = "";
            string cabeceraCard = "";
            string cuerpoCard = "";

            switch (Estado)
            {
                case 1:
                    cabeceraCard = "<div class='card-header bg-danger text-white'>Rechazada <i class='fas fa-thumbs-down text-white'></i></div>";
                    //cuerpoCard = item.Respuesta.ToString();
                    break;
                case 3:
                    cabeceraCard = "<div class='card-header bg-primary text-white'>Pendiente <i class='fas fa-hourglass-start text-white'></i></div>";
                    //cuerpoCard = item.Descripcion.ToString();
                    break;
                case 4:
                    cabeceraCard = "<div class='card-header bg-success text-white'>Aprobada <i class='fas fa-thumbs-up text-white'></i></div>";
                    //cuerpoCard = item.Respuesta.ToString();
                    break;
            }
            contenedor += "<div class='col-sm-6 col-md-4'>" +
                                    "<div class='card shadow'>"
                                        + cabeceraCard +
                                        "<div class='card-body overflow-auto' style='height:195px'>";
            //contenedor += "<div class='col-sm-4'>" +
            //                       "<div class='card shadow'>"
            //                           + cabeceraCard +
            //                           "<div class='card-body' style='height:180px'>" +
            //                               "<p>" + cuerpoCard + "</p>" +
            //                           "</div>" +
            //                           "<div class='card-footer text-muted'>" +
            //                           //"<a href= '#' class='card-link'>Ver mas</a>" +
            //                           "</div>" +
            //                       "</div>" +
            //                 "</div>";
            foreach (var item in solicitudes)
            {
                cuerpoCard += item.Estado != 3 ? "<p>" + item.FechaRespuesta + " - " + item.tTipoSolicitud.Nombre + "</p>" : "<p>" + item.Fecha + " - " + item.tTipoSolicitud.Nombre + "</p>";
            }
            contenedor += cuerpoCard;
            contenedor += "</div>" +
                                        "<div class='card-footer text-muted'>" +
                                        //"<a href= '#' class='card-link'>Ver mas</a>" +
                                        "</div>" +
                                    "</div>" +
                              "</div>";
            return contenedor;
        }

        public ActionResult Notifications()
        {
            int idUsuario = Convert.ToInt32(db.tUsuario.Single(x => x.Nomusu == User.Identity.Name).Id);
            var Listsolicitudes = db.tSolicitud.Where(x => x.Idusuario == idUsuario);
            var listPendientes = Listsolicitudes.OrderByDescending(x => x.FechaRespuesta).Where(x => x.Estado == 3).Take(4).ToList();
            var listRech = Listsolicitudes.OrderByDescending(x => x.FechaRespuesta).Where(x => x.Estado == 1).Take(4).ToList();
            var listAprob = Listsolicitudes.OrderByDescending(x => x.FechaRespuesta).Where(x => x.Estado == 4).Take(4).ToList();
            //var solicitudes = db.tSolicitud.SqlQuery("select top 3 * from tSolicitud where Idusuario = " + idUsuario + " and  Estado = 1 or Estado = 3 or Estado = 4 order by Fecha desc").ToList();
            string contenedor = "";
            contenedor += loadCard(listPendientes, 3);
            contenedor += loadCard(listRech, 1);
            contenedor += loadCard(listAprob, 4);
            return Json(contenedor, JsonRequestBehavior.AllowGet);
        }
    }
}