using System.Linq;
using XavierSchoolMicroService.Models;

namespace XavierSchoolMicroService.Services
{
    public interface IServiceLecPrivadas
    {
        IQueryable<object> GetAll();
        object GetLeccionPrivada(string id);
        bool SaveLeccPrivada(Leccionprivadum lec, string hour, string idProf, string idEst); 
    }
}