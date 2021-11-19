using System.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using XavierSchoolMicroService.Services;
using XavierSchoolMicroService.Utilities;
using XavierSchoolMicroService.Models;

namespace XavierSchoolMicroService.Controllers
{
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceUsuarios _service;

        public UsuariosController(IConfiguration configuration, IServiceUsuarios service)
        {
            _configuration = configuration;
            _service = service;
        }

        [HttpGet ("api/{str}")]
        public IActionResult Cifrar(string str)
        {
            try
            {
                var key = _configuration.GetSection("KeyEncrypt").Get<string>();

                var res = EncryptionClass.Encrypt(str, key);
                string[] result = new string[2];
                //result[0] = EncryptionClass.Descrypt(res, key);
                result[1] = EncryptionClass.Descrypt("holo", key);

                return Ok (result);
            }
            catch (System.Exception)
            {
                return BadRequest("No se pudo decriptar");
                //throw;
            }
        }


        [HttpGet ("api/usuarios/all")]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public IActionResult GetAllUsuarios()
        {
            try
            {
                var users = _service.GetAll();
                return Ok (users); 
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost ("api/usuarios/save")]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public IActionResult SaveUsuarios([FromBody] Usuario usuario)
        {
            try
            {
                var resp = _service.SaveUsuario(usuario);
                return Ok (resp);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}