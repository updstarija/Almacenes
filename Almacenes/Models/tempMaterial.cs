using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Almacenes.Models
{
    public class tempMaterial
    {
        private int idMaterial;
        private string nombre;
        private int idUnidad;
        private string nombreUnidad;
        private string nombreUnidadPadre;
        private int cantidadUnidad;
        private int cantidadUnidadPadre;
        private int total;
        private string sumConvertUnit;
        private List<tDetalleMaterial> Unidades;

        public int IdMaterial { get => idMaterial; set => idMaterial = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public int IdUnidad { get => idUnidad; set => idUnidad = value; }
        public string NombreUnidad { get => nombreUnidad; set => nombreUnidad = value; }
        public string NombreUnidadPadre { get => nombreUnidadPadre; set => nombreUnidadPadre = value; }
        public int CantidadUnidad { get => cantidadUnidad; set => cantidadUnidad = value; }
        public int CantidadUnidadPadre { get => cantidadUnidadPadre; set => cantidadUnidadPadre = value; }
        public int Total { get => total; set => total = value; }
        public string SumConvertUnit { get => sumConvertUnit; set => sumConvertUnit = value; }
        public List<tDetalleMaterial> ListUnidades { get => Unidades; set => Unidades = value; }
    }
}