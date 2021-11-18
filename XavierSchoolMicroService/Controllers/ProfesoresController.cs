using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;


namespace XavierSchoolMicroService.Controllers
{
    public class ProfesoresController : ControllerBase
    {
        private readonly IServiceProfesores _service;

        public ProfesoresController(IServiceProfesores service)
        {
            _service = service;
        }

        [HttpGet("api/profesores/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IQueryable<Object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllEstudiantes()
        {
            try
            {
                var teachers = _service.GetAll(0, 90);
                return Ok(teachers);
            } catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet ("api/profesores/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Estudiante))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProfesor(string id)
        {
            try
            {
                var teacher = _service.GetProfesor(id);
                if (teacher != null)
                    return Ok (teacher);
                return BadRequest("Teacher Id was not found.");
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost ("api/profesores/save")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SaveProfesor([FromBody] Profesore profesor)
        {
            try
            {
                var b = _service.SaveProfesor(profesor);
                return Ok (b);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost ("api/profesores/update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProfesor([FromBody] Profesore profesor, string id)
        {
            try
            {
                var bo = _service.UpdateProfesor(profesor, id);

                if (bo)
                    return Ok(bo);
                return BadRequest("Nel wey");    
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet("api/profesores/lecGrupo/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IQueryable<Object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult GetLeccionesGrupoByIdProf(string id)
        {
            try
            {
                var leccciones = _service.GetLeccionesPublicasByIdProf(id);
                return Ok (leccciones);    
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}