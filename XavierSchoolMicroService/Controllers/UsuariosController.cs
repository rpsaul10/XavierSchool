using System.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using XavierSchoolMicroService.Services;
using XavierSchoolMicroService.Utilities;
using XavierSchoolMicroService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace XavierSchoolMicroService.Controllers
{
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceUsuarios _service;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IConfiguration configuration, IServiceUsuarios service, ILogger<UsuariosController> logger)
        {
            _configuration = configuration;
            _service = service;
            _logger = logger;
        }

        // [HttpGet ("api/{str}")]
        // public IActionResult Cifrar(string str)
        // {
        //     try
        //     {
        //         var key = _configuration.GetSection("KeyEncrypt").Get<string>();

        //         var res = EncryptionClass.Encrypt(str, key);
        //         string[] result = new string[2];
        //         //result[0] = EncryptionClass.Descrypt(res, key);
        //         result[1] = EncryptionClass.Descrypt("holo", key);

        //         return Ok (result);
        //     }
        //     catch (System.Exception)
        //     {
        //         return BadRequest("No se pudo decriptar");
        //         //throw;
        //     }
        // }


        [HttpGet ("api/usuarios/all")]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public IActionResult GetAllUsuarios()
        {
            _logger.LogInformation($"User -> Intentando otener la lista completa de los uuarios");
            try
            {
                var users = _service.GetAll();
                return Ok (users); 
            }
            catch (System.Security.Cryptography.CryptographicException ce)
            {
                _logger.LogInformation(ce, $"User -> Error en la encrisptacionde los ids de los ususarios");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, $"User -> Error durante la consulta de los usuarios");
                throw;
            }
        }

        [HttpPost ("api/usuarios/save")]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public IActionResult SaveUsuarios([FromBody] Usuario usuario)
        {
            _logger.LogInformation($"User -> Intentando registrar un nuevo usuario : {usuario}");
            try
            {
                var resp = _service.SaveUsuario(usuario);
                return Ok (resp);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, $"User -> Error al intentar registar el usuario");
                throw;
            }
        }

        [HttpPost ("api/usuarios/autenticar")]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public IActionResult AutenticarUsuario([FromBody] Usuario user)
        {
            _logger.LogInformation($"User -> Intentando autenticar al usuario {user.Correo}");
            try
            {
                var res = _service.AutenticarUsuario(user.Correo, user.Password);

                if (res == null)
                    return BadRequest("escriba bien, wey");
                return Ok (res); 
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, $"User -> Error al intentar autenticar el usuario");
                throw;
            }
        }

        public IActionResult DeleteUsuario(string id)
        {
            _logger.LogInformation($"User -> Intentando eliminal al usuario con id : {id}");
            try
            {
                var res = _service.DeleteUsuario(id);

                if (!res)
                    return BadRequest("No se encontro el usuario con id : {id}");
                return Ok (res);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, $"User -> Un error ocurrio al intentar eliminar al usuario");
                throw;
            }
        }
    }
}