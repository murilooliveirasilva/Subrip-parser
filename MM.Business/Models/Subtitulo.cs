using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMiners.Bussiness.Models
{
    public class Subtitulo : Entity
    {
        public Subtitulo()
        {
            linhasLegenda = new List<string>();
        }

        public int numeroSequencial { get; set; }
        public TimeSpan tempoInicial { get; set; }
        public TimeSpan tempoFinal { get; set; }
        public List<string> linhasLegenda { get; set; }
    }
}
