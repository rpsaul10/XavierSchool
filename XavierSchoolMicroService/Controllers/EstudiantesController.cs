using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;

namespace XavierSchoolMicroService.Controllers
{
    [Authorize]
    public class EstudiantesController : ControllerBase
    {
        private readonly IServiceEstudiante _service;
        private readonly ILogger<EstudiantesController> _logger;

        public EstudiantesController(IServiceEstudiante service, ILogger<EstudiantesController> logger)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("api/estudiantes/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllEstudiantes()
        {
            _logger.LogInformation($"User -> Intentando obtener la lista de estudiantes");
            try
            {
                var estudiantes = _service.GetAll();
                // Si todo sale bien retorna la lista aunque este vacia y un un codigo RequestCode 200
                return Ok (estudiantes);
            } catch (Exception e)
            {
                _logger.LogError(e, $"User -> Error durante la consulta de los estudiante");
                // Si algo sale mal se retornara la excepcion con un RequestCode 500
                throw;
            }
        }

        [HttpGet ("api/estudiantes/{id}&{mode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Estudiante))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetEstudiante(string id, byte mode)
        {
            try
            {
                _logger.LogInformation($"User -> Intentando obtener los datos del estudiante con id : {id}");
                var e = _service.GetEstudiante(id, mode);
                
                // Si todo sale bien retorn Ok 200
                if (e != null)
                {
                    return Ok (e);
                }
                // Si "e" es null se retorna un mensaje que inofrma que el usuario no fue encontrado
                // Ademas de un RequestCode de 400
                return BadRequest($"El estudiante con id: {id} no fue encontrado");
            } catch (CryptographicException ce)
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
                _logger.LogError(e, "User -> Un error ocurrio durante la consulta del estudiante");
                throw;
            }
        }

        [HttpPost ("api/estudiantes/save")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SaveEstudiante([FromBody] RequestForEstudiante req)
        {
            try
            {
                _logger.LogInformation($"Usuario -> Intentando registrar un nuevo estudiante : {req.estudiante}");
                var b = _service.SaveEstudiante(req.estudiante, req.powers); // Falta quitar null
                // Si todo sale bien re retorna un true y un RequestCode 200
                return Ok(b);
            }
            catch (System.Exception e)
            {
                // Si algo sale mal en la insercion caeremos aqui
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, "User -> Un error ocurrio durante el registro del estudiante");
                throw;
            }
        }


        [HttpPost ("api/estudiante/update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateEstudainte([FromBody] RequestForEstudiante req, string id)
        {
            _logger.LogInformation($"User -> Intentando guardar un nuevo estudiante {req.estudiante}");
            try
            {
                var result = _service.UpdateEstudiante(id, req.estudiante, req.powers);

                if (result)
                    return Ok (result);
                return BadRequest("No se encontro el id del estudiante");
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
                _logger.LogError(e, "User -> Un error ocurrio durante la actializacion del estudiante");
                throw;
            }
        }

        [HttpGet("api/estudiantes/poderes/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPowersByEstudiante(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener los poderes del estudiante con id {id}");
            try
            {
                var pow = _service.GetPowersByEstudiante(id);
                // Si "pow" es null significa que algo metieeron mal por ende retorno RequestCode 400
                if (pow == null)
                    return BadRequest("Entrada invalida");
                // Si todo sale bien se retorna la lista aunque sea vacia y un RequestCode 200
                return Ok(pow);
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
                _logger.LogError(e, "User -> Un error ocurrio durante la obtencion de poderes por un id de estudiante");
                throw;
            }
        }

        [HttpGet("api/estudiantes/lecGrupo/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetLeccionesGrupoByIdEstu(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener las lecciones en grupo del estudiante con : {id}");
            try
            {
                var lecc = _service.GetLeccionesPublicasByIdEstu(id);

                // Si "lecc" es null significa que algo metieron mal por ende retorno RequestCode 400
                if (lecc == null)
                    return BadRequest("Entrada invalida");
                // Si todo sale bien se retorna la lista de lecciones aunque sea vacia y un RequestCode 200
                return Ok (lecc);    
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
                _logger.LogError(e, "User -> Un error ocurrio durante la obtencion de lecciones en grupo por un id de estudiante");
                throw;
            }
        }

        [HttpGet ("api/estudiantes/lecPrivadas/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetLeccionesPrivadasByIdEstu(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener las lecciones privadas del estudiante con: {id}");
            try
            {
                var less = _service.GetLeccionesPrivadasByIdEstu(id);
                // Si "less" es null significa que algo metieron mal por ende retorno RequestCode 400
                if (less == null)
                    return BadRequest("Entrada invalida");
                // Si todo sale bien se retorna la lista de lecciones aunque sea vacia y un RequestCode 200
                return Ok (less);
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
                _logger.LogError(e, "User -> Un error ocurrio durante la obtencion de las lecciones privadas por un id de estudiante");
                throw;
            }
        }

        [HttpGet ("api/estudiantes/presentaciones/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
         [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPresentacionesByIdEstu(string id)
        {
            _logger.LogInformation($"User -> Intentando obtener las presentaciones del estudiante con: {id}");
            try
            {
                var pres = _service.GetPresentacionesByIdEstu(id);
                // Si "press" es null significa que algo metieron mal por ende retorno RequestCode 400
                if (pres == null)
                    return BadRequest("Entrada invalida");
                // Si todo sale bien se retorna la lista de presentaciones aunque sea vacia y un RequestCode 200
                return Ok (pres);    
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
                _logger.LogError(e, "User -> Un error ocurrio durante la obtencion de las presentaciones por un id de estudiante");
                throw;
            }
        }


        [HttpGet ("api/niveles/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetNiveles()
        {
            try
            {
                var res = ((Bussiness.ServiceEstudiante) _service).GetNiveles();
                return Ok (res);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }

    public class RequestForEstudiante
    {
        public Estudiante estudiante { get; set; }
        public List<int> powers { get; set; }
    }
}
