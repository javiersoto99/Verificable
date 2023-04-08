using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Controllers
{
    public class InscripcionsController : Controller
    {
        private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();

        // GET: Inscripcions
        public ActionResult Index()
        {
            var inscripcion = db.Inscripcion.Include(i => i.Rol);
            return View(inscripcion.ToList());
        }

        // GET: Inscripcions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscripcion inscripcion = db.Inscripcion.Find(id);
            if (inscripcion == null)
            {
                return HttpNotFound();
            }
            return View(inscripcion);
        }

        // GET: Inscripcions/Create
        public ActionResult Create()
        {
            ViewBag.Fk_rol = new SelectList(db.Rol, "Id", "Id");
            ViewBag.Enajenante = new Enajenante();

            return View();
        }

        // POST: Inscripcions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inscripcion inscripcion)
        {
            //Calcular Cantidad de Enajenantes
            int cantidadEnajenantes = inscripcion.Enajenante.Count();

            //Calcular Suma de Porcentajes en Adquirentes y en Adquirentes con Porcentaje No Acreditado
            double sumaPorcentajeAdquirentes;
            double sumaPorcentajeAdquirentesNA;
            if (inscripcion.Adquirente.Sum(a => a.Porcentaje).HasValue)
            {
                sumaPorcentajeAdquirentes = inscripcion.Adquirente.Sum(a => a.Porcentaje).Value;
                sumaPorcentajeAdquirentesNA = inscripcion.Adquirente.Where(a => a.Porcentaje_Na == 1).Sum(a => a.Porcentaje).Value;
            }
            else
            {
                sumaPorcentajeAdquirentes = 0;
                sumaPorcentajeAdquirentesNA = 0;
            }

            //Validar el modelo
            if (ModelState.IsValid)
            {
                //Validar RdP
                if (inscripcion.Cne == "Regularización de Patrimonio")
                {
                    //Condiciones de RdP
                    if(cantidadEnajenantes == 0 && sumaPorcentajeAdquirentes <= 100 && sumaPorcentajeAdquirentesNA == 0)
                    {
                        db.Inscripcion.Add(inscripcion);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        //Error
                    }
                }
                else //No validar el RdP
                {
                    db.Inscripcion.Add(inscripcion);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                
            }

            ViewBag.Fk_rol = new SelectList(db.Rol, "Id", "Id", inscripcion.Fk_rol);
            return View(inscripcion);
        }

        // GET: Inscripcions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscripcion inscripcion = db.Inscripcion.Find(id);
            if (inscripcion == null)
            {
                return HttpNotFound();
            }
            ViewBag.Fk_rol = new SelectList(db.Rol, "Id", "Id", inscripcion.Fk_rol);
            return View(inscripcion);
        }

        // POST: Inscripcions/Edit/5
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Numero_atencion,Cne,Fojas,Creacion,Fk_rol")] Inscripcion inscripcion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inscripcion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Fk_rol = new SelectList(db.Rol, "Id", "Id", inscripcion.Fk_rol);
            return View(inscripcion);
        }

        // GET: Inscripcions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscripcion inscripcion = db.Inscripcion.Find(id);
            if (inscripcion == null)
            {
                return HttpNotFound();
            }
            return View(inscripcion);
        }

        // POST: Inscripcions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inscripcion inscripcion = db.Inscripcion.Find(id);
            db.Inscripcion.Remove(inscripcion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        // GET: Inscripcions/Create
        public ActionResult Search()
        {
            ViewBag.Comunas = db.Comuna.ToList();
            return View();
        }
    }
}
