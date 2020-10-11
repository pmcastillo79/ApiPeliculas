using ApiPeliculas.Models;
using ApiPeliculas.Models.DTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.PeliculasMapper
{
    /// <summary>
    /// vinculamos el DTO con el modelo.
    /// </summary>
    public class PeliculasMappers : Profile 
    {
        public PeliculasMappers()
        {
            //se llenan los objetos con los campos comunes entre el modelo y los DTO
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            //peliculas
            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<Pelicula, PeliculaCreateDto>().ReverseMap();
            CreateMap<Pelicula, PeliculaUpdateDto>().ReverseMap();
            //Usuarios
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            //CreateMap<Usuario, UsuarioAuthDto>().ReverseMap();
            //CreateMap<Usuario, UsuarioAuthLoginDto>().ReverseMap();


        }
    }
}
