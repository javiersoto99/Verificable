//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UAndes.ICC5103._202301.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Multipropietario
    {
        public int Id { get; set; }
        public int Fk_comuna { get; set; }
        public int Manzana { get; set; }
        public int Predio { get; set; }
        public string Rut { get; set; }
        public Nullable<double> Porcentaje_derechos { get; set; }
        public int Fojas { get; set; }
        public int Ano_inscripcion { get; set; }
        public int Numero_inscripcion { get; set; }
        public System.DateTime Fecha_inscripcion { get; set; }
        public int Vigencia_inicial { get; set; }
        public Nullable<int> Vigencia_final { get; set; }
    
        public virtual Comuna Comuna { get; set; }
    }
}
