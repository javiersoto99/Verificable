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

        // GET: Multipropietario
        public ActionResult Index()
        {
            ViewBag.Comunas = db.Comuna.ToList();
            var multiPropietario = db.Multipropietario.OrderBy(mp => mp.Fecha_inscripcion.Year).ToList();

            return View(multiPropietario);
        }

        // GET: Multipropietario / Search
        [HttpGet]
        public ActionResult Search(int ano, int comuna, int manzana, int predio)
        {
            ViewBag.Comunas = db.Comuna.ToList();

            var resultado = db.Multipropietario.Where(mp => mp.Fk_comuna == comuna && mp.Manzana == manzana && mp.Predio == predio).ToList();
            //Filtrar por Año
            resultado = resultado.Where(r => r.Vigencia_inicial == ano).ToList();

            return View(resultado);
        }
    }
}