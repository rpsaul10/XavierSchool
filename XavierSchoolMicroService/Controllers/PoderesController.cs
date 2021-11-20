using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XavierSchoolMicroService.Services;


namespace XavierSchoolMicroService.Controllers
{
    //[Authorize]
    public class PoderesController : ControllerBase
    {
        private readonly IServicePoderes _service;
        private readonly IServiceUsuarios _userService;

        public PoderesController(IServicePoderes service, IServiceUsuarios userService)
        {
            _userService = userService;
            _service = service;
        }

        [HttpGet ("api/poderes/all")]
        [ProducesResponseType (StatusCodes.Status200OK)]
        public IActionResult GetAllPoderes()
        {
            try
            {
                var powers = _service.GetAll();
                return Ok (powers);   
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet ("api/poderes/save")]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public IActionResult SavePoder([FromBody] string poder)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var idUser = claimsIdentity.FindFirst(ClaimTypes.SerialNumber)?.Value;
            try
            {
                if (!_userService.EsAdministrador(idUser))
                    return BadRequest("No eres admin");

                var ret = _service.SavePoder(poder);
                return Ok (ret);    
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}