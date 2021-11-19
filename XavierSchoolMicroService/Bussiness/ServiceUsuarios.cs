using Microsoft.AspNetCore.DataProtection;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;
using System.Linq;
using XavierSchoolMicroService.Utilities;
using Microsoft.Extensions.Configuration;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceUsuarios : IServiceUsuarios
    {
        private readonly escuela_xavierContext _context;
        private readonly IDataProtector _protector;
        private const string PURPOSE = "UsuariosProvider";
        private readonly IConfiguration _configuration;

        public ServiceUsuarios(escuela_xavierContext context, IDataProtectionProvider provider, IConfiguration configuration)
        {
            _context = context;
            _protector = provider.CreateProtector(PURPOSE);
            _configuration = configuration;
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
    }
}