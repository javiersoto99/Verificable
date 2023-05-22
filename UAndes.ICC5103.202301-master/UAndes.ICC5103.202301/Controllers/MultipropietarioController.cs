using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Controllers {
    public class MultipropietarioController: Controller {
        private InscripcionesBrDbEntities database = new InscripcionesBrDbEntities();

        public ActionResult Index() {
            ViewBag.Comunas = database.Comuna.ToList();
            var multipropietario = database.Multipropietario.OrderBy(mp => mp.Fecha_inscripcion.Year).ToList();

            return View(multipropietario);
        }

        [HttpGet]
        public ActionResult Search(int ano, int comuna, int manzana, int predio) {
            ViewBag.Comunas = database.Comuna.ToList();

            var resultadoFinal = new List<Multipropietario>();

            var resultado = database.Multipropietario.Where(mp => mp.Fk_comuna == comuna && mp.Manzana == manzana && mp.Predio == predio).ToList();

            // Filtrar por Año
            resultado = resultado.Where(r => r.Vigencia_inicial <= ano && (r.Vigencia_final == null || r.Vigencia_final >= ano)).OrderBy(r => r.Numero_inscripcion).ToList();

            return View(resultado);
        }
    }
}