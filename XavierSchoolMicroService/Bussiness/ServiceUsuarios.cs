using Microsoft.AspNetCore.DataProtection;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;
using System.Linq;
using XavierSchoolMicroService.Utilities;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceUsuarios : IServiceUsuarios
    {
        private readonly escuela_xavierContext _context;
        private readonly IDataProtector _protector;
        private const string PURPOSE = "UsuariosProtection";
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<ServiceUsuarios> _logger;

        public ServiceUsuarios(escuela_xavierContext context, IDataProtectionProvider provider,
                                IConfiguration configuration, IOptions<JwtSettings> options, ILogger<ServiceUsuarios> logger)
        {
            _logger = logger;
            _context = context;
            _protector = provider.CreateProtector(PURPOSE);
            _configuration = configuration;
            _jwtSettings = options.Value;
        }
        public bool DeleteUsuario(string id)
        {
            try
            {
                string idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Buscando usuario con id : {idStr} para eliminarlo.");
                var user = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idStr)).FirstOrDefault();

                if (user == null)
                    return false;

                _logger.LogInformation($"Eliminando el usuario con id : {idStr}");
                _context.Usuarios.Remove(user);
                _context.SaveChanges();
                return true; 
            } catch (CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intentar decriptar el id : {id}");
                throw;
            } catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, "Error al intentar convertir cadena a numero");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar eliminar al usuario.");
                throw;
            }
        }

        public IQueryable<object> GetAll()
        {
            try
            {   _logger.LogInformation("Obteniendo la lista competa de usuarios.");
                return _context.Usuarios.Select(u => new {
                    IdUsuario = _protector.Protect(u.IdUsuario.ToString()),
                    Correo = u.Correo,
                    NombreUsuario = u.NombreUsuario,
                    ApellidoUsuario = u.ApellidoUsuario,
                    EstadoAdministrador = u.EstadoAdministrador
                });
            } catch (CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intentar encriptar los ids");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar actualizar la informacion del profesor.");
                throw;
            }
        }

        public object GetUserData(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo la informacion del usuario con el id : {idStr}");
                var user = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idStr)).Select(u => new {
                    IdUsuario = id,
                    Correo = u.Correo,
                    NombreUsuario = u.NombreUsuario,
                    ApellidoUsuario = u.ApellidoUsuario,
                    EstadoAdministrador = u.EstadoAdministrador
                }).FirstOrDefault();
                return user;
            } catch (CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intentar decriptar el id : {id}");
                throw;
            } catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, "Error al intentar convertir cadena a numero");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar obtener la informacion del usuario");
                throw;
            }
        }

        public bool SaveUsuario(Usuario usuario)
        {
            try
            {
                _logger.LogInformation($"Registrando informmacion de nueno usuario {usuario}");
                string key = _configuration.GetSection("KeyEncrypt").Get<string>();
                var encryptPassword = EncryptionClass.Encrypt(usuario.Password, key);
                usuario.Password = encryptPassword;

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                return true;
            } catch (CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intentar encriptar la contraseña");
                throw;
            } catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, "Error al intentar convertir cadena a numero");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar guardar nuevo usuario.");
                throw;
            }
        }

        public bool UpdateUsuario(Usuario usuario, string id)
        {
            try
            {
                string idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Actualizando datos de usuario con id : {idStr}");
                var user = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idStr)).FirstOrDefault();

                if (user == null)
                    return false;
                
                string key = _configuration.GetSection("KeyEncrypt").Get<string>();
                if (!ComparePassword(user.Password, usuario.Password, key)) user.Password = EncryptionClass.Encrypt(usuario.Password, key);
                if (!user.NombreUsuario.Equals(usuario.NombreUsuario)) user.NombreUsuario = usuario.NombreUsuario;
                if (!user.ApellidoUsuario.Equals(usuario.ApellidoUsuario)) user.ApellidoUsuario = usuario.ApellidoUsuario;
                if (!user.Correo.Equals(usuario.Correo)) user.Correo = usuario.Correo;
                if (user.EstadoAdministrador != usuario.EstadoAdministrador) user.EstadoAdministrador = usuario.EstadoAdministrador;

                _context.SaveChanges();
                return true;
            } catch (CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intentar desencriptar el id o encriptar la contraseña");
                throw;
            } catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, "Error al intentar convertir cadena a numero");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar actualizar la informacion del usuario.");
                throw;
            }
        }

        public object AutenticarUsuario(string mail, string password)
        {
            try
            {
                _logger.LogInformation($"Auntenticando usuario con mail : {mail}");
                var key = _configuration.GetSection("KeyEncrypt").Get<string>();
                var user = _context.Usuarios.Where(u => u.Correo == mail).FirstOrDefault();

                if (user == null)
                    return null;
                
                if (!ComparePassword(user.Password, password, key))
                    return null;

                var tokenHandler = new JwtSecurityTokenHandler();
                var key2 = System.Text.Encoding.ASCII.GetBytes (_jwtSettings.Secret);

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity (
                        new Claim [] {
                            new Claim (ClaimTypes.SerialNumber, user.IdUsuario.ToString())
                        }
                    ),
                    Expires = DateTime.Now.AddHours (2),
                    NotBefore = DateTime.Now,
                    SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (key2), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken (tokenDescriptor);
                string finalToken = tokenHandler.WriteToken (token);
            
                return new{
                    NombreUsuario = user.NombreUsuario,
                    ApellidoUsuario = user.ApellidoUsuario,
                    Correo = user.Correo,
                    EstadoAdministrador = user.EstadoAdministrador,
                    Token = finalToken
                };
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error durante la autenticacion");
                throw;
            }
        }

        public static bool ComparePassword(string passwordEncrypt, string password, string key)
        {
            try
            {
                var passDecript = EncryptionClass.Descrypt(passwordEncrypt, key);
                if (passDecript == password)
                    return true;
                return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public bool EsAdministrador(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                var estado = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idStr)).Select(u => u.EstadoAdministrador).FirstOrDefault();

                if (estado == null)
                    return false;
                if (estado == 0)
                    return false;
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public string GetCorreoById(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                return _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idStr)).Select(u => u.Correo).FirstOrDefault();  
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}