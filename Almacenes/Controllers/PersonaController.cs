using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;

namespace Almacenes.Controllers
{
    public class PersonaController : Controller
    {
        // GET: Persona
        public ActionResult Index()
        {
            return View();
        }

        AlmacenesEntities db = new AlmacenesEntities();

        public ActionResult Get()
        {
            var user = db.tUsuario.Single(o => o.Nomusu == User.Identity.Name);
            string btn = "";
            List<tPersona> lista = db.tPersona.Where(x => x.Estado == 1).ToList();
            List<object[]> tabla = new List<object[]>();

            foreach (var item in lista)
            {
                btn = "<div class='dropdown d-flex flex-nowrap text-nowrap justify-content-center'><button class='btn btn-outline-success btn-sm dropdown-toggle m-1' style='font-size:12px' type='button' id='dropdownMenuButton1' data-bs-toggle='dropdown' aria-expanded='false' >Agregar <i class='fas fa-user'></i></button><ul class='dropdown-menu' aria-labelledby='dropdownMenuButton1'></li><li><a class='dropdown-item' href='#' onclick=UserAdd(" + item.Id + ") data-bs-toggle='modal' data-bs-target='#ModalUsuario'>Usuarios</a></li><li><a class='dropdown-item'  data-bs-toggle='modal' data-bs-target='#ModalCargo' onclick='RoleAdd(" + item.Id + ")' href='#'>Cargo</a></li></ul><button type='button' onclick='Edit(" + item.Id + ")' class='btn btn-outline-primary btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalPersona' style='font-size:12px'>Editar <i class='fas fa-pencil-alt'></i></button><button type='button' data-bs-toggle='modal' data-bs-target='#ModalEliminar' onclick='Delete(" + item.Id + ")' class='btn btn-outline-danger btn-sm m-1' style='font-size:12px'>Eliminar <i class='fas fa-trash-alt'></i></button></div>";
                if (db.tUsuario.SingleOrDefault(x => x.Id == item.Id && x.Estado == 1) != null && item.Idcargo != null)
                {
                    btn = "<div class='dropdown d-flex flex-nowrap text-nowrap justify-content-center'><button class='btn btn-outline-success btn-sm dropdown-toggle m-1' style='font-size:12px' type='button' id='dropdownMenuButton1' data-bs-toggle='dropdown' aria-expanded='false' disabled>Agregar <i class='fas fa-user'></i></button><ul class='dropdown-menu' aria-labelledby='dropdownMenuButton1'></li><li><a class='dropdown-item' href='#' onclick=UserAdd(" + item.Id + ") data-bs-toggle='modal' data-bs-target='#ModalUsuario'>Usuarios</a></li><li><a class='dropdown-item'  data-bs-toggle='modal' data-bs-target='#ModalCargo' onclick='RoleAdd(" + item.Id + ")' href='#'>Cargo</a></li></ul><button type='button' onclick='Edit(" + item.Id + ")' class='btn btn-outline-primary btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalPersona' style='font-size:12px'>Editar <i class='fas fa-pencil-alt'></i></button><button type='button' data-bs-toggle='modal' data-bs-target='#ModalEliminar' onclick='Delete(" + item.Id + ")' class='btn btn-outline-danger btn-sm m-1' style='font-size:12px'>Eliminar <i class='fas fa-trash-alt'></i></button></div>";
                }
                else
                {

                    if (db.tUsuario.SingleOrDefault(x => x.Id == item.Id && x.Estado == 1) != null)
                    {
                        btn = "<div class='dropdown d-flex flex-nowrap text-nowrap justify-content-center'><button class='btn btn-outline-success btn-sm dropdown-toggle m-1' style='font-size:12px' type='button' id='dropdownMenuButton1' data-bs-toggle='dropdown' aria-expanded='false'>Agregar <i class='fas fa-user'></i></button><ul class='dropdown-menu' aria-labelledby='dropdownMenuButton1'></li><li><a class='dropdown-item' data-bs-toggle='modal' data-bs-target='#ModalCargo' onclick='RoleAdd(" + item.Id + ")' href='#'>Cargo</a></li></ul><button type='button' onclick='Edit(" + item.Id + ")' class='btn btn-outline-primary btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalPersona' style='font-size:12px'>Editar <i class='fas fa-pencil-alt'></i></button><button type='button' data-bs-toggle='modal' data-bs-target='#ModalEliminar' onclick='Delete(" + item.Id + ")' class='btn btn-outline-danger btn-sm m-1' style='font-size:12px'>Eliminar <i class='fas fa-trash-alt'></i></button></div>";
                    }
                    if (item.Idcargo != null)
                    {
                        btn = "<div class='dropdown d-flex flex-nowrap text-nowrap justify-content-center'><button class='btn btn-outline-success btn-sm dropdown-toggle m-1' style='font-size:12px' type='button' id='dropdownMenuButton1' data-bs-toggle='dropdown' aria-expanded='false'>Agregar <i class='fas fa-user'></i></button><ul class='dropdown-menu' aria-labelledby='dropdownMenuButton1'></li><li><a class='dropdown-item' href='#' onclick=UserAdd(" + item.Id + ") data-bs-toggle='modal' data-bs-target='#ModalUsuario'>Usuarios</a></li></ul><button type='button' onclick='Edit(" + item.Id + ")' class='btn btn-outline-primary btn-sm m-1' data-bs-toggle='modal' data-bs-target='#ModalPersona' style='font-size:12px'>Editar <i class='fas fa-pencil-alt'></i></button><button type='button' data-bs-toggle='modal' data-bs-target='#ModalEliminar' onclick='Delete(" + item.Id + ")' class='btn btn-outline-danger btn-sm m-1' style='font-size:12px'>Eliminar <i class='fas fa-trash-alt'></i></button></div>";
                    }

                }
                string materno = "";
                if (item.Materno != null)
                {
                    materno = item.Materno.ToUpper();
                }
                var departam = db.tDepartamento.SingleOrDefault(x=>x.Id==item.IdDepartamento);
                object[] obj = { (lista.IndexOf(item) + 1).ToString(), item.Documento.Trim(), item.Nombre.ToUpper(), item.Paterno.ToUpper(), materno,departam.Nombre.ToUpper(), btn };
                tabla.Add(obj);
            }
            return Json(tabla, JsonRequestBehavior.AllowGet);
        }

        public object GetId(int Id)
        {
            var o = db.tPersona.Single(x => x.Id == Id);
            string fecha="";
            if (o.FechaNac!=null)
            {
                //fecha = o.FechaNac.Value.ToShortDateString();
                fecha = o.FechaNac?.ToString("yyyy-MM-dd");
            }
            var obj = new { Id = o.Id, Documento = o.Documento, Nombre = o.Nombre, Paterno = o.Paterno, Materno = o.Materno,IdDepartamento=o.IdDepartamento,FechaNac = fecha };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(tPersona o)
        {
            Status s = new Status();
            if (o.FechaNac == null)
            {
                s.tipo = 4;
                s.msj = "La fecha de nacimiento no debe estar vacia";
            }
            if (string.IsNullOrEmpty(o.Paterno))
            {
                s.tipo = 4;
                s.msj = "El apellido paterno no debe estar vacio";
            }
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
            if (o.IdDepartamento==-1)
            {
                s.tipo = 4;
                s.msj = "Debe seleccionar un departamento";
            }
            if (string.IsNullOrEmpty(s.msj))
            {
                try
                {
                    if (o.Id == 0)
                    {
                        o.Estado = 1;
                        db.tPersona.Add(o);
                        s.tipo = 1;
                        s.msj = "Registro Exitoso";
                    }
                    else
                    {
                        var obj = db.tPersona.Single(x => x.Id == o.Id);
                        obj.Id = o.Id;
                        obj.Documento = o.Documento;
                        obj.Nombre = o.Nombre;
                        obj.Paterno = o.Paterno;
                        obj.Materno = o.Materno;
                        obj.IdDepartamento = o.IdDepartamento;
                        obj.FechaNac = o.FechaNac;
                        s.tipo = 2;
                        s.msj = "Modificacion Exitosa";
                    }
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
        public ActionResult Delete(int Id)
        {
            Status s = new Status();
            try
            {
                if (db.tUsuario.SingleOrDefault(x => x.Id == Id && x.Estado == 1)  != null)
                {
                    s.tipo = 4;
                    s.msj = "Debe dar baja al usuarios antes que eliminar a la persona";
                }
                else
                {
                    var obj = db.tPersona.Single(x => x.Id == Id);
                    obj.Estado = -1;
                    db.SaveChanges();
                    s.tipo = 1;
                    s.msj = "Se realizo la eliminación exitosamente";
                }

            }
            catch (Exception e)
            {
                s.tipo = 3;
                s.msj = e.Message;
            }
            return Json(s, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SearchUser(string texto)
        {
            List<object> lista = new List<object>();
            List<tPersona> personas = db.tPersona.SqlQuery("Select top 5 * from tPersona p, tUsuario u where p.Id = u.Id and p.Documento like '%" + texto + "%' and p.Estado = 1").ToList();
            var datos = "";
            foreach (var item in personas)
            {
                if ((db.tSolicitud.Count(x=> x.Idusuario == item.tUsuario.Id)) > 0)
                {
                    string paterno = "";
                    string materno = "";
                    if (item.Paterno != null)
                    {
                        paterno = item.Paterno.ToUpper();
                    }
                    if (item.Materno != null)
                    {
                        materno = item.Materno.ToUpper();
                    }
                    string NombreCompleto = item.Nombre.ToUpper() + " " + paterno + " " + materno;
                    datos += "<a href='#' class='list-group-item list-group-item-action border'  onclick='SelectUser(" + item.Documento + ")'>" + NombreCompleto + " </li>";

                }
            }
            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectUser(string documento)
        {
            object o = new object();
            if (documento == "" || documento == "0")
            {
                o = new { NombreCompleto = "NO DEFINIDO", Documento = "0" };
            }
            else
            {
                var p = db.tPersona.Single(x => x.Documento == documento);
                string paterno = "";
                string materno = "";
                if (p.Paterno != null)
                {
                    paterno = p.Paterno.ToUpper();
                }
                if (p.Materno != null)
                {
                    materno = p.Materno.ToUpper();
                }
                string Nombre = p.Nombre.ToUpper() + " " + paterno + " " + materno;
                string usuario = "SIN CORREO";
                if (p.tUsuario != null)
                {
                    usuario = p.tUsuario.Nomusu.ToString();
                }
                o = new { Nombre = Nombre, Documento = p.Documento, Correo = usuario};

            }
            return Json(o, JsonRequestBehavior.AllowGet);
        }
    }
}