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

            var resultado = database.Multipropietario.Where(mp => mp.Fk_comuna == comuna && mp.Manzana == manzana && mp.Predio == predio).ToList();

            // Filtrar por Año
            if (resultado.Select(r => r.Vigencia_final).Any()) {
                resultado = resultado.Where(r => r.Vigencia_inicial <= ano && r.Vigencia_final >= ano).ToList();
            } else {
                resultado = resultado.Where(r => r.Vigencia_inicial == ano).ToList();
            }

            return View(resultado);
        }
    }
}