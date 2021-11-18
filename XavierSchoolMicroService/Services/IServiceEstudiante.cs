using System;
using System.Collections.Generic;
using System.Linq;
using XavierSchoolMicroService.Models;
using System.Threading.Tasks;

namespace XavierSchoolMicroService.Services
{
    public interface IServiceEstudiante
    {
        IQueryable<object> GetAll();
        object GetEstudiante(string id);
        bool SaveEstudiante(Estudiante estudiante);
        bool UpdateEstudiante(string id, Estudiante estudiante);
        IQueryable<string> GetPowersByEstudiante(string id);

        IQueryable<object> GetLeccionesPublicasByIdEstu(string id);

    }
}
