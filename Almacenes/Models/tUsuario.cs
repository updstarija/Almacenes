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
    
    public partial class tUsuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tUsuario()
        {
            this.tReseteoContraseña = new HashSet<tReseteoContraseña>();
            this.tSolicitud = new HashSet<tSolicitud>();
        }
    
        public int Id { get; set; }
        public string Nomusu { get; set; }
        public string Password { get; set; }
        public Nullable<int> Estado { get; set; }
        public Nullable<int> IdTtipo { get; set; }
    
        public virtual tPersona tPersona { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tReseteoContraseña> tReseteoContraseña { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tSolicitud> tSolicitud { get; set; }
        public virtual tTipo tTipo { get; set; }
    }
}