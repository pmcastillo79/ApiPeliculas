using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class PeliculaRepository : IPeliculaRepository
    {
        private readonly ApplicationDbContext _db;

        public PeliculaRepository(ApplicationDbContext db, IMapper mp)
        {
            _db = db;
        }

        public bool ActualizarPelicula(Pelicula pelicula)
        {
            _db.Pelicula.Update(pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula pelicula)
        {
            _db.Pelicula.Remove(pelicula);
            return Guardar();
        }

        public IEnumerable<Pelicula> BuscarPelicula(string nombre)
        {
            IQueryable<Pelicula> query = _db.Pelicula;

            if (!String.IsNullOrEmpty(nombre))
            {
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre));
            }

            return query.ToList();


        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            _db.Pelicula.Add(pelicula);
            return Guardar();
        }

        public bool ExistePelicula(string Nombre)
        {
            return _db.Categoria.Any(c => c.Nombre.ToLower().Trim() == Nombre);
       }

        public bool ExistePelicula(int id)
        {
            return _db.Pelicula.Any(o => o.ID == id);

        }

        public Pelicula GetPelicula(int peliculaId)
        {
            return _db.Pelicula.FirstOrDefault(f=>f.ID == peliculaId);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return _db.Pelicula.OrderBy(d => d.Nombre).ToList();

        }

        public ICollection<Pelicula> GetPeliculasEnCategoria(int categoriaId)
        {
            return _db.Pelicula.Include(ca => ca.Categoria).Where(c => c.categoriaId == categoriaId).ToList();
        }

        public bool Guardar()
        {
            return  _db.SaveChanges() >= 0 ? true : false;
        }
    }
}
