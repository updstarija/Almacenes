using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Almacenes.Models
{
    public class Solicitudes
    {
        private int rechazadas;
        private int pendientes;
        private int aprobadas;
        private int rechazadasTotal;
        private int pendientesTotal;
        private int aprobadasTotal;

        public int Rechazadas { get => rechazadas; set => rechazadas = value; }
        public int Pendientes { get => pendientes; set => pendientes = value; }
        public int Aprobadas { get => aprobadas; set => aprobadas = value; }
        public int RechazadasTotal { get => rechazadasTotal; set => rechazadasTotal = value; }
        public int PendientesTotal { get => pendientesTotal; set => pendientesTotal = value; }
        public int AprobadasTotal { get => aprobadasTotal; set => aprobadasTotal = value; }
    }
}