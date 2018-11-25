using Model;
using Repository;
using Sys.Inventarios.Helpers;
using Sys.Inventarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sys.Inventarios.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string email, string password, bool? recordar)
        {
            IRepository repository = new Model.Repository();
            var objUsu = repository.FindEntity<Usuarios>(c => c.CorreoElectronico == email && c.Activo == true);
            int id = 0;
            string strMensaje = "El usuario y/o contraseña son incorrectos.";
            recordar = recordar == null ? false : true;
            if (objUsu != null)
            {
                if (CryproHelper.Confirm(password, objUsu.Password, CryproHelper.Supported_HA.SHA512))
                {
                    id = -1;
                    SessionHelper.AddUserToSession(objUsu.Id.ToString(), (bool)recordar);
                    SessionHelper.ActualizarSession(objUsu);
                    if (objUsu.Rol_Id == 1)
                    {
                        strMensaje = Url.Content("~/Home");
                    }
                }
            }
            return Json(new Response { IsSuccess = true, Message = strMensaje, Id = id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Registrarme()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrarme(string NombreEmpresa, string CorreoElectronico, string Password)
        {
            IRepository repository = new Model.Repository();
            var objUsu = repository.FindEntity<Usuarios>(c => c.CorreoElectronico == CorreoElectronico);
            string strMensaje = "";
            int id = 0;
            if (objUsu != null)
            {
                strMensaje = "El usuario ya existe en nuestra base de datos, intente recuperar su cuenta para cambiar su contraseña.";
            }
            else
            {
                string strPass = CryproHelper.ComputeHash(Password, CryproHelper.Supported_HA.SHA512, null);
                var objEmpresa = repository.Create(new Empresas
                {
                    CorreoElectronico = CorreoElectronico,
                    Direccion = "",
                    Logo = "",
                    Moneda = "GT",
                    NombreEmpresa = NombreEmpresa,
                    Telefono = "",
                    Tipo_Id = 2,
                    ZonaHoraria_Id = null
                });
                if (objEmpresa != null)
                {
                    var objUsuNew = repository.Create(new Usuarios
                    {
                        Activo = true,
                        CorreoElectronico = CorreoElectronico,
                        EmpresaId = objEmpresa.Id,
                        Fecha = DateTime.Now,
                        Nombres = "",
                        Password = strPass,
                        Rol_Id = 1,
                        Telefono = ""
                    });
                    if (objUsuNew != null)
                    {
                        var baseAddress = new Uri(ToolsHelper.UrlOriginal(Request));
                        string Mensaje = "Gracias por inscribirse al sistema de inventarios, puede entrar con el usuario " +
                            "y contraseña registrada. <a href='" + baseAddress + "'>INVENTARIOS</a>";
                        ToolsHelper.SendMail(CorreoElectronico, "Gracias por registrarte a INVENTARIOS", Mensaje);
                        strMensaje = "Te registraste correctamente, ya puedes entrar al sistema.";
                        id = objUsuNew.Id;
                    }
                    else
                    {
                        strMensaje = "Disculpe las molestias, por el momento no podemos conectarnos con el servidor, intentelo nuevamente.";
                    }
                }               
                else
                {
                    strMensaje = "Disculpe las molestias, por el momento no podemos conectarnos con el servidor, intentelo nuevamente.";
                }
            }
            return Json(new Response { IsSuccess = true, Message = strMensaje, Id = id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecuperarCuenta()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecuperarCuenta(string CorreoElectronico)
        {
            IRepository repository = new Model.Repository();
            var objUsu = repository.FindEntity<Usuarios>(c => c.CorreoElectronico == CorreoElectronico);
            int id = 0;
            string strMensaje = "El correo no se encuentra registrado.";
            if (objUsu != null)
            {
                objUsu.Activo = true;
                string strToken = objUsu.Id.ToString() + objUsu.CorreoElectronico;
                string strTknAjax = CryproHelper.ComputeHash(strToken, CryproHelper.Supported_HA.SHA512, null);
                objUsu.Token = Server.UrlEncode(strTknAjax);
                repository.Update(objUsu);
                var baseAddress = ToolsHelper.UrlOriginal(Request) + "/Account/ResetPass/?tkn=" + objUsu.Token;
                string Mensaje = "Para restaurar tu cuenta de INVENTARIOS, entra a la siguiente liga y crea una nueva contraseña. <br/><br/> <a href='" + baseAddress + "'>INVENTARIOS recuperar cuenta</a>";
                ToolsHelper.SendMail(CorreoElectronico, "Recuperar cuenta de INVENTARIOS", Mensaje);
                strMensaje = "Se envío un correo con la información requerida para recuperar su cuenta.";
            }
            return Json(new Response { IsSuccess = true, Message = strMensaje, Id = id }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ResetPass(string tkn)
        {
            if (!string.IsNullOrEmpty(tkn))
            {
                IRepository repository = new Model.Repository();
                tkn = Server.UrlEncode(tkn);
                ViewBag.tkn = tkn;
                var objUsu = repository.FindEntity<Usuarios>(c => c.Token == tkn);
                if (objUsu != null)
                {
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ResetPass(string Password, string tkn)
        {
            IRepository repository = new Model.Repository();
            var objUsu = repository.FindEntity<Usuarios>(c => c.Token == tkn);
            string strMensaje = "";
            int id = 0;
            if (objUsu != null)
            {
                string strPass = CryproHelper.ComputeHash(Password, CryproHelper.Supported_HA.SHA512, null);
                objUsu.Password = strPass;
                objUsu.Token = "";
                repository.Update(objUsu);
                strMensaje = "Se actualizó la contraseña correctamente, ya puede entrar al sistema INVENTARIOS.";
            }
            else
            {
                strMensaje = "El token se encuentra vencido, necesita recuperar nuevamente su cuenta.";
            }
            return Json(new Response { IsSuccess = true, Message = strMensaje, Id = id }, JsonRequestBehavior.AllowGet);
        }

    }
}