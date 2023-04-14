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
            var multiPropietario = db.Multipropietario.ToList();

            return View(multiPropietario);
        }

        // GET: Multipropietario / Search
        [HttpGet]
        public ActionResult Search(int comuna, int manzana, int predio)
        {
            ViewBag.Comunas = db.Comuna.ToList();

            var resultado = db.Multipropietario.Where(mp => mp.Fk_comuna== comuna && mp.Manzana == manzana && mp.Predio ==predio ).ToList();

            //resultado = resultado.Where(i => i.Creacion.Year == anoMP).ToList();

            return View(resultado);
        }
    }
}