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


        public void CreateMultipropietarioRDP(int fk_comuna, int manzana, int predio, List<Adquirente> listaAdquirentes, int fojas, int numeroInscripcion, DateTime fechaInscripcion)
        {
            int anoLimite = 2019;

            //Repartir Porcentajes nulos
            double sumaPorcentajes = 0.0;
            int sumaPorcentajesNulos = 0;

            foreach (var adquirente in listaAdquirentes)
            {
                if(adquirente.Porcentaje != null)
                {
                    sumaPorcentajes += adquirente.Porcentaje.Value;
                }
                else
                {
                    sumaPorcentajesNulos ++;
                }

            }

            double porcentajeRestanteCU = (100 - sumaPorcentajes) / sumaPorcentajesNulos;            

            foreach(var adquirente in listaAdquirentes)
            {
                Multipropietario multipropietario = new Multipropietario();

                multipropietario.Fk_comuna = fk_comuna;
                multipropietario.Manzana = manzana;
                multipropietario.Predio = predio;
                multipropietario.Rut = adquirente.Rut;

                if(adquirente.Porcentaje != null)
                {
                    multipropietario.Porcentaje_derechos = adquirente.Porcentaje.Value;
                }
                else
                {
                    multipropietario.Porcentaje_derechos = porcentajeRestanteCU;
                }
                
                multipropietario.Fojas = fojas;
                multipropietario.Numero_inscripcion = numeroInscripcion;
                multipropietario.Fecha_inscripcion = fechaInscripcion;
                multipropietario.Ano_inscripcion = fechaInscripcion.Year;

                // Verificar que año de Vigencia Inicial 
                if (fechaInscripcion.Year < anoLimite)
                {
                    multipropietario.Vigencia_inicial = anoLimite;
                }
                else
                {
                    multipropietario.Vigencia_inicial = fechaInscripcion.Year;
                }

                multipropietario.Vigencia_final = null;

                database.Multipropietario.Add(multipropietario);


            }

            //Checkear si es una instancia nueva de la propiedad
            if (database.Multipropietario.Where(x => x.Fk_comuna == fk_comuna && x.Manzana == manzana && x.Predio == predio && x.Numero_inscripcion == numeroInscripcion).Any())
            {
                database.Multipropietario.RemoveRange(database.Multipropietario.Where(x => x.Fk_comuna == fk_comuna && x.Manzana == manzana && x.Predio == predio));
            }
            else if (database.Multipropietario.Where(x => x.Fk_comuna == fk_comuna && x.Manzana == manzana && x.Predio == predio && x.Numero_inscripcion != numeroInscripcion).Any())
            {
                foreach (var mp in database.Multipropietario.Where(x => x.Fk_comuna == fk_comuna && x.Manzana == manzana && x.Predio == predio && x.Numero_inscripcion != numeroInscripcion))
                {
                    mp.Vigencia_final = fechaInscripcion.Year - 1;
                }
            }



        }

        public void CreateMultipropietarioCompraventa(int fk_comuna, int manzana, int predio, List<Adquirente> listaAdquirentes, List<Enajenante> listaEnajenantes, int fojas, int numeroInscripcion, DateTime fechaInscripcion)
        {
            int anoLimite = 2019;
            double sumaPorcentajeTotalTransferencia = 0.0;
            double sumaPorcentajes = 0.0;
            int sumaPorcentajesNulos = 0;
            double porcentajeRestanteCU = 0.0;

            //Checkear por enajenantes fantasmas
            foreach (var enajenante in listaEnajenantes)
            {
                var enajenanteMP = database.Multipropietario.Where(e => e.Rut == enajenante.Rut).FirstOrDefault();
                if (enajenanteMP == null)
                {
                    //Error
                    Console.WriteLine("Enajenante No Existe");
                }
                else
                {
                    sumaPorcentajeTotalTransferencia += (enajenanteMP.Porcentaje_derechos.Value * (enajenante.Porcentaje.Value / 100));
                }
            }

            //Repartir Porcentajes nulos

            foreach (var adquirente in listaAdquirentes)
            {
                if (adquirente.Porcentaje != null)
                {
                    sumaPorcentajes += adquirente.Porcentaje.Value;
                }
                else
                {
                    sumaPorcentajesNulos++;
                }

            }

            porcentajeRestanteCU = (sumaPorcentajeTotalTransferencia - sumaPorcentajes) / sumaPorcentajesNulos;

            foreach (var adquirente in listaAdquirentes)
            {
                Multipropietario multipropietario = new Multipropietario();

                multipropietario.Fk_comuna = fk_comuna;
                multipropietario.Manzana = manzana;
                multipropietario.Predio = predio;
                multipropietario.Rut = adquirente.Rut;

                if (adquirente.Porcentaje != null)
                {
                    multipropietario.Porcentaje_derechos = adquirente.Porcentaje.Value;
                }
                else
                {
                    multipropietario.Porcentaje_derechos = porcentajeRestanteCU;
                }

                multipropietario.Fojas = fojas;
                multipropietario.Numero_inscripcion = numeroInscripcion;
                multipropietario.Fecha_inscripcion = fechaInscripcion;
                multipropietario.Ano_inscripcion = fechaInscripcion.Year;

                // Verificar que año de Vigencia Inicial 
                if (fechaInscripcion.Year < anoLimite)
                {
                    multipropietario.Vigencia_inicial = anoLimite;
                }
                else
                {
                    multipropietario.Vigencia_inicial = fechaInscripcion.Year;
                }

                multipropietario.Vigencia_final = null;

                database.Multipropietario.Add(multipropietario);


            }

            //Corroborar si enajenantes dan un 100%
            if(sumaPorcentajeTotalTransferencia < 100)
            {

                foreach(var enajenante in listaEnajenantes)
                {
                    var enajenanteMP = database.Multipropietario.Where(e => e.Rut == enajenante.Rut).FirstOrDefault();

                    Multipropietario multipropietario = new Multipropietario();

                    multipropietario.Fk_comuna = fk_comuna;
                    multipropietario.Manzana = manzana;
                    multipropietario.Predio = predio;
                    multipropietario.Rut = enajenante.Rut;
                    multipropietario.Fojas = fojas;
                    multipropietario.Porcentaje_derechos = (enajenanteMP.Porcentaje_derechos.Value * (enajenante.Porcentaje.Value / 100));
                    multipropietario.Numero_inscripcion = numeroInscripcion;
                    multipropietario.Fecha_inscripcion = fechaInscripcion;
                    multipropietario.Ano_inscripcion = fechaInscripcion.Year;

                    // Verificar que año de Vigencia Inicial 
                    if (fechaInscripcion.Year < anoLimite)
                    {
                        multipropietario.Vigencia_inicial = anoLimite;
                    }
                    else
                    {
                        multipropietario.Vigencia_inicial = fechaInscripcion.Year;
                    }

                    multipropietario.Vigencia_final = null;

                    database.Multipropietario.Add(multipropietario);
                }
                
            }

            //Agregar los enajenantes que faltan 
            var listaEnajenantesBBDD = database.Multipropietario.Where(mp => mp.Fk_comuna == fk_comuna && mp.Manzana == manzana && mp.Predio == predio).ToList();
            foreach(var enajenante in listaEnajenantesBBDD)
            {
                if(enajenante.Porcentaje_derechos.Value > 0 && enajenante.Numero_inscripcion != numeroInscripcion && enajenante.Vigencia_final == null)
                {
                    int contador = 0;
                    foreach(var e in listaEnajenantes)
                    {
                        if(e.Rut == enajenante.Rut)
                        {
                            contador++;
                        }
                    }

                    if(contador == 0)
                    {
                        Multipropietario multipropietario = new Multipropietario();

                        multipropietario.Fk_comuna = fk_comuna;
                        multipropietario.Manzana = manzana;
                        multipropietario.Predio = predio;
                        multipropietario.Rut = enajenante.Rut;
                        multipropietario.Fojas = fojas;
                        multipropietario.Porcentaje_derechos = enajenante.Porcentaje_derechos.Value;
                        multipropietario.Numero_inscripcion = numeroInscripcion;
                        multipropietario.Fecha_inscripcion = fechaInscripcion;
                        multipropietario.Ano_inscripcion = fechaInscripcion.Year;

                        // Verificar que año de Vigencia Inicial 
                        if (fechaInscripcion.Year < anoLimite)
                        {
                            multipropietario.Vigencia_inicial = anoLimite;
                        }
                        else
                        {
                            multipropietario.Vigencia_inicial = fechaInscripcion.Year;
                        }

                        multipropietario.Vigencia_final = null;

                        database.Multipropietario.Add(multipropietario);
                    }


                   
                }

            }

            //Checkear si es una instancia nueva de la propiedad
            if (database.Multipropietario.Where(x => x.Fk_comuna == fk_comuna && x.Manzana == manzana && x.Predio == predio && x.Numero_inscripcion == numeroInscripcion).Any())
            {
                database.Multipropietario.RemoveRange(database.Multipropietario.Where(x => x.Fk_comuna == fk_comuna && x.Manzana == manzana && x.Predio == predio));
            }
            else if (database.Multipropietario.Where(x => x.Fk_comuna == fk_comuna && x.Manzana == manzana && x.Predio == predio && x.Numero_inscripcion != numeroInscripcion).Any())
            {
                foreach (var mp in database.Multipropietario.Where(x => x.Fk_comuna == fk_comuna && x.Manzana == manzana && x.Predio == predio && x.Numero_inscripcion != numeroInscripcion))
                {
                    mp.Vigencia_final = fechaInscripcion.Year - 1;
                }
            }



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

            if(adquirentesList != null)
            {
                foreach (var adquirente in adquirentesList)
                {
                    inscripcion.Adquirente.Add(adquirente);
                }
            }
            
            // Calcular Cantidad de Enajenantes y Adquirentes
            int cantidadEnajenantes = inscripcion.Enajenante.Count();
            int cantidadAdquirentes = inscripcion.Adquirente.Count();

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
                        CreateMultipropietarioRDP(comuna, manzana, predio, adquirentesList, inscripcion.Fojas, inscripcion.Numero_inscripcion, inscripcion.Creacion);

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
                        }

                        database.Inscripcion.Add(inscripcion);

                        database.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Error");
                    }
                }
                else  //Validar Compraventa
                {
                    //Condiciones de Compraventa
                    if (cantidadEnajenantes >=1 && cantidadAdquirentes >=1)
                    {
                        CreateMultipropietarioCompraventa(comuna, manzana, predio, adquirentesList, enajenantesList, inscripcion.Fojas, inscripcion.Numero_inscripcion, inscripcion.Creacion);

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
                        }

                        database.Inscripcion.Add(inscripcion);

                        database.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Error");
                    }
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

        public ActionResult Error()
        {
            return View();
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
