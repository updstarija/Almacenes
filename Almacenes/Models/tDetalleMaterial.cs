//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Almacenes.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tDetalleMaterial
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tDetalleMaterial()
        {
            this.tDetalleSolicitud = new HashSet<tDetalleSolicitud>();
        }
    
        public int Id { get; set; }
        public int IdUnidad { get; set; }
        public int IdMaterial { get; set; }
        public Nullable<int> Estado { get; set; }
    
        public virtual tMaterial tMaterial { get; set; }
        public virtual tUnidad tUnidad { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tDetalleSolicitud> tDetalleSolicitud { get; set; }
    }
}
