using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;

namespace XavierSchoolMicroService.Controllers
{
    public class PresentacionesController : ControllerBase
    {
        private readonly IServicePresentaciones _service;
        private readonly ILogger<PresentacionesController> _logger;

        public PresentacionesController(IServicePresentaciones service, ILogger<PresentacionesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet ("api/presentaciones/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllPresentaciones()
        {
            _logger.LogInformation($"User -> Intentando obtener la lista de las presentaciones");
            try
            {
                var pres = _service.GetAll();
                // Si todo sale bien se retorna la lsita de presentaciones aunque sea vacia y un RequestCode 200
                return Ok (pres);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, $"User -> Error durante la consulta de las presentaciones");
                // Si algo sale mal se retornara la excepcion con un RequestCode 500
                throw;
            }
        }

        [HttpGet ("api/presentaciones/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPresentacion(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener los datos de una presentacion con id : {id}");
            try
            {
                var pre = _service.GetPresentacion(id);
                // Si "pre" es null se retorna un mensaje que inofrma que el maestro no fue encontrado
                // Ademas de un RequestCode de 400
                if (pre == null)
                    return BadRequest($"La presentacion id: {id} no fue encontrada");
                // Si todo sale bien se retorna la informacion de la presentacion y un RequestCode  200
                return Ok (pre);
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
                _logger.LogError(e, "User -> Un error ocurrio durante la obtencion de la leccion privada");
                throw;
            }
        }

        [HttpPost ("api/presentaciones/save")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SavePresentacion([FromBody] RequestBodyPresentacion req)
        {
            _logger.LogInformation($"Usuario -> Intentando registrar una nueva presentacion : {req.presentacion}");
            try
            {
                var ret = _service.SavePresentacion(req.presentacion, req.estudiantes, req.profesores, req.hour);
                // Si todo sale bien se retorna true y un RequestCode 200
                return Ok (ret);    
            }
            catch (System.Exception e)
            {
                // Si algo sale mal en la insercion caeremos aqui
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, "User -> Un error ocurrio durante el registro de la leccion en grupo");
                throw;
            }
        }

        [HttpGet ("api/presentaciones/estuds/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetEstudiantesByIdPres(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener los estudiantes de una presentacion con el id : {id}");
            try
            {
                var ests = _service.GetEstudiantesById(id);
                // Si "ests" es null significa que algo metieeron mal por ende retorno RequestCode 400
                if (ests == null)
                    return BadRequest("Entrada invalida");
                // Si todo sale bien se retorna la lista aunque sea vacia y un RequestCode 200
                return Ok (ests);
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
                _logger.LogError(e, "User -> Un error ocurrio durante la obtencion los estudiantes de presentacion");
                throw;
            }
        }

        [HttpGet ("api/presentaciones/profes/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProfesoresByIdPres(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener los profesores de una presentacion con el id : {id}");
            try
            {
                var profesores = _service.GetProfesoresById(id);
                // Si "profesores" es null significa que algo metieeron mal por ende retorno RequestCode 400
                if (profesores == null)
                    return BadRequest("Entrada invalida");
                // Si todo sale bien se retorna la lista aunque sea vacia y un RequestCode 200
                return Ok (profesores);    
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
                _logger.LogError(e, "User -> Un error ocurrio durante la obtencion los profesores de presentacion");
                throw;
            }
        }
    }

    public class RequestBodyPresentacion
    {
        public Presentacione presentacion { get; set; }
        public List<int[]> estudiantes { get; set; }
        public List<int> profesores { get; set; }
        public string hour { get; set; }
    }
}