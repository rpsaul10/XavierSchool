using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServicePoderes : IServicePoderes
    {
        private readonly escuela_xavierContext _context;
        private readonly ILogger<ServicePoderes> _logger;
        public ServicePoderes(escuela_xavierContext context, ILogger<ServicePoderes> logger)
        {
            _logger = logger;
            _context = context;
        }
        public IQueryable<object> GetAll()
        {
            try
            {
                _logger.LogInformation("Obteniendo la lista completa de poderes");
                var powers = _context.Poderes.Select(p => CleanPoderesData(p));
                return powers;
            } catch (CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intentar encriptar los ids");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar obtener la lista de poderes.");
                throw;
            }
        }

        public bool SavePoder(string poder)
        {
            try
            {
                _logger.LogInformation($"Reigistrando informacion del nuevo poder : {poder}");
                _context.Poderes.Add(new Podere{
                    NombrePoder = poder
                });
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception e)
            {
                _logger.LogInformation(e, $"Error al intentar guardar el nuevo poder");
                throw;
            }
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