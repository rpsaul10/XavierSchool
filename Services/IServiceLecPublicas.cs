using XavierSchoolMicroService.Models;
using System.Linq;
using System.Collections.Generic;

namespace XavierSchoolMicroService.Services
{
    public interface IServiceLecPublicas
    {
        IQueryable<object> GetAll();
        object GetLecPublica(string id);
        bool SaveLeccPublica(Leccionpublica lec, List<int> estuds, string hour);
        IQueryable<object> EtudiantesPorLeccion(string id);

    }
}