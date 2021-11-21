using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XavierSchoolMicroService.Services;


namespace XavierSchoolMicroService.Controllers
{
    //[Authorize]
    public class PoderesController : ControllerBase
    {
        private readonly IServicePoderes _service;
        private readonly IServiceUsuarios _userService;
        private readonly ILogger<PoderesController> _logger;

        public PoderesController(IServicePoderes service, IServiceUsuarios userService, ILogger<PoderesController> logger)
        {
            _logger = logger;
            _userService = userService;
            _service = service;
        }

        [HttpGet ("api/poderes/all")]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllPoderes()
        {
            _logger.LogInformation($"User -> Intentando obtener la lista de todos los poderes");
            try
            {
                var powers = _service.GetAll();
                // Si todo sale bien se retorna la lsiat de poderes aunque este vacia y un RequestCode 200
                return Ok (powers);   
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, $"User -> Error durante la consulta de los poderes");
                // Si algo sale mal se retornara la excepcion con un RequestCode 500
                throw;
            }
        }

        [HttpGet ("api/poderes/save")]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public IActionResult SavePoder([FromBody] string poder)
        {
            // var claimsIdentity = this.User.Identity as ClaimsIdentity;
            // var idUser = claimsIdentity.FindFirst(ClaimTypes.SerialNumber)?.Value;
            _logger.LogInformation($"User -> Intentando guardar un nuevo poder {poder}");
            try
            {
                // if (!_userService.EsAdministrador(idUser))
                //     return BadRequest("No eres admin");

                var ret = _service.SavePoder(poder);
                return Ok (ret);    
            }
            catch (System.Exception e)
            {
                // Si algo sale mal en la insercion caeremos aqui
                // Se retorna la excepcion y un RequestCode de 500
                _logger.LogError(e, "User -> Un error ocurrio durante el registro del poder");
                throw;
            }
        }
    }
}