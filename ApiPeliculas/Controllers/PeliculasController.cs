using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPeliculas.Models.DTO;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiPeliculas.Models;
using System.IO;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "APIPeliculas")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class PeliculasController : Controller
    {
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly IMapper _mapper;

        //para permitir la subida de archivos.
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PeliculasController(IPeliculaRepository peliculaRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _peliculaRepository = peliculaRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<CategoriaDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = _peliculaRepository.GetPeliculas();

            var listaPeliculasDto = new List<PeliculaDto>();

            // no exponemos el modelo sino al DTO
            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(lista));
            }

            return Ok(listaPeliculasDto);
        }






        [AllowAnonymous]
        [HttpGet("{PeliculaId:int}", Name = "GetPelicula")]
        [ProducesResponseType(200, Type = typeof(CategoriaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetPelicula(int PeliculaId)
        {
            var PeliculaItem = _peliculaRepository.GetPelicula(PeliculaId);

            if (PeliculaItem == null)
            {
                return NotFound();
            }

            var itemPeliculaDto = _mapper.Map<PeliculaDto>(PeliculaItem);

            return Ok(itemPeliculaDto);


        }

        [AllowAnonymous]
        [HttpGet("GetPeliculasEnCategoria/{categoriaId:int}")]
        [ProducesResponseType(200, Type = typeof(CategoriaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetPeliculasEnCategoria(int categoriaId)
        {
            var listaPeliculas = _peliculaRepository.GetPeliculasEnCategoria(categoriaId);

            if (listaPeliculas == null)
                return NotFound();

            var itemPelicula = new List<PeliculaDto>();
            foreach (var item in listaPeliculas)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDto>(item));
            }

            return Ok(itemPelicula);

        }

        [AllowAnonymous]
        [HttpGet("Buscar")]
        [ProducesResponseType(200, Type = typeof(CategoriaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult Buscar(string nombre)
        {
            try
            {
                var resultado = _peliculaRepository.BuscarPelicula(nombre);
                if (resultado.Any())
                {
                    return Ok(resultado);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de la aplicacion");
            }
        }

              
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoriaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearPelicula([FromForm] PeliculaCreateDto PeliculaDto)
        {
            if (PeliculaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_peliculaRepository.ExistePelicula(PeliculaDto.Nombre))
            {
                ModelState.AddModelError("", "La Pelicula ya existe");
                return StatusCode(404, ModelState);
            }

            //subida de archivos aca

            var archivo = PeliculaDto.Foto;
            string rutaPrincipal = _webHostEnvironment.WebRootPath;
            var archivos = HttpContext.Request.Form.Files;

            if (archivo.Length > 0)
            {
                //nueva imagen
                var nombreFoto = Guid.NewGuid().ToString();
                var subidas = Path.Combine(rutaPrincipal, @"fotos");
                var extension = Path.GetExtension(archivos[0].FileName);         


                using (var fileStreams = new FileStream(Path.Combine(subidas, nombreFoto + extension), FileMode.Create))
                {
                    archivos[0].CopyTo(fileStreams);
                    PeliculaDto.RutaImagen = @"\fotos\" + nombreFoto + extension;
                }

            }

            var Pelicula = _mapper.Map<Pelicula>(PeliculaDto);

            if (!_peliculaRepository.CrearPelicula(Pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {Pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            //llama a otro action y retorna el valor del Id
            return CreatedAtRoute("GetPelicula", new { PeliculaId = Pelicula.ID }, Pelicula);
        }

       
        [HttpPatch("{PeliculaId:int}", Name = "ActualizarPelicula")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPelicula(int PeliculaId, [FromBody] PeliculaDto PeliculaDto)
        {
            if (PeliculaDto == null || PeliculaId != PeliculaDto.ID)
            {
                return BadRequest(ModelState);
            }


            var Pelicula = _mapper.Map<Pelicula>(PeliculaDto);

            if (!_peliculaRepository.ActualizarPelicula(Pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {Pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();



        }


        [HttpDelete("{PeliculaId:int}", Name = "BorrarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarPelicula(int PeliculaId)
        {

            if (!_peliculaRepository.ExistePelicula(PeliculaId))
            {
                return NotFound();
            }

            var Pelicula = _peliculaRepository.GetPelicula(PeliculaId);

            if (!_peliculaRepository.BorrarPelicula(Pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {Pelicula.Nombre}");
                return StatusCode(500, ModelState);

            }

            return NoContent();

        }
    }
}
