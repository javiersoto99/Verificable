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
        private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();

        // GET: Inscripcions
        public ActionResult Index()
        {
            var inscripcion = db.Inscripcion.Include(i => i.Rol);
            return View(inscripcion.ToList());
        }

        // GET: Inscripcions/Details/id
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

            var joinComunaRol = (from r in db.Rol join c in db.Comuna on r.Fk_comuna equals c.Id select new
                    {
                        Nombre = c.Nombre
                    }
                ).FirstOrDefault();

            ViewBag.Comuna = joinComunaRol.Nombre;
            ViewBag.Manzana = db.Rol.Where(r => r.Id == inscripcion.Fk_rol).Select(r => r.Manzana).FirstOrDefault();
            ViewBag.Predio = db.Rol.Where(r => r.Id == inscripcion.Fk_rol).Select(r => r.Predio).FirstOrDefault();

            return View(inscripcion);
        }

        // GET: Inscripcions/Create
        public ActionResult Create()
        {
            ViewBag.Fk_rol = new SelectList(db.Rol, "Id", "Id");
            ViewBag.Comunas = db.Comuna.ToList();

            return View();
        }

        // Crear Rol
        public Rol CreateRol(int comuna, int manzana, int predio)
        {
            Rol rol = new Rol();
            rol.Fk_comuna = comuna;
            rol.Manzana = manzana;
            rol.Predio = predio;

            return rol;
        }

        //Crear Multipropietario
        public Multipropietario CreateMP(int fk_comuna, int manzana, int predio, string rut, double? porcentajeDerechos, int fojas, int numeroInscripcion, DateTime fechaInscripcion)
        {
            Multipropietario mp= new Multipropietario();

            mp.Fk_comuna = fk_comuna;
            mp.Manzana = manzana;
            mp.Predio = predio;
            mp.Rut = rut;
            mp.Porcentaje_derechos = porcentajeDerechos;
            mp.Fojas = fojas;
            mp.Numero_inscripcion = numeroInscripcion;
            mp.Fecha_inscripcion = fechaInscripcion;
            mp.Ano_inscripcion = fechaInscripcion.Year;
            //Verificar que Vigencia Inicial sea de 2019 en adelante
            if (fechaInscripcion.Year < 2019)
            {
                mp.Vigencia_inicial = 2019;
            }
            else
            {
                mp.Vigencia_inicial = fechaInscripcion.Year;
            }

            mp.Vigencia_final = null;
 
            return mp;
        }

        // TODO: Separar método en varios métodos más pequeños
        // POST: Inscripcions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inscripcion inscripcion, int comuna, int manzana, int predio, FormCollection form)
        {

            //Crear Enajenantes y agregarlos a la Inscripción
            var enajenantesListJson = form["enajenantesList"];
            var enajenantesList = JsonConvert.DeserializeObject<List<Enajenante>>(enajenantesListJson);

            if(enajenantesList != null)
            {
                foreach (var enajenante in enajenantesList)
                {
                    inscripcion.Enajenante.Add(enajenante);
                }
            }

            //Crear Adquirentes y agregarlos a la Inscripción
            var adquirentesListJson = form["adquirentesList"];
            var adquirentesList = JsonConvert.DeserializeObject<List<Adquirente>>(adquirentesListJson);

            foreach (var adquirente in adquirentesList)
            {
                inscripcion.Adquirente.Add(adquirente);
            }

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
                    if (cantidadEnajenantes == 0 && sumaPorcentajeAdquirentes <= 100 && sumaPorcentajeAdquirentesNA == 0)
                    {
                        //Agregar Rol, Enajenantes, Adquirentes e Inscripción a la BBDD
                        Rol rol = CreateRol(comuna, manzana, predio);
                        db.Rol.Add(rol);
                        inscripcion.Fk_rol = rol.Id;

                        if (enajenantesList != null)
                        {
                            foreach (var enajenante in enajenantesList)
                            {
                                db.Enajenante.Add(enajenante);
                            }
                        }
                            
                        foreach (var adquirente in adquirentesList)
                        {
                            db.Adquirente.Add(adquirente);
                            //Crear instancia de Multipropietario
                            Multipropietario multiP = CreateMP(comuna, manzana, predio, adquirente.Rut, adquirente.Porcentaje, inscripcion.Fojas, inscripcion.Numero_inscripcion, inscripcion.Creacion);
                            db.Multipropietario.Add(multiP);
                        }

                        db.Inscripcion.Add(inscripcion);

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        //Retornar Error
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }
                else //No validar el RdP
                {
                    //Agregar Rol, Enajenantes, Adquirentes e Inscripción a la BBDD
                    Rol rol = CreateRol(comuna, manzana, predio);
                    db.Rol.Add(rol);
                    inscripcion.Fk_rol = rol.Id;

                    if (enajenantesList != null)
                    {
                        foreach (var enajenante in enajenantesList)
                        {
                            db.Enajenante.Add(enajenante);
                        }
                    }

                    foreach (var adquirente in adquirentesList)
                    {
                        db.Adquirente.Add(adquirente);
                        //Crear instancia de Multipropietario
                        Multipropietario multiP = CreateMP(comuna, manzana, predio, adquirente.Rut, adquirente.Porcentaje, inscripcion.Fojas, inscripcion.Numero_inscripcion, inscripcion.Creacion);
                        db.Multipropietario.Add(multiP);
                    }
                    db.Inscripcion.Add(inscripcion);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }


            }

            ViewBag.Fk_rol = new SelectList(db.Rol, "Id", "Id", inscripcion.Fk_rol);
            return View(inscripcion);

        }

        // GET: Inscripcions/Edit/id
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

        // POST: Inscripcions/Edit/id
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Numero_inscripcion,Cne,Fojas,Creacion,Fk_rol")] Inscripcion inscripcion)
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

        // GET: Inscripcions/Delete/id
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

        // POST: Inscripcions/Delete/id
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
    }
}
