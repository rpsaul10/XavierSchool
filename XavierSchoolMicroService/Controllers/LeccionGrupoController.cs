using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using XavierSchoolMicroService.Utilities;
using System.Security.Cryptography;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;
using Microsoft.AspNetCore.Authorization;

namespace XavierSchoolMicroService.Controllers
{
    [Authorize]
    public class LeccionGrupoController : ControllerBase
    {
        private readonly IServiceLecPublicas _service;
        private readonly ILogger<LeccionGrupoController> _logger;
        private readonly IServiceUsuarios _userService;
        public LeccionGrupoController(IServiceLecPublicas service, ILogger<LeccionGrupoController> logger, IServiceUsuarios userService)
        {
            _logger = logger;
            _service = service;
            _userService = userService;
        }

        [HttpGet("api/lecGrupo/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllLeccionesGrupo()
        {
            _logger.LogInformation($"{Utils.GetMail(_userService, this)} -> Intentando obtener la lista de Lecciones en grupo");
            try
            {
                var lecs = _service.GetAll();
                // Si todo sale bien retorna la lista aunque este vacia y un un codigo RequestCode 200
                return Ok (lecs);    
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{Utils.GetMail(_userService, this)} -> Error durante la consulta de las lecciones en grupo");
                // Si algo sale mal se retornara la excepcion con un RequestCode 500
                throw;
            }
        }

        [HttpGet ("api/lecGrupo/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetLeccionGrupo(string id)
        {
            _logger.LogInformation($"{Utils.GetMail(_userService, this)} -> Intentando obtener los datos de una leccion en grupo con id : {id}");
            try
            {
                var lecc = _service.GetLecPublica(id);
                // Si "lecc" es null se retorna un mensaje que inofrma que el usuario no fue encontrado
                // Ademas de un RequestCode de 400
                if (lecc == null)
                    return BadRequest($"La leccion en grupo con id: {id} no fue encontrada");
                // Si todo sale bien se retorna la informacion de la leccion y un RequestCode  200
                return Ok (lecc);
            }
            catch (CryptographicException ce)
            {
                _logger.LogError(ce,$"{Utils.GetMail(_userService, this)} -> No se pudo decriptar el id insertado : {id}");
                // Si cae en este catch significa que hubo algo mal en el id de entrada
                // Se retorna un mensaje de error y un RequestCode de 400
                return BadRequest("Entrada Invalida");
            } catch (InvalidOperationException fe)
            {
                _logger.LogError(fe, "{Utils.GetMail(_userService, this)} -> Error por cadena demasiado corta");
                // Si cae en este catch significa que hubo algo mal en el id de entrada
                // Se retorna un mensaje de error y un RequestCode de 400
                return BadRequest("Entrada Invalida");
            }
            catch (System.Exception e)
            {
                // Si llegamos hasta aca significa que hubo un problema interno no esperado
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, $"{Utils.GetMail(_userService, this)} -> Un error ocurrio durante la obtencion de la leccion en grupo");
                throw;
            }
        }

        [HttpPost ("api/lecGrupo/save")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SaveLeccionGrupo([FromBody] RequestBodyPublic c)
        {
            if (!_userService.EsAdministrador(Utils.GetId(this)))
                return Unauthorized("El usuario no es administrador");
            
            _logger.LogInformation($"{Utils.GetMail(_userService, this)} -> Intentando registrar una nueva leccion en grupo : {c.lec}");
            try
            {
                var res = _service.SaveLeccPublica(c.lec, c.est, c.hour);
                // Si todo sale bien se retorna true y un RequestCode 200
                return Ok (res);  
            }
            catch (System.Exception e)
            {
                // Si algo sale mal en la insercion caeremos aqui
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, $"{Utils.GetMail(_userService, this)} -> Un error ocurrio durante el registro de la leccion en grupo");
                throw;
            }
        }

        [HttpGet ("api/lecGrupo/estuds/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetEstudiantesByIdLeccion(string id)
        {
            _logger.LogInformation($"{Utils.GetMail(_userService, this)} -> Intentando obtener los estudiantes de una leccion en grupo con id : {id}");
            try
            {
                var estuds = _service.EtudiantesPorLeccion(id);
                // Si "ests" es null significa que algo metieeron mal por ende retorno RequestCode 400
                if (estuds == null)
                    return BadRequest("Entrada invalida");
                // Si todo sale bien se retorna la lista aunque sea vacia y un RequestCode 200
                return Ok (estuds);
            }
            catch (CryptographicException ce)
            {
                _logger.LogError(ce,$"{Utils.GetMail(_userService, this)} -> No se pudo decriptar el id insertado : {id}");
                // Si cae en este catch significa que hubo algo mal en el id de entrada
                // Se retorna un mensaje de error y un RequestCode de 400
                return BadRequest("Entrada Invalida");
            }
            catch (System.Exception e)
            {
                // Si llegamos hasta aca significa que hubo un problema interno no esperado
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, $"{Utils.GetMail(_userService, this)} -> Un error ocurrio durante la obtencion los estudiantes de una leccion en grupo");
                throw;
            }
        }
    }

    public class RequestBodyPublic
    {
        public Leccionpublica lec { get; set; }
        public List<int> est { get; set; }
        public string hour { get; set; }
    }
}