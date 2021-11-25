using System.Linq;

namespace XavierSchoolMicroService.Services
{
    public interface IServicePoderes
    {
        IQueryable<object> GetAll();
        bool SavePoder(string poder);
    }
}