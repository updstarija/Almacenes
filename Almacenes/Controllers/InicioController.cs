using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Almacenes.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Almacenes.Controllers
{
    public class InicioController : Controller
    {
        // GET: Inicio
        AlmacenesEntities db = new AlmacenesEntities();
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            var user = User.Identity.Name;
            if (string.IsNullOrEmpty(user))
            {
                return View();
            }
            else
            {
                var rol = db.tUsuario.SingleOrDefault(x => x.Estado == 1 && x.Nomusu == User.Identity.Name).IdTtipo;
                if (rol == 1)
                {
                    return RedirectToAction("index", "Home");
                }
                else
                {
                    return RedirectToAction("index", "HomeUsuario");
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index(string username, string password)
        {
            if (ModelState.IsValid)
            {
                Encriptar e = new Encriptar();
                password = e.GetSHA256(password);
                tUsuario user = db.tUsuario.SingleOrDefault(x => x.Nomusu == username & x.Password == password && x.Estado == 1);
                if (user != null)
                {
                    if (user.IdTtipo == 1)
                    {
                        IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                        authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                        ClaimsIdentity identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                        identity.AddClaim(new Claim(ClaimTypes.Name, username));
                        AuthenticationProperties props = new AuthenticationProperties();
                        props.IsPersistent = true;
                        authenticationManager.SignIn(props, identity);
                        return RedirectToAction("index", "Home");
                    }
                    else
                    {
                        IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                        authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                        ClaimsIdentity identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                        identity.AddClaim(new Claim(ClaimTypes.Name, username));
                        AuthenticationProperties props = new AuthenticationProperties();
                        props.IsPersistent = true;
                        authenticationManager.SignIn(props, identity);
                        return RedirectToAction("index", "HomeUsuario");
                    }
                }
                else
                {
                    ViewBag.error = 0;
                }
            }
            return View();
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Logout()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Inicio");
        }

        public ActionResult Principal()
        {
            return View();
        }
    }
}