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
    
    public partial class tSolicitud
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tSolicitud()
        {
            this.tDetalleSolicitud = new HashSet<tDetalleSolicitud>();
        }
    
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Respuesta { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<int> Idusuario { get; set; }
        public Nullable<int> Idtiposolicitud { get; set; }
        public Nullable<int> Estado { get; set; }
        public Nullable<System.DateTime> FechaRespuesta { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tDetalleSolicitud> tDetalleSolicitud { get; set; }
        public virtual tTipoSolicitud tTipoSolicitud { get; set; }
        public virtual tUsuario tUsuario { get; set; }
    }
}