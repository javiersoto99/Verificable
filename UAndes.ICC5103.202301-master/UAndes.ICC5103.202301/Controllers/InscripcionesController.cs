using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Controllers
{
    public class InscripcionesController : Controller
    {
        private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();

        // GET: Inscripciones
        public ActionResult Inscripciones()
        {
            return View();
        }

        // GET: NuevaInscripcion
        public ActionResult NuevaInscripcion()
        {
            return View();
        }
    }
}