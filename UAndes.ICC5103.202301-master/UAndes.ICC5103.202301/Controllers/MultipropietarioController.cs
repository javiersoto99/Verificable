using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Controllers
{
    public class MultipropietarioController : Controller
    {
        private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();
        public ActionResult Index()
        {
            var inscripcion = db.Inscripcion.ToList();
            ViewBag.Comunas = db.Comuna.ToList();
            return View(inscripcion);
        }
        // GET: Multipropietario
        [HttpGet]
        public ActionResult Search(int comuna, int manzana, int predio)
        {
            var idRolBusqueda = db.Rol.Where(r => r.Fk_comuna== comuna && r.Manzana == manzana && r.Predio == predio).Select(r => r.Id).FirstOrDefault();

            var resultado = db.Inscripcion.Where(i => i.Fk_rol == idRolBusqueda).ToList();

            //resultado = resultado.Where(i => i.Creacion.Year == anoMP).ToList();

            return View(resultado);
        }
    }
}