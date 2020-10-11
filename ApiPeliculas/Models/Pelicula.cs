using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models
{
    public class Pelicula
    {
        [Key]
        public int ID { get; set; }
        public String Nombre { get; set; }
        public String RutaImagen { get; set; }
        public String Descripcion { get; set; }
        public String Duracion { get; set; }
        public TipoClasificacion Clasificacion { get; set; }
        public enum TipoClasificacion
        {
            Siete, Trece, Dieciseis, Dieciocho
        } 

        public DateTime FechaCreacion { get; set; }



        //Ahora la relacion con la tabla Categoria
        public int categoriaId { get; set; }
        [ForeignKey("categoriaId")]
        public Categoria Categoria{ get; set; }


    }
}
