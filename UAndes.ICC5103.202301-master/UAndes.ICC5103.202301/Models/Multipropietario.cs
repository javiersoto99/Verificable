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
        public int Fk_rol { get; set; }
        public int Fk_persona { get; set; }
        public double Porcentaje { get; set; }
        public int Fojas { get; set; }
        public int Ano_inscripcion { get; set; }
        public int Fk_numero_inscripcion { get; set; }
        public int Vigencia_inicial { get; set; }
        public Nullable<int> Vigencia_final { get; set; }
    
        public virtual Inscripcion Inscripcion { get; set; }
        public virtual Persona Persona { get; set; }
        public virtual Rol Rol { get; set; }
    }
}
