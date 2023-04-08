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
    using System.ComponentModel.DataAnnotations;

    public partial class Inscripcion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Inscripcion()
        {
            this.Multipropietario = new HashSet<Multipropietario>();
            this.Adquirente = new HashSet<Adquirente>();
            this.Enajenante = new HashSet<Enajenante>();
        }
    
        public int Id { get; set; }
        [Display(Name = "N�mero de Atenci�n")]
        public int Numero_atencion { get; set; }
        [Display(Name = "C�digo de Naturaleza de la Escritura")]
        public string Cne { get; set; }
        public int Fojas { get; set; }
        [Display(Name = "Fecha de Creaci�n")]
        public System.DateTime Creacion { get; set; }
        [Display(Name = "ID Rol Asociado (temporal)")]
        public int Fk_rol { get; set; }
    
        public virtual Rol Rol { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Multipropietario> Multipropietario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Adquirente> Adquirente { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Enajenante> Enajenante { get; set; }
    }
}