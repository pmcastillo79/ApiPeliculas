using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPeliculas.Models;
using ApiPeliculas.Models.DTO;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "APIPeliculasCategorias")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class CategoriasController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapeador;

        public CategoriasController(ICategoryRepository categoryRepository, IMapper mapeador)
        {
            _categoryRepository = categoryRepository;
            _mapeador = mapeador;
        }


        /// <summary>
        /// Obtener todas las categorias 
        /// </summary>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<CategoriaDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategorias()
        {
            var listaCategorias = _categoryRepository.GetCategorias();

            var listaCategoriasDto = new List<CategoriaDto>();

            // no exponemos el modelo sino al DTO
            foreach (var lista in listaCategorias)
            {
                listaCategoriasDto.Add(_mapeador.Map<CategoriaDto>(lista));
            }

            return Ok(listaCategoriasDto);
        }


        /// <summary>
        /// obtener una categoria individual
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <returns></returns>
        ///
        [AllowAnonymous]
        [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
        [ProducesResponseType(200, Type = typeof(CategoriaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetCategoria(int categoriaId)
        {
            var categoriaItem = _categoryRepository.GetCategoria(categoriaId);

            if (categoriaItem == null)
            {
                return NotFound();
            }

            var itemCategoriaDto = _mapeador.Map<CategoriaDto>(categoriaItem);
            
            return Ok(itemCategoriaDto);


        }


        /// <summary>
        /// Crea una categoria
        /// </summary>
        /// <param name="categoriaDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoriaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearCategoria([FromBody] CategoriaDto categoriaDto)
        {
            if (categoriaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.ExisteCategoria(categoriaDto.Nombre))
            {
                ModelState.AddModelError("", "La categoria ya existe");
                return StatusCode(404, ModelState);
            }

            var categoria = _mapeador.Map<Categoria>(categoriaDto);

            if (!_categoryRepository.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            //llama a otro action y retorna el valor del Id
            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.ID }, categoria);
        }


        /// <summary>
        /// Actualiza una categoria
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <param name="categoriaDto"></param>
        /// <returns></returns>
        [HttpPatch("{categoriaId:int}", Name = "ActualizarCategoria")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            if (categoriaDto == null || categoriaId != categoriaDto.ID)
            {
                return BadRequest(ModelState);
            }


            var categoria = _mapeador.Map<Categoria>(categoriaDto);

            if (!_categoryRepository.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();



        }


        /// <summary>
        /// Borra la categoria.
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <returns></returns>
        [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarCategoria(int categoriaId)
        {

            if (!_categoryRepository.ExisteCategoria(categoriaId))
            {
                return NotFound();
            }

            var categoria = _categoryRepository.GetCategoria(categoriaId);

            if (!_categoryRepository.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("",$"Algo salio mal borrando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);

            }

            return NoContent();

        }



    }
}
