using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;

namespace Almacenes.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario

        AlmacenesEntities db = new AlmacenesEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Perfil()
        {
            return View();
        }

        public ActionResult ViewProfile()
        {
            var p = db.tPersona.Single(x => x.tUsuario.Nomusu == User.Identity.Name);
            string nombre = p.Nombre.ToUpper() + " " + p.Paterno.ToUpper();
            string correo = p.tUsuario.Nomusu.ToString();
            string cargo = "CARGO NO DEFINIDO";
            if (p.tCargo != null)
            {
                cargo = db.tCargo.Single(x => x.Id == p.Idcargo).Nombre.ToUpper();
            }

            string datos = "<h5 class='card-title text-primary'>" + nombre + "</h5>" +
                        "<img src='https://us.123rf.com/450wm/thesomeday123/thesomeday1231712/thesomeday123171200009/91087331-icono-de-perfil-de-avatar-predeterminado-para-hombre-marcador-de-posici%C3%B3n-de-foto-gris-vector-de-ilu.jpg?ver=6' class='img-fluid' style='height:230px; width:230px' />" +
                        "<p class='card-text'>" + cargo + "</p>" +
                        "<p class='card-text'><i class='far fa-envelope'></i> " + correo + "</p>";

            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewInformation()
        {
            var p = db.tPersona.Single(x => x.tUsuario.Nomusu == User.Identity.Name);
            var obj = new { Nombre = p.Nombre, Paterno = p.Paterno, Materno = p.Materno, Documento = p.Documento, Correo = p.tUsuario.Nomusu };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateInformation(tPersona o)
        {
            Status s = new Status();
            if (string.IsNullOrEmpty(o.Nombre))
            {
                s.tipo = 4;
                s.msj = "El nombre no debe estar vacio";
            }
            if (string.IsNullOrEmpty(o.Documento))
            {
                s.tipo = 4;
                s.msj = "El documento no debe estar vacio";
            }
            if (string.IsNullOrEmpty(s.msj))
            {
                try
                {
                    int IdUsu = db.tUsuario.Single(x => x.Nomusu == User.Identity.Name).Id;
                    var obj = db.tPersona.Single(x => x.Id == IdUsu);
                    obj.Documento = o.Documento;
                    obj.Nombre = o.Nombre;
                    obj.Paterno = o.Paterno;
                    obj.Materno = o.Materno;
                    s.tipo = 1;
                    s.msj = "Modificacion Exitosa";
                    db.SaveChanges();
                }
                catch
                {
                    s.tipo = 3;
                    s.msj = "Error al guardar";
                }
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdatePassword(string ContraseñaAnte, string NuevaContraseña, string NuevaContraseña2)
        {
            Status s = new Status();
            if (string.IsNullOrEmpty(NuevaContraseña2))
            {
                s.tipo = 4;
                s.msj = "Debe repetir la contraseña nueva";
            }
            if (string.IsNullOrEmpty(NuevaContraseña))
            {
                s.tipo = 4;
                s.msj = "Debe ingresar una nueva contraseña";
            }
            if (string.IsNullOrEmpty(ContraseñaAnte))
            {
                s.tipo = 4;
                s.msj = "Debe ingresar la contraseña actual";
            }
            if (string.IsNullOrEmpty(s.msj))
            {

                int IdUsu = db.tUsuario.Single(x => x.Nomusu == User.Identity.Name).Id;
                var contraseñaIncorrecta = db.tUsuario.Count(x => x.Id == IdUsu && x.Password == ContraseñaAnte);
                if (contraseñaIncorrecta > 0)
                {
                    if (NuevaContraseña == NuevaContraseña2)
                    {
                        Encriptar e = new Encriptar();
                        var obj = db.tUsuario.Single(x => x.Id == IdUsu);
                        obj.Password = e.GetSHA256(NuevaContraseña);
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
                else
                {
                    s.tipo = 4;
                    s.msj = "La contraseña actual no coincide";
                }
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get()
        { 
            string btn = "";
            var user = db.tUsuario.Single(o => o.Nomusu == User.Identity.Name);
            List<tUsuario> lista = db.tUsuario.Where(x => x.Estado == 1 && x.Id!=user.Id).ToList();
            List<object[]> tabla = new List<object[]>();
            foreach (var item in lista)
            {
                btn = "<div class='d-flex flex-nowrap text-nowrap justify-content-center'><button type='button' onclick='Edit(" + item.Id + ")' class='btn btn-outline-primary btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalUsuario' style='font-size:12px'>Editar <i class='fas fa-pencil-alt'></i></button><button type='button' data-bs-toggle='modal' data-bs-target='#ModalEliminar' onclick='Delete(" + item.Id + ")' class='btn btn-outline-danger btn-sm m-1' style='font-size:12px'>Eliminar <i class='fas fa-trash-alt'></i></button></div>";
                object[] obj = { (lista.IndexOf(item) + 1).ToString(), item.tPersona.Nombre.ToUpper()+" "+ item.tPersona.Paterno.ToUpper(), item.Nomusu, btn };
                tabla.Add(obj); 
            }
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }

        public object GetId(int Id)
        {
            var o = db.tUsuario.Single(x => x.Id == Id);
            var obj = new { Id = o.Id , Nomusu = o.Nomusu, Password = o.Password, Idtipo = o.IdTtipo };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(tUsuario o)
        {
            Status s = new Status();
            if (o.IdTtipo <= 0)
            {
                s.tipo = 4;
                s.msj = "Debe seleccionar un tipo de usuario";
            }
            if (string.IsNullOrEmpty(o.Nomusu))
            {
                s.tipo = 4;
                s.msj = "El nombre de usuario no puede estar vacío";
            }
            else
            {
                if (db.tUsuario.SingleOrDefault(x => x.Id == o.Id) == null)
                {
                    var usu = db.tUsuario.SingleOrDefault(x => x.Nomusu.Equals(o.Nomusu));
                    if (usu != null)
                    {
                        s.tipo = 4;
                        s.msj = "El nombre de usuario ya existe ingrese otro distinto";
                    }
                    else
                    {
                        var foo = new EmailAddressAttribute();
                        bool verif;
                        verif = foo.IsValid("" + o.Nomusu);
                        if (verif == false)
                        {
                            s.tipo = 4;
                            s.msj = "Por favor ingrese un correo valido";
                        }
                    }
                }
                else
                {
                    var foo = new EmailAddressAttribute();
                    bool verif;
                    verif = foo.IsValid("" + o.Nomusu);
                    if (verif == false)
                    {
                        s.tipo = 4;
                        s.msj = "Por favor ingrese un correo valido";
                    }
                }
            }
            if (string.IsNullOrEmpty(o.Password))
            {
                s.tipo = 4;
                s.msj = "El password no puede estar vacío";
            }

            if (string.IsNullOrEmpty(s.msj))
            {
                try
                {
                    var obj = db.tUsuario.SingleOrDefault(x => x.Id == o.Id);
                    if (obj == null)
                    {
                        Encriptar e = new Encriptar();
                        o.Estado = 1;
                        o.Password = e.GetSHA256(o.Password);
                        db.tUsuario.Add(o);
                        s.tipo = 1;
                        s.msj = "Registro Exitoso";
                    }
                    else
                    {
                        if (o.Password!= "NULL")
                        {
                            Encriptar e = new Encriptar();
                            obj.Password = e.GetSHA256(o.Password);
                        }
                        obj.Nomusu = o.Nomusu;
                        obj.IdTtipo = o.IdTtipo;
                        obj.Estado = 1;
                        s.tipo = 2;
                        s.msj = "Modificacion Exitosa";
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    s.tipo = 3;
                    s.msj = e.Message;
                }
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int Id)
        {
            Status s = new Status();
            try
            {
                var obj = db.tUsuario.Single(x => x.Id == Id);
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