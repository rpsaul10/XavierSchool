using XavierSchoolMicroService.Models;
using System.Linq;
using System.Collections.Generic;

namespace XavierSchoolMicroService.Services
{
    public interface IServicePresentaciones
    {
        IQueryable<object> GetAll();
        object GetPresentacion(string id);
        bool SavePresentacion(Presentacione presentacion, List<int[]> idEstus, List<int> idProfs, string hora);
        IQueryable<object> GetEstudiantesById(string id);
        IQueryable<object> GetProfesoresById(string id);
    }
}