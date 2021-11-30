using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class LeccionPrivadaController : ControllerBase
    {
        private readonly IServiceLecPrivadas _service;
        private readonly ILogger<LeccionPrivadaController> _logger;
        
        public LeccionPrivadaController(IServiceLecPrivadas service, ILogger<LeccionPrivadaController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("api/lecPrivadas/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllLeccionesPrivada()
        {
            _logger.LogInformation($"User -> Intentando obtener la lista de Lecciones en grupo");
            try
            {
                var lecciones = _service.GetAll();
                // Si todo sale bien retorna la lista aunque este vacia y un un codigo RequestCode 200
                return Ok (lecciones);    
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"User -> Error durante la consulta de las lecciones privadas");
                // Si algo sale mal se retornara la excepcion con un RequestCode 500
                throw;
            }
        }

        [HttpGet ("api/lecPrivadas/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetLeccionPrivada(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener los datos de una leccion privada con id : {id}");
            try
            {
                var leccion = _service.GetLeccionPrivada(id);

                // Si "leccion" es null se retorna un mensaje que inofrma que el usuario no fue encontrado
                // Ademas de un RequestCode de 400
                if (leccion == null)
                    return BadRequest($"La leccion privada id: {id} no fue encontrada");
                // Si todo sale bien se retorna la informacion de la leccion y un RequestCode  200
                return Ok (leccion);    
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


        [HttpPost ("api/lecPrivadas/save")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SaveLeccionPrivada([FromBody] RequestBodyPriv requestBodyPriv)
        {
            _logger.LogInformation($"Usuario -> Intentando registrar una nueva leccion privada : {requestBodyPriv.leccion}");
            try
            {
                var res = _service.SaveLeccPrivada(requestBodyPriv.leccion, requestBodyPriv.hora);
                // Si todo sale bien se retorna true y un RequestCode 200
                return Ok (res);
            }
            catch (System.Exception e)
            {
                // Si algo sale mal en la insercion caeremos aqui
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, "User -> Un error ocurrio durante el registro de la leccion en grupo");
                throw;
            }
        }

        public class RequestBodyPriv
        {
            public Leccionprivadum leccion { get; set; }
            public string hora { get; set; }
        }
    }
}