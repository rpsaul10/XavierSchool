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

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceUsuarios : IServiceUsuarios
    {
        private readonly escuela_xavierContext _context;
        private readonly IDataProtector _protector;
        private const string PURPOSE = "UsuariosProtection";
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;

        public ServiceUsuarios(escuela_xavierContext context, IDataProtectionProvider provider,
                                IConfiguration configuration, IOptions<JwtSettings> options)
        {
            _context = context;
            _protector = provider.CreateProtector(PURPOSE);
            _configuration = configuration;
            _jwtSettings = options.Value;
        }
        public bool DeleteUsuario(string id)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<object> GetAll()
        {
            try
            {    
                return _context.Usuarios.Select(u => new {
                    IdUsuario = _protector.Protect(u.IdUsuario.ToString()),
                    Correo = u.Correo,
                    NombreUsuario = u.NombreUsuario,
                    ApellidoUsuario = u.ApellidoUsuario,
                    EstadoAdministrador = u.EstadoAdministrador
                });
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public object GetUserData(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;

                var user = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(idStr)).Select(u => new {
                    IdUsuario = id,
                    Correo = u.Correo,
                    NombreUsuario = u.NombreUsuario,
                    ApellidoUsuario = u.ApellidoUsuario,
                    EstadoAdministrador = u.EstadoAdministrador
                }).FirstOrDefault();
                return user;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public bool SaveUsuario(Usuario usuario)
        {
            try
            {
                string key = _configuration.GetSection("KeyEncrypt").Get<string>();
                var encryptPassword = EncryptionClass.Encrypt(usuario.Password, key);
                usuario.Password = encryptPassword;

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public bool UpdateUsuario(Usuario usuario, string id)
        {
            throw new System.NotImplementedException();
        }

        public object AutenticarUsuario(string mail, string password)
        {
            try
            {
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
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
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
                var estado = _context.Usuarios.Where(u => u.IdUsuario == int.Parse(id)).Select(u => u.EstadoAdministrador).FirstOrDefault();

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
            throw new NotImplementedException();
        }
    }
}