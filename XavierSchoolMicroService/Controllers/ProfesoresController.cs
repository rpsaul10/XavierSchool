using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;


namespace XavierSchoolMicroService.Controllers
{
    [Authorize]
    public class ProfesoresController : ControllerBase
    {
        private readonly IServiceProfesores _service;
        private readonly ILogger<ProfesoresController> _logger;

        public ProfesoresController(IServiceProfesores service, ILogger<ProfesoresController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("api/profesores/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllProfesores()
        {
            _logger.LogInformation($"User -> Intentando obtener la lista de las profesores");
            try
            {
                var teachers = _service.GetAll(0, 90);
                // Si todo sale bien se retorna la lista de profesores aunque sea vacia y un RequestCode 200
                return Ok(teachers);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, $"User -> Error durante la consulta de laos profesores");
                // Si algo sale mal se retornara la excepcion con un RequestCode 500
                throw;
            }
        }

        [HttpGet ("api/profesores/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetProfesor(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener los datos de un profesor con id : {id}");
            try
            {
                var teacher = _service.GetProfesor(id);
                // Si "teacher" es null se retorna un mensaje que inofrma que el maestro no fue encontrado
                // Ademas de un RequestCode de 400
                if (teacher == null)
                    return BadRequest($"El prfesor id: {id} no fue encontrada");
                // Si todo sale bien se retorna la informacion de la presentacion y un RequestCode  200
                return Ok (teacher);
            }
            catch (CryptographicException ce)
            {
                _logger.LogError(ce,$"User -> No se pudo decriptar el id insertado : {id}");
                // Si cae en este catch significa que hubo algo mal en el id de entrada
                // Se retorna un mensaje de error y un RequestCode de 400
                return BadRequest("Entrada Invalida");
            } catch (InvalidOperationException fe)
            {
                _logger.LogError(fe, "User -> Error por cadena demasiado corta");
                // Si cae en este catch significa que hubo algo mal en el id de entrada
                // Se retorna un mensaje de error y un RequestCode de 400
                return BadRequest("Entrada Invalida");
            }
            catch (System.Exception e)
            {
                // Si llegamos hasta aca significa que hubo un problema interno no esperado
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, "User -> Un error ocurrio durante la obtencion del profesor");
                throw;
            }
        }

        [HttpPost ("api/profesores/save")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SaveProfesor([FromBody] Profesore profesor)
        {
            _logger.LogInformation($"User -> Intentando registar un nuevo profesor {profesor}");
            try
            {
                var b = _service.SaveProfesor(profesor);
                // Si todo sale bien se retorna true y un RequestCode 200
                return Ok (b);
            }
            catch (System.Exception e)
            {
                // Si algo sale mal en la insercion caeremos aqui
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, "User -> Un error ocurrio durante el registro del profesor");
                throw;
            }
        }

        [HttpPost ("api/profesores/update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProfesor([FromBody] Profesore profesor, string id)
        {
            _logger.LogInformation($"User -> Intentando registrar un nuevo maestro {profesor}");
            try
            {
                var bo = _service.UpdateProfesor(profesor, id);

                if (bo)
                    return Ok(bo);
                return BadRequest("Nel wey");    
            }
            catch (CryptographicException ce)
            {
                _logger.LogError(ce,$"User -> No se pudo decriptar el id insertado : {id}");
                // Si cae en este catch significa que hubo algo mal en el id de entrada
                // Se retorna un mensaje de error y un RequestCode de 400
                return BadRequest("Entrada Invalida");
            } catch (InvalidOperationException fe)
            {
                _logger.LogError(fe, "User -> Error por cadena demasiado corta");
                // Si cae en este catch significa que hubo algo mal en el id de entrada
                // Se retorna un mensaje de error y un RequestCode de 400
                return BadRequest("Entrada Invalida");
            }
            catch (System.Exception e)
            {
                // Si llegamos hasta aca significa que hubo un problema interno no esperado
                // El error puede ser causado por una fecha mal insertada, verifica bien como mandas los datos
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, "User -> Un error ocurrio durante la actializacion del profesor");
                throw;
            }
        }

        [HttpGet("api/profesores/lecGrupo/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult GetLeccionesGrupoByIdProf(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener las lecciones en grupo del profesor con id {id}");
            try
            {
                var leccciones = _service.GetLeccionesPublicasByIdProf(id);
                // Si "lecciones" es null significa que algo metieeron mal por ende retorno RequestCode 400
                if (leccciones == null)
                    return BadRequest("Entrada invalida");
                // Si todo sale bien se retorna la lista aunque sea vacia y un RequestCode 200
                return Ok (leccciones);    
            }
            catch (CryptographicException ce)
            {
                _logger.LogError(ce,$"User -> No se pudo decriptar el id insertado : {id}");
                // Si cae en este catch significa que hubo algo mal en el id de entrada
                // Se retorna un mensaje de error y un RequestCode de 400
                return BadRequest("Entrada Invalida");
            }
            catch (System.Exception e)
            {
                // Si llegamos hasta aca significa que hubo un problema interno no esperado
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, "User -> Un error ocurrio durante la obtencion de las lecciones en grupo del profesor");
                throw;
            }
        }
        
        [HttpGet ("api/profesores/lecPrivadas/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetLeccionesPrivadasByIdProf(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener las lecciones privadas del profesor con id {id}");
            try
            {
                var lecciones = _service.GetLeccionesPrivadasByIdProf(id);
                // Si "lecciones" es null significa que algo metieeron mal por ende retorno RequestCode 400
                if (lecciones == null)
                    return BadRequest("Entrada invalida");
                // Si todo sale bien se retorna la lista aunque sea vacia y un RequestCode 200
                return Ok(lecciones);    
            }
            catch (CryptographicException ce)
            {
                _logger.LogError(ce,$"User -> No se pudo decriptar el id insertado : {id}");
                // Si cae en este catch significa que hubo algo mal en el id de entrada
                // Se retorna un mensaje de error y un RequestCode de 400
                return BadRequest("Entrada Invalida");
            }
            catch (System.Exception e)
            {
                // Si llegamos hasta aca significa que hubo un problema interno no esperado
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, "User -> Un error ocurrio durante la obtencion de las lecciones privadas que impartio el profesor");
                throw;
            }
        }

        [HttpGet ("api/profesores/presentaciones/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
        public IActionResult GetPresentacionesByIdProf(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener las presentaciones a las que asistio el profesor con : {id}");
            try
            {
                var presentaciones = _service.GetPresentacionesByIdProf(id);
                // Si "presentaciones" es null significa que algo metieeron mal por ende retorno RequestCode 400
                if (presentaciones == null)
                    return BadRequest("Entrada invalida");
                // Si todo sale bien se retorna la lista aunque sea vacia y un RequestCode 200
                return Ok (presentaciones);   
            }
            catch (CryptographicException ce)
            {
                _logger.LogError(ce,$"User -> No se pudo decriptar el id insertado : {id}");
                // Si cae en este catch significa que hubo algo mal en el id de entrada
                // Se retorna un mensaje de error y un RequestCode de 400
                return BadRequest("Entrada Invalida");
            }
            catch (System.Exception e)
            {
                // Si llegamos hasta aca significa que hubo un problema interno no esperado
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, "User -> Un error ocurrio durante la obtencion de las presentaciones que asistio el profesor");
                throw;
            }
        }
    }
}