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
    
    public partial class Adquirente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Adquirente()
        {
            this.Inscripcion = new HashSet<Inscripcion>();
        }
    
        public int Id { get; set; }
        public string Rut { get; set; }
        public string Nombre { get; set; }
        public Nullable<double> Porcentaje { get; set; }
        public int Porcentaje_Na { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inscripcion> Inscripcion { get; set; }
    }
}
