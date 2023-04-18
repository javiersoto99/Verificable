using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Controllers
{
    public class InscripcionsController : Controller
    {
        private InscripcionesBrDbEntities database = new InscripcionesBrDbEntities();

        public ActionResult Index()
        {
            var inscripcion = database.Inscripcion.Include(i => i.Rol);
            return View(inscripcion.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Inscripcion inscripcion = database.Inscripcion.Find(id);

            if (inscripcion == null)
            {
                return HttpNotFound();
            }

            var joinComunaRol = (from r in database.Rol join c in database.Comuna on r.Fk_comuna equals c.Id select new
                    {
                        Nombre = c.Nombre
                    }
                ).FirstOrDefault();

            ViewBag.Comuna = joinComunaRol.Nombre;
            ViewBag.Manzana = database.Rol.Where(r => r.Id == inscripcion.Fk_rol).Select(r => r.Manzana).FirstOrDefault();
            ViewBag.Predio = database.Rol.Where(r => r.Id == inscripcion.Fk_rol).Select(r => r.Predio).FirstOrDefault();

            return View(inscripcion);
        }

        public ActionResult Create()
        {
            ViewBag.Fk_rol = new SelectList(database.Rol, "Id", "Id");
            ViewBag.Comunas = database.Comuna.ToList();

            return View();
        }

        public Rol CreateRol(int comuna, int manzana, int predio)
        {
            Rol rol = new Rol();

            rol.Fk_comuna = comuna;
            rol.Manzana = manzana;
            rol.Predio = predio;

            return rol;
        }

        public Multipropietario CreateMultipropietario(int fk_comuna, int manzana, int predio, string rut, double? porcentajeDerechos, int fojas, int numeroInscripcion, DateTime fechaInscripcion)
        {
            Multipropietario multipropietario= new Multipropietario();

            multipropietario.Fk_comuna = fk_comuna;
            multipropietario.Manzana = manzana;
            multipropietario.Predio = predio;
            multipropietario.Rut = rut;
            multipropietario.Porcentaje_derechos = porcentajeDerechos;
            multipropietario.Fojas = fojas;
            multipropietario.Numero_inscripcion = numeroInscripcion;
            multipropietario.Fecha_inscripcion = fechaInscripcion;
            multipropietario.Ano_inscripcion = fechaInscripcion.Year;

            // Verificar que Vigencia Inicial sea de 2019 en adelante
            if (fechaInscripcion.Year < 2019)
            {
                multipropietario.Vigencia_inicial = 2019;
            }
            else
            {
                multipropietario.Vigencia_inicial = fechaInscripcion.Year;
            }

            multipropietario.Vigencia_final = null;
 
            return multipropietario;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inscripcion inscripcion, int comuna, int manzana, int predio, FormCollection form)
        {

            // Crear Enajenantes y agregarlos a la Inscripción
            var enajenantesListJson = form["enajenantesList"];
            var enajenantesList = JsonConvert.DeserializeObject<List<Enajenante>>(enajenantesListJson);

            if(enajenantesList != null)
            {
                foreach (var enajenante in enajenantesList)
                {
                    inscripcion.Enajenante.Add(enajenante);
                }
            }

            // Crear Adquirentes y agregarlos a la Inscripción
            var adquirentesListJson = form["adquirentesList"];
            var adquirentesList = JsonConvert.DeserializeObject<List<Adquirente>>(adquirentesListJson);

            foreach (var adquirente in adquirentesList)
            {
                inscripcion.Adquirente.Add(adquirente);
            }

            // Calcular Cantidad de Enajenantes
            int cantidadEnajenantes = inscripcion.Enajenante.Count();

            // Calcular Suma de Porcentajes en Adquirentes y en Adquirentes con Porcentaje No Acreditado
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

            if (ModelState.IsValid)
            {
                //Validar RdP
                if (inscripcion.Cne == "Regularización de Patrimonio")
                {
                    //Condiciones de RdP
                    if (cantidadEnajenantes == 0 && sumaPorcentajeAdquirentes <= 100 && sumaPorcentajeAdquirentesNA == 0)
                    {
                        //Agregar Rol, Enajenantes, Adquirentes e Inscripción a la BBDD
                        Rol rol = CreateRol(comuna, manzana, predio);
                        database.Rol.Add(rol);
                        inscripcion.Fk_rol = rol.Id;

                        if (enajenantesList != null)
                        {
                            foreach (var enajenante in enajenantesList)
                            {
                                database.Enajenante.Add(enajenante);
                            }
                        }

                        foreach (var adquirente in adquirentesList)
                        {
                            database.Adquirente.Add(adquirente);

                            // Crear instancia de Multipropietario
                            if (database.Multipropietario.Where(x => x.Fk_comuna == comuna && x.Manzana == manzana && x.Predio == predio).Any())
                            {
                                database.Multipropietario.RemoveRange(database.Multipropietario.Where(x => x.Fk_comuna == comuna && x.Manzana == manzana && x.Predio == predio));
                            }

                            Multipropietario multiP = CreateMultipropietario(comuna, manzana, predio, adquirente.Rut, adquirente.Porcentaje, inscripcion.Fojas, inscripcion.Numero_inscripcion, inscripcion.Creacion);
                            database.Multipropietario.Add(multiP);
                        }

                        database.Inscripcion.Add(inscripcion);

                        database.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }
                else  // No validar el RdP
                {
                    // Agregar Rol, Enajenantes, Adquirentes e Inscripción a la BBDD
                    Rol rol = CreateRol(comuna, manzana, predio);
                    database.Rol.Add(rol);
                    inscripcion.Fk_rol = rol.Id;

                    if (enajenantesList != null)
                    {
                        foreach (var enajenante in enajenantesList)
                        {
                            database.Enajenante.Add(enajenante);
                        }
                    }

                    foreach (var adquirente in adquirentesList)
                    {
                        database.Adquirente.Add(adquirente);
                        // Crear instancia de Multipropietario

                        Multipropietario multiP = CreateMultipropietario(comuna, manzana, predio, adquirente.Rut, adquirente.Porcentaje, inscripcion.Fojas, inscripcion.Numero_inscripcion, inscripcion.Creacion);
                        database.Multipropietario.Add(multiP);
                    }
                    database.Inscripcion.Add(inscripcion);
                    database.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Fk_rol = new SelectList(database.Rol, "Id", "Id", inscripcion.Fk_rol);
            return View(inscripcion);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Inscripcion inscripcion = database.Inscripcion.Find(id);

            if (inscripcion == null)
            {
                return HttpNotFound();
            }

            ViewBag.Fk_rol = new SelectList(database.Rol, "Id", "Id", inscripcion.Fk_rol);
            return View(inscripcion);
        }

        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Numero_inscripcion,Cne,Fojas,Creacion,Fk_rol")] Inscripcion inscripcion)
        {
            if (ModelState.IsValid)
            {
                database.Entry(inscripcion).State = EntityState.Modified;
                database.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Fk_rol = new SelectList(database.Rol, "Id", "Id", inscripcion.Fk_rol);

            return View(inscripcion);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Inscripcion inscripcion = database.Inscripcion.Find(id);

            if (inscripcion == null)
            {
                return HttpNotFound();
            }

            return View(inscripcion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inscripcion inscripcion = database.Inscripcion.Find(id);
            database.Inscripcion.Remove(inscripcion);
            database.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
