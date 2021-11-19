using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XavierSchoolMicroService.Services;


namespace XavierSchoolMicroService.Controllers
{
    public class PoderesController : ControllerBase
    {
        private readonly IServicePoderes _service;

        public PoderesController(IServicePoderes service)
        {
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
            try
            {
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