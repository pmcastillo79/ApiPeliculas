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
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "APIPeliculasUsuarios")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IMapper mapper;
        private readonly IConfiguration config;
        public UsuariosController(IUsuarioRepository _usuarioRepository, IMapper _mapper, IConfiguration _config)
        {
            usuarioRepository = _usuarioRepository;
            mapper = _mapper;
            config = _config;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<CategoriaDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = usuarioRepository.GetUsuarios();

            var listaUsuariosDto = new List<UsuarioDto>();

            // no exponemos el modelo sino al DTO
            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDto.Add(mapper.Map<UsuarioDto>(lista));
            }

            return Ok(listaUsuariosDto);
        }

        [HttpGet("{UsuarioId:int}", Name = "GetUsuario")]
        [ProducesResponseType(200, Type = typeof(CategoriaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetUsuario(int UsuarioId)
        {
            var UsuarioItem = usuarioRepository.GetUsuario(UsuarioId);

            if (UsuarioItem == null)
            {
                return NotFound();
            }

            var itemUsuarioDto = mapper.Map<UsuarioDto>(UsuarioItem);

            return Ok(itemUsuarioDto);


        }

        [AllowAnonymous]
        [HttpPost("Registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Registro(UsuarioAuthDto usuario)
        {
            usuario.Usuario = usuario.Usuario.ToLower();

            if (usuarioRepository.ExisteUsuario(usuario.Usuario))
            {
                return BadRequest("El usuario ya existe");
            }

            var usuarioCrear = new Usuario()
            {
                 UsuarioA = usuario.Usuario
            };

            var usuarioCreado = usuarioRepository.Registro(usuarioCrear,usuario.Password);
            return Ok(usuarioCreado);

        }


        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login(UsuarioAuthDto usuario)
        {
                    var usuarioDesdeRepo = usuarioRepository.Login(usuario.Usuario, usuario.Password);

                    if (usuarioDesdeRepo == null)
                    {
                        return Unauthorized();
                    }

                    //empiezo a generar el token jWT
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier,usuarioDesdeRepo.Id.ToString()),
                        new Claim(ClaimTypes.Name,usuarioDesdeRepo.UsuarioA.ToString()),
                    };

                    //tomo las claves del appsettings
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value));

                    //instancio el tipo de credenciales
                    var credenciales = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

                    //genero el descriptor
                    var tokenDescriptor = new SecurityTokenDescriptor() 
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.Now.AddDays(1),
                        SigningCredentials = credenciales
                    };

                    //genero el handler
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    //retorno el token generado.
                    return Ok(new 
                        {
                            token = tokenHandler.WriteToken(token) 
                        });
                            

        }




    }
}
