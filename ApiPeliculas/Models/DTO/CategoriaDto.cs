using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.DTO
{
    public class CategoriaDto
    {
        public int ID { get; set; }

        [Required(ErrorMessage ="El Nombre es requerido")]
        public String Nombre { get; set; }

        public DateTime FechaCreacion { get; set; }

        public String Observacion { get; set; }
    }
}
