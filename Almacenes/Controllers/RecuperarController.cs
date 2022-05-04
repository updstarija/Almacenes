using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;

namespace Almacenes.Controllers
{
    public class RecuperarController : Controller
    {
        // GET: Recuperar
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Restablecer(int idusuario, string token)
        {
            AlmacenesEntities db = new AlmacenesEntities();
            var existe = db.tReseteoContraseña.SingleOrDefault(x => x.IdUsuario == idusuario && x.Token == token);
            if (existe != null)
            {
                DateTime fecha = DateTime.Now;
                DateTime f = existe.Fecha.AddMinutes(3);
                if (f > fecha)
                {
                    ViewBag.Id = idusuario;
                }
                else
                {
                    ViewBag.Id = -1;
                }
            }
            else
            {
                ViewBag.Id = 0;
            }
            return View();
        }

        public ActionResult ResetPass(int IdUsuario, string NuevaContra, string NuevaContra2)
        {
            Status s = new Status();
            if (string.IsNullOrEmpty(NuevaContra2))
            {
                s.tipo = 4;
                s.msj = "Debe repetir la contraseña nueva";
            }
            if (string.IsNullOrEmpty(NuevaContra))
            {
                s.tipo = 4;
                s.msj = "Debe ingresar una nueva contraseña";
            }
            if (string.IsNullOrEmpty(s.msj))
            {
                AlmacenesEntities db = new AlmacenesEntities();
                if (NuevaContra == NuevaContra2)
                {
                    Encriptar e = new Encriptar();
                    var obj = db.tUsuario.Single(x => x.Id == IdUsuario);
                    obj.Password = e.GetSHA256(NuevaContra);
                    s.tipo = 1;
                    s.msj = "Cambio de contraseña exitoso";
                    db.SaveChanges();
                }
                else
                {
                    s.tipo = 4;
                    s.msj = "Las nuevas contraseñas no coinciden";
                }
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendCode(string correo)
        {
            Status s = new Status();
            try
            {

                if (correo == "")
                {
                    s.tipo = 3;
                    s.msj = "El correo no debe estar vacio";
                }
                else
                {
                    AlmacenesEntities db = new AlmacenesEntities();
                    var existeUsuario = db.tUsuario.SingleOrDefault(x => x.Nomusu == correo && x.Estado == 1);
                    if (existeUsuario != null)
                    {
                        tReseteoContraseña o = new tReseteoContraseña();
                        Encriptar e = new Encriptar();
                        Random r = new Random();
                        o.IdUsuario = existeUsuario.Id;
                        o.Fecha = DateTime.Now;
                        string token = correo + r.Next(1,99999) + o.Fecha;
                        o.Token = e.GetSHA256(token);
                        db.tReseteoContraseña.Add(o);
                        db.SaveChanges();
                        string enlace = "https://localhost:44397/Recuperar/Restablecer?idusuario=" + o.IdUsuario + "&token=" + o.Token +"";
                        var mensaje = new MailMessage();
                        mensaje.Subject = "Recuperación de Contraseñas";
                        mensaje.Body = "<html>" +
                                            "<head>" +
                                                "<title>Recuperación de Contraseñas</title>" +
                                            "</head>" +
                                            "<body>" +
                                                "<p>Hemos recibido una solicitud para la recuperación de contraseñas de su cuenta, si usted no hubiera realizado esta solicitud, no debe ingresar al link que proporcionamos en este mensaje, e informar al administrador de sistemas sobre actividad sospechosa con su información.</p>" +
                                                "<p>Por tanto, si desea recuperar o cambiar su contraseña, ingrese al link que se proporciona a continuación, y modifique su contraseña.</p>" +
                                                "<p><a href=" + enlace + "  > Restablecer contraseña </a></p>" +
                                            "</body>" +
                                        "</html>";
                        mensaje.To.Add(correo);
                        mensaje.IsBodyHtml = true;
                        var smtp = new SmtpClient();
                        smtp.Send(mensaje);
                    }
                    else
                    {
                        s.tipo = 3;
                        s.msj = "El correo es valido";
                    }
                }
            }
            catch (Exception)
            {
                s.tipo = 3;
                s.msj = "Intente mas tarde";
            } 
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}