using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;

namespace XavierSchoolMicroService.Controllers
{
    public class PresentacionesController : ControllerBase
    {
        private readonly IServicePresentaciones _service;

        public PresentacionesController(IServicePresentaciones service)
        {
            _service = service;
        }

        [HttpGet ("api/presentaciones/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllPresentaciones()
        {
            try
            {
                var pres = _service.GetAll();
                return Ok (pres);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet ("api/presentaciones/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPresentacion(string id)
        {
            try
            {
                var pre = _service.GetPresentacion(id);
                if (pre == null)
                    return BadRequest("Nel wey");
                return Ok (pre);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost ("api/presentaciones/save")]
        public IActionResult SavePresentacion([FromBody] RequestBodyPresentacion req)
        {
            try
            {
                var ret = _service.SavePresentacion(req.presentacion, req.estudiantes, req.profesores, req.hour);
                return Ok (ret);    
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet ("api/presentaciones/estuds/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetEstudiantesByIdPres(string id)
        {
            try
            {
                var ests = _service.GetEstudiantesById(id);
                return Ok (ests);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet ("api/presentaciones/profes/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProfesoresByIdPres(string id)
        {
            try
            {
                var profesores = _service.GetProfesoresById(id);
                return Ok (profesores);    
            }
            catch (System.Exception)
            {
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