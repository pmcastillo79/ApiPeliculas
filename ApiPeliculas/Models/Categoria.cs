using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Categoria

    {
        [Key]
        public int ID { get; set; }
        
        public String Nombre { get; set; }

        public DateTime FechaCreacion { get; set; }

        public String Observacion { get; set; }

    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
