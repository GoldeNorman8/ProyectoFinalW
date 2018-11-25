using Model;
using Repository;
using Sys.Inventarios.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sys.Inventarios.Controllers
{
    public class ProductosController : Controller
    {
        // GET: Productos
        public ActionResult Index()
        {
            IRepository repository = new Model.Repository();
            var objProduct = repository.FindEntitySet<Productos>(c => true).OrderBy(c => c.Nombre);
            return View(objProduct);
        }
        [HttpPost]
        public ActionResult Add(Producto objProd)
        {
            IRepository repository = new Model.Repository();
            int id = 0;
string            strMensaje = "No se pudo actualizar la información, intentelo más tarde";
            if (objProd.Id > 0)
            {
                id = objProd.Id;
                Productos objUpdateProd = (Productos)objProd;
                repository.Update(objUpdateProd);
                strMensaje = "Se actualizo el producto";
            }
            else
            {
                decimal? uti = objProd.Costo - objProd.PrecioVenta;
                int valorUtilidad = 0;
                int.TryParse(uti.ToString(), out valorUtilidad );
                objProd.Utilidad = valorUtilidad;
                string strGui = Guid.NewGuid().ToString();
                var objResultado = repository.Create(new Productos
                {
                    Codigo = strGui,
                    Costo = objProd.Costo,
                    Descripcion = objProd.Descripcion,
                    Estatus = true,
                    FechaActivo = DateTime.Now,
                    FechaRegistro = DateTime.Now,
                    Marca = objProd.Marca,
                    Modelo = objProd.Modelo,
                    Nombre = objProd.Nombre,
                    PrecioVenta = objProd.PrecioVenta,
                    Stock = objProd.Stock,
                    UnidadMedida = objProd.UnidadMedida,
                    Utilidad = objProd.Utilidad                    

                });
                id = objResultado.Id;
                if(objResultado!=null)
                    strMensaje = "Se agrego el producto";
            }
            return Json(new Response { IsSuccess = true, Message = strMensaje, Id = id }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase[] file)
        {
            string fName = "";
            try
            {
                foreach (var item in file)
                {
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    { 
                        Directory.CreateDirectory(path);
                    }
                    fName = "Prod_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                    fName += Path.GetExtension(item.FileName);
                    item.SaveAs(path + fName);
                }

            }
            catch (Exception exception)
            {
                return Json(new
                {
                    success = false,
                    response = exception.Message
                });
            }

            return Json(new
            {
                success = true,
                response = fName
            });
        }
    }
}