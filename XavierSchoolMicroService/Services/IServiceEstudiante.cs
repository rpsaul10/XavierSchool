using System.Collections.Generic;
using System.Linq;
using XavierSchoolMicroService.Models;

namespace XavierSchoolMicroService.Services
{
    public interface IServiceEstudiante
    {
        IQueryable<object> GetAll();
        object GetEstudiante(string id);
        bool SaveEstudiante(Estudiante estudiante, List<int> powerList);
        bool UpdateEstudiante(string id, Estudiante estudiante, List<int> powerList);
        IQueryable<object> GetPowersByEstudiante(string id);
        IQueryable<object> GetLeccionesPublicasByIdEstu(string id);
        IQueryable<object> GetLeccionesPrivadasByIdEstu(string id);
        IQueryable<object> GetPresentacionesByIdEstu(string id);
    }
}
