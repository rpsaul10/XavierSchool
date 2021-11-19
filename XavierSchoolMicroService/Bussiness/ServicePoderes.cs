using System.Linq;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServicePoderes : IServicePoderes
    {
        private readonly escuela_xavierContext _context;

        public ServicePoderes(escuela_xavierContext context)
        {
            _context = context;
        }
        public IQueryable<object> GetAll()
        {
            try
            {  
                var powers = _context.Poderes.Select(p => CleanPoderesData(p));
                return powers;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public bool SavePoder(string poder)
        {
            try
            {
                _context.Poderes.Add(new Podere{
                    NombrePoder = poder
                });
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public static object CleanPoderesData(Podere poder)
        {
            return new {
                    IdPoder = poder.IdPoder,
                    NombrePoder = poder.NombrePoder
                };
        }
    }
}