using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;
using Microsoft.Reporting.WebForms;

namespace Almacenes.Controllers
{
    public class ReporteController : Controller
    {
        // GET: Reporte
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Statistics()
        {
            AlmacenesEntities db = new AlmacenesEntities();
            int aprobadas = db.tSolicitud.Count(x => x.Estado == 4);
            int pendientes = db.tSolicitud.Count(x => x.Estado == 3);
            int rechazadas = db.tSolicitud.Count(x => x.Estado == 1);
            int total = aprobadas + pendientes + rechazadas;
            var obj = new { Aprobadas = aprobadas, Pendientes = pendientes, Rechazadas = rechazadas, Total = total };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportByType(int IdTipo)
        {
            AlmacenesEntities db = new AlmacenesEntities();
            string Tipo = "PDF";
            var reporte = "ReportSolicitudTipo.rdlc";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), reporte);
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                ViewBag.dir = path;
                return View("Index");
            }

            List<object> lista = new List<object>();
            string tipoSoli = db.tTipoSolicitud.Single(x => x.Id == IdTipo).Nombre.ToUpper();
            var solicitudes = db.tSolicitud.OrderByDescending(x => x.Fecha).Where(x => x.Idtiposolicitud == IdTipo && (x.Estado == 1 || x.Estado == 3 || x.Estado == 4)).ToList();
            if (solicitudes.Count > 0)
            {
                foreach (var item in solicitudes)
                {
                    string fecha = item.Fecha.Value.ToShortDateString();
                    var p = db.tPersona.Single(x => x.tUsuario.Id == item.Idusuario);
                    string usuario = p.Nombre.ToUpper() + " " + p.Paterno.ToUpper() + " " + (p.Materno!=null?p.Materno.ToUpper():"");
                    int cantidad = db.tDetalleSolicitud.Count(x => x.Idsolicitud == item.Id);
                    string descripcion = item.Descripcion.ToUpper();
                    string estado = "";
                    switch (item.Estado)
                    {
                        case 1:
                            estado = "RECHAZADA";
                            break;

                        case 3:
                            estado = "PENDIENTE";
                            break;

                        case 4:
                            estado = "APROBADA";
                            break;
                    }
                    object o = new
                    {
                        fecha,
                        usuario,
                        tipoSoli,
                        descripcion,
                        cantidad,
                        estado,
                    };
                    lista.Add(o);
                }
            }
            else
            {
                object o = new
                {
                    tipoSoli
                };
                lista.Add(o);
            }
            ReportDataSource rd = new ReportDataSource("DataSet1", lista);
            lr.DataSources.Add(rd);
            string reportType = Tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + Tipo + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.1in</MarginTop>" +
            "  <MarginLeft>0.1in</MarginLeft>" +
            "  <MarginRight>0.1in</MarginRight>" +
            "  <MarginBottom>0.1in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);

        }

        public ActionResult ReportByDate(DateTime Fecha1, DateTime Fecha2)
        {
            AlmacenesEntities db = new AlmacenesEntities();
            string Tipo = "PDF";
            var reporte = "ReporteSolicitudFecha.rdlc";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), reporte);
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                ViewBag.dir = path;
                return View("Index");
            }

            List<object> lista = new List<object>();
            string fechaIni = Fecha1.ToString("D").ToUpper();
            string fechaFin = Fecha2.ToString("D").ToUpper();
            DateTime Fecha3 = Fecha2.AddDays(1);
            var solicitudes = db.tSolicitud.OrderByDescending(x => x.Fecha).Where(x => x.Fecha > Fecha1 && x.Fecha < Fecha3 && (x.Estado == 1 || x.Estado == 3 || x.Estado == 4)).ToList();
            if (solicitudes.Count > 0)
            {
                foreach (var item in solicitudes)
                {
                    string fecha = item.Fecha.Value.ToShortDateString();
                    string tipoSoli = db.tTipoSolicitud.Single(x => x.Id == item.Idtiposolicitud).Nombre.ToUpper();
                    var p = db.tPersona.Single(x => x.tUsuario.Id == item.Idusuario);
                    string usuario = p.Nombre.ToUpper() + " " + p.Paterno.ToUpper() + " " + (p.Materno!=null?p.Materno.ToUpper():"");
                    int cantidad = db.tDetalleSolicitud.Count(x => x.Idsolicitud == item.Id);
                    string descripcion = item.Descripcion.ToUpper();
                    string estado = "";
                    switch (item.Estado)
                    {
                        case 1:
                            estado = "RECHAZADA";
                            break;

                        case 3:
                            estado = "PENDIENTE";
                            break;

                        case 4:
                            estado = "APROBADA";
                            break;
                    }
                    object o = new
                    {

                        fechaIni,
                        fechaFin,
                        fecha,
                        usuario,
                        tipoSoli,
                        descripcion,
                        cantidad,
                        estado,
                    };
                    lista.Add(o);
                }
            }
            else
            {
                object o = new
                {
                    fechaIni,
                    fechaFin
                };
                lista.Add(o);
            }
            ReportDataSource rd = new ReportDataSource("DataSet1", lista);
            lr.DataSources.Add(rd);
            string reportType = Tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + Tipo + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.1in</MarginTop>" +
            "  <MarginLeft>0.1in</MarginLeft>" +
            "  <MarginRight>0.1in</MarginRight>" +
            "  <MarginBottom>0.1in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);

        }

        public ActionResult ReportByUser(string documento)
        {
            AlmacenesEntities db = new AlmacenesEntities();
            string Tipo = "PDF";
            var reporte = "ReportSolicitudUsuario.rdlc";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), reporte);
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                ViewBag.dir = path;
                return View("Index");
            }

            List<object> lista = new List<object>();
            int idUsuario = db.tPersona.Single(x => x.Documento == documento).Id;
            var p = db.tPersona.Single(x => x.tUsuario.Id == idUsuario);
            string usuario = p.Nombre.ToUpper() + " " + p.Paterno.ToUpper() + " " + (p.Materno!=null?p.Materno.ToUpper():"");
            var solicitudes = db.tSolicitud.OrderByDescending(x => x.Fecha).Where(x => x.Idusuario == idUsuario && (x.Estado == 1 || x.Estado == 3 || x.Estado == 4)).ToList();
            if (solicitudes.Count > 0)
            {
                foreach (var item in solicitudes)
                {
                    string fecha = item.Fecha.Value.ToShortDateString();
                    string tipoSoli = db.tTipoSolicitud.Single(x => x.Id == item.Idtiposolicitud).Nombre.ToUpper();
                    int cantidad = db.tDetalleSolicitud.Count(x => x.Idsolicitud == item.Id);
                    string descripcion = item.Descripcion.ToUpper();
                    string estado = "";
                    switch (item.Estado)
                    {
                        case 1:
                            estado = "RECHAZADA";
                            break;

                        case 3:
                            estado = "PENDIENTE";
                            break;

                        case 4:
                            estado = "APROBADA";
                            break;
                    }
                    object o = new
                    {
                        fecha,
                        usuario,
                        tipoSoli,
                        descripcion,
                        cantidad,
                        estado,
                    };
                    lista.Add(o);
                }
            }
            else
            {
                object o = new
                {
                    usuario
                };
                lista.Add(o);
            }
            ReportDataSource rd = new ReportDataSource("DataSet1", lista);
            lr.DataSources.Add(rd);
            string reportType = Tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + Tipo + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.1in</MarginTop>" +
            "  <MarginLeft>0.1in</MarginLeft>" +
            "  <MarginRight>0.1in</MarginRight>" +
            "  <MarginBottom>0.1in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);

        }

        public ActionResult ReportListMaterials(string Especificacion, DateTime Fecha1, DateTime Fecha2)
        {
            AlmacenesEntities db = new AlmacenesEntities();
            string Tipo = "PDF";
            var reporte = "ReporteListaMateriales.rdlc";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), reporte);
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                ViewBag.dir = path;
                return View("Index");
            }

            List<object> lista = new List<object>();
            string fechaIni = Fecha1.ToString("D").ToUpper();
            string fechaFin = Fecha2.ToString("D").ToUpper();
            DateTime Fecha3 = Fecha2.AddDays(1);
            var listMaterials = db.tSolicitud.OrderByDescending(x => x.FechaRespuesta).Where(x => x.FechaRespuesta > Fecha1 && x.FechaRespuesta < Fecha3 && x.Estado == 4).ToList();
            //var listMaterials = db.tSolicitud.Where(x=>x.Estado==4).ToList();
            //string tipoSoli = db.tTipoSolicitud.Single(x => x.Id == IdTipo).Nombre.ToUpper();
            //var solicitudes = db.tSolicitud.OrderByDescending(x => x.Fecha).Where(x => x.Idtiposolicitud == IdTipo && (x.Estado == 1 || x.Estado == 3 || x.Estado == 4)).ToList();
            if (listMaterials.Count > 0)
            {
                List<tempMaterial> totalListMaterial = (
                     from s in db.tSolicitud
                     join ds in db.tDetalleSolicitud on s.Id equals ds.Idsolicitud
                     join dm in db.tDetalleMaterial on ds.IdDetalleMaterial equals dm.Id
                     join m in db.tMaterial on dm.IdMaterial equals m.Id
                     join unid in db.tUnidad on dm.IdUnidad equals unid.Id
                     where s.Estado == 4 && (s.FechaRespuesta > Fecha1 && s.FechaRespuesta < Fecha3) && ds.Estado == 2 && ds.Especificacion == Especificacion
                     let IdUnidad = unid.Id
                     let Unidad = unid.Nombre
                     let CantidadUnidad = unid.Cantidad
                     group ds by new { m.Id, m.Nombre, IdUnidad, Unidad, CantidadUnidad } into dt
                     select new tempMaterial
                     {
                         IdMaterial = dt.Key.Id,
                         Nombre = dt.Key.Nombre,
                         IdUnidad = dt.Key.IdUnidad,
                         NombreUnidad = dt.Key.Unidad,
                         CantidadUnidad = (int)dt.Key.CantidadUnidad,
                         Total = (int)dt.Sum(x => x.CantidadAprobada),
                     }).OrderBy(x => x.Nombre).ThenBy(d => d.CantidadUnidad).ToList();
                var temp = totalListMaterial;
                foreach (var item in temp)
                {
                    var listUnidades = db.tDetalleMaterial.OrderByDescending(x => x.tUnidad.Cantidad).Where(x => x.IdMaterial == item.IdMaterial && x.Estado == 1).ToList();
                    tempMaterial tm = totalListMaterial.SingleOrDefault(x => x.IdMaterial == item.IdMaterial && x.IdUnidad == item.IdUnidad && x.Total == item.Total);
                    tm.ListUnidades = listUnidades;
                    if (item.NombreUnidad.ToUpper() == "UNIDAD")
                    {
                        int verific = -1;
                        var c = 0;
                        while (verific == -1)
                        {
                            if (item.Total >= tm.ListUnidades[c].tUnidad.Cantidad)
                            {
                                var rest = (int)item.Total % (int)tm.ListUnidades[c].tUnidad.Cantidad;
                                var tot = ((int)item.Total - rest) / (int)tm.ListUnidades[c].tUnidad.Cantidad;
                                tm.NombreUnidadPadre = tm.ListUnidades[c].tUnidad.Nombre;
                                tm.CantidadUnidadPadre = (int)tm.ListUnidades[c].tUnidad.Cantidad;
                                tm.SumConvertUnit = rest == 0 ? tot.ToString() : tot + " & " + rest +" unid.";
                                verific = 1;
                            }
                            c++;
                            if (c > tm.ListUnidades.Count())
                            {
                                verific = 0;
                            }
                        }
                    }
                }

                foreach (var item in listMaterials)
                {
                    //var materials = db.tDetalleSolicitud.Where(x => x.Estado == 2).ToList();
                    foreach (var mat in item.tDetalleSolicitud)
                    {
                        if (mat.Estado == 2 && mat.Especificacion == Especificacion)
                        {
                            var p = db.tPersona.Single(x => x.tUsuario.Id == item.Idusuario);
                            string usuario = p.Nombre.ToUpper() + " " + p.Paterno.ToUpper() + " " + (p.Materno == null ? "" : p.Materno.ToUpper());
                            var material = mat.tDetalleMaterial.tMaterial.Nombre.ToLower();
                            material = material.Substring(0, 1).ToUpper() + material.Substring(1, (material.Length - 1));
                            var cantidad = 0;
                            var listDetMat = db.tDetalleMaterial.Where(x => x.IdMaterial == mat.tDetalleMaterial.IdMaterial && x.Estado == 1);

                            //VERIFICAR LA CONVERSION MAS OPTIMA QUE SE DEBE DE HACER POR UNIDAD
                            string cantUnidPadre = "";
                            string especificacionPadre = "";
                            int CantidadConversion = -1;
                            cantidad = (int)mat.CantidadAprobada;
                            var cantUnidad = "-1";

                            if (mat.tDetalleMaterial.tUnidad.Nombre.ToUpper() == "UNIDAD")
                            {
                                var tSumMaterial = totalListMaterial.SingleOrDefault(x => x.IdMaterial == mat.tDetalleMaterial.tMaterial.Id && x.CantidadUnidad == mat.tDetalleMaterial.tUnidad.Cantidad);
                                if (tSumMaterial.NombreUnidadPadre.ToUpper() != "UNIDAD")
                                {
                                    cantUnidPadre = tSumMaterial.CantidadUnidadPadre != 1 ? " - " + tSumMaterial.CantidadUnidadPadre : "";
                                    especificacionPadre = tSumMaterial.NombreUnidadPadre.ToUpper() + cantUnidPadre;
                                    CantidadConversion = tSumMaterial.CantidadUnidadPadre;
                                    cantUnidad = tSumMaterial.SumConvertUnit;
                                }
                            }

                            string fechaAprob = item.FechaRespuesta != null ? item.FechaRespuesta.Value.ToShortDateString() : "";
                            string departamento = p.tDepartamento.Nombre;
                            string especificacion = mat.Especificacion;
                            string cantUnid = mat.tDetalleMaterial.tUnidad.Cantidad != 1 ? " - " + mat.tDetalleMaterial.tUnidad.Cantidad : "";
                            string unidad = mat.tDetalleMaterial.tUnidad.Nombre.ToUpper() + cantUnid;
                            string estado = mat.Estado.ToString();
                            int idSolicitud = item.Id;
                            switch (item.Estado)
                            {
                                case 2:
                                    estado = "PENDIENTE";
                                    break;
                                case 4:
                                    estado = "ENTREGADO";
                                    break;
                            }
                            object o = new
                            {
                                usuario,
                                material,
                                cantidad,
                                fechaAprob,
                                departamento,
                                especificacion,
                                estado,
                                unidad,
                                cantUnidad,
                                CantidadConversion,
                                fechaIni,
                                fechaFin,
                                especificacionPadre,
                                idSolicitud
                            };
                            lista.Add(o);
                        }
                    }
                }
            }
            else
            {
                var msj = "";
                object o = new
                {
                    msj
                };
                lista.Add(o);
            }
            ReportDataSource rd = new ReportDataSource("DataSet1", lista);
            lr.DataSources.Add(rd);
            string reportType = Tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + Tipo + "</OutputFormat>" +
            "  <PageWidth>11in</PageWidth>" +
            "  <PageHeight>8.5in</PageHeight>" +
            "  <MarginTop>0.1in</MarginTop>" +
            "  <MarginLeft>0.1in</MarginLeft>" +
            "  <MarginRight>0.1in</MarginRight>" +
            "  <MarginBottom>0.1in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);

        }

    }
}