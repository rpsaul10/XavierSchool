using System.Linq;
using XavierSchoolMicroService.Models;


namespace XavierSchoolMicroService.Services
{
    public interface IServiceUsuarios
    {
        IQueryable<object> GetAll();
        object GetUserData(string id);
        bool SaveUsuario(Usuario usuario);
        bool UpdateUsuario(Usuario usuario, string id);
        bool DeleteUsuario(string id);
        object AutenticarUsuario(string mail, string password);
        bool EsAdministrador(string id);
    }
}