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
    
    public partial class tPersona
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Documento { get; set; }
        public Nullable<System.DateTime> FechaNac { get; set; }
        public Nullable<int> Estado { get; set; }
        public Nullable<int> Idcargo { get; set; }
        public Nullable<int> Idsuperior { get; set; }
        public Nullable<int> IdDepartamento { get; set; }
    
        public virtual tCargo tCargo { get; set; }
        public virtual tUsuario tUsuario { get; set; }
        public virtual tDepartamento tDepartamento { get; set; }
    }
}
