
using System.Collections.Generic;
using System.Linq;
using XavierSchoolMicroService.Models;

namespace XavierSchoolMicroService.Services
{
    public interface IServiceProfesores
    {
        IQueryable<object> GetAll();
        object GetProfesor(string id);
        bool SaveProfesor(Profesore profesor);
        bool UpdateProfesor(Profesore profesore, string id);
        IQueryable<object> GetLeccionesPublicasByIdProf(string id);
        IQueryable<object> GetLeccionesPrivadasByIdProf(string id);
        IQueryable<object> GetPresentacionesByIdProf(string id);
    }
}