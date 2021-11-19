using System.Linq;
using XavierSchoolMicroService.Models;

namespace XavierSchoolMicroService.Services
{
    public interface IServicePoderes
    {
        IQueryable<object> GetAll();
        bool SavePoder(string poder);
    }
}