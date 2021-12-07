using XavierSchoolMicroService.Models;
using System.Linq;
using System.Collections.Generic;

namespace XavierSchoolMicroService.Services
{
    public interface IServicePresentaciones
    {
        IQueryable<object> GetAll();
        object GetPresentacion(string id);
        bool SavePresentacion(Presentacione presentacion, List<string[]> idEstus, List<string> idProfs, string hora);
        IQueryable<object> GetEstudiantesById(string id);
        IQueryable<object> GetProfesoresById(string id);
    }
}