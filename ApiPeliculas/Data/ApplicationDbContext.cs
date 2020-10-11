using ApiPeliculas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Data
{
    public class ApplicationDbContext : DbContext 
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //se agregan para que el comando migrations lo agregue en la DB
        //son necesarias porque sin esto no se crean las tablas.
        public DbSet<Categoria> Categoria { get; set; }

        public DbSet<Pelicula> Pelicula { get; set; }

        public DbSet<Usuario> Usuario { get; set; }



    }
}
