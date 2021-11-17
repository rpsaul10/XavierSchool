using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;

namespace XavierSchoolMicroService.Controllers
{
    public class LeccionPrivadaController : ControllerBase
    {
        private readonly IServiceLecPrivadas _service;
        
        public LeccionPrivadaController(IServiceLecPrivadas service)
        {
            _service = service;
        }

        [HttpGet("api/lecPrivadas/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IQueryable<Object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllLeccionesPrivada()
        {
            try
            {
                var lecciones = _service.GetAll();
                return Ok (lecciones);    
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet ("api/lecPrivadas/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Leccionprivadum))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetLeccionPrivada(string id)
        {
            try
            {
                var leccion = _service.GetLeccionPrivada(id);

                if (leccion == null)
                    return BadRequest("Id was not found");
                return Ok (leccion);    
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        [HttpPost ("api/lecPrivadas/save")]
        public IActionResult SaveLeccionPrivada([FromBody] RequestBodyPriv requestBodyPriv)
        {
            try
            {
                var res = _service.SaveLeccPrivada(requestBodyPriv.leccion, requestBodyPriv.hora);
                return Ok (res);
            }
            catch (System.Exception)
            {
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