using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UAndes.ICC5103._202301.Controllers
{
    public class InscripcionesController : Controller
    {
        // GET: Inscripciones
        public ActionResult Inscripciones()
        {
            return View();
        }

        public ActionResult NuevaInscripcion()
        {
            return View();
        }
    }
}