using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static ApiPeliculas.Models.Pelicula;

namespace ApiPeliculas.Models.DTO
{
    public class PeliculaUpdateDto
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El Nombre es Requerido")]
        public String Nombre { get; set; }


        [Required(ErrorMessage = "la ruta de la imagen es Requerido")]
        public String RutaImagen { get; set; }


        [Required(ErrorMessage = "Descripcion es Requerido")]
        public String Descripcion { get; set; }


        [Required(ErrorMessage = "Duracion es Requerido")]
        public String Duracion { get; set; }


        [Required(ErrorMessage = "Clasificacion es Requerido")]
        public TipoClasificacion Clasificacion { get; set; }

        //Ahora la relacion con la tabla Categoria
        public int categoriaId { get; set; }
    }
}
