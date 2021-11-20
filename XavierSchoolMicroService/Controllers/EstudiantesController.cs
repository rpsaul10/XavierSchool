using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;
using XavierSchoolMicroService.Utilities;

namespace XavierSchoolMicroService.Controllers
{
    //[Authorize]
    public class EstudiantesController : ControllerBase
    {
        private readonly IServiceEstudiante _service;

        public EstudiantesController(IServiceEstudiante service)
        {
            _service = service;
        }

        [HttpGet("api/estudiantes/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IQueryable<Object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllEstudiantes()
        {
            try
            {
                var estudiantes = _service.GetAll();
                return Ok(estudiantes);
            } catch (Exception)
            {
                throw;
            }
        }

        [HttpGet ("api/estudiantes/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Estudiante))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetEstudiante(string id)
        {
            try
            {
                var e = _service.GetEstudiante(id);
                
                if (e != null)
                {
                    return Ok (e);
                }
                return BadRequest("Student Id was not found.");
            }
            catch (System.Exception)
            {
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
                var b = _service.SaveEstudiante(req.estudiante, req.powers); // Falta quitar null
                return Ok(b);
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        [HttpPost ("api/estudiante/update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateEstudainte([FromBody] RequestForEstudiante req, string id)
        {
            try
            {
                var result = _service.UpdateEstudiante(id, req.estudiante, req.powers);

                if (result)
                    return Ok ();
                return BadRequest("Esta mal wey");
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet("api/estudiantes/poderes/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IQueryable<Object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPowersByEstudiante(string id)
        {
            try
            {
                var pow = _service.GetPowersByEstudiante(id);
                return Ok(pow);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet("api/estudiantes/lecGrupo/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IQueryable<Object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetLeccionesGrupoByIdEstu(string id)
        {
            try
            {
                var lecc = _service.GetLeccionesPublicasByIdEstu(id);
                return Ok (lecc);    
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet ("api/estudiantes/lecPrivadas/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetLeccionesPrivadasByIdEstu(string id)
        {
            try
            {
                var less = _service.GetLeccionesPrivadasByIdEstu(id);
                return Ok (less);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet ("api/estudiantes/presentaciones/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPresentacionesByIdEstu(string id)
        {
            try
            {
                var pres = _service.GetPresentacionesByIdEstu(id);
                return Ok (pres);    
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
