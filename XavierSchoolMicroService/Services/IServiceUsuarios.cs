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
    }
}