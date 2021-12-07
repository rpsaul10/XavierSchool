using System.Linq;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;
using XavierSchoolMicroService.Utilities;
using System.Security.Cryptography;
using System;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceLecPrivadas : IServiceLecPrivadas
    {
        private readonly escuela_xavierContext _context;
        private readonly IDataProtector _protector;
        private const string PURPOSE = "LeccionesPrivadasProtection";
        private const string PURPUSE_EST = "EstudiantesProtection";
        private const string PROPUSE_PROF = "ProfesoresProtection";
        private readonly IDataProtector _protector_est;
        private readonly IDataProtector _protector_prof;
        private readonly ILogger<ServiceLecPrivadas> _logger;
        public ServiceLecPrivadas(escuela_xavierContext context, IDataProtectionProvider provider, ILogger<ServiceLecPrivadas> logger)
        {
            _logger = logger;
            _context = context;
            _protector_est = provider.CreateProtector(PURPUSE_EST);
            _protector = provider.CreateProtector(PURPOSE);
            _protector_prof = provider.CreateProtector(PROPUSE_PROF);
        }
        public IQueryable<object> GetAll()
        {
            try
            {
                _logger.LogInformation("Obteniendo la lista de  todas las lecciones privadas");
                var lePri = from lec in _context.Leccionprivada
                            join te in _context.Profesores on lec.FkProfesorLpriv equals te.IdProfesor
                            join es in _context.Estudiantes on lec.FkEstudianteLpriv equals es.IdEstudiante
                            select CleanLecPrivadaData(lec, te, es, _protector);
                return lePri;
            } catch (CryptographicException ce)
            {
                _logger.LogError(ce, "Error al intentar encriptar los ids.");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar obtener la lista de todas las lecciones privadas");
                throw;
            }
        }

        public object GetLeccionPrivada(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo la leccion privada con el id : {idStr}");
                var lePri = from lec in _context.Leccionprivada
                            join te in _context.Profesores on lec.FkProfesorLpriv equals te.IdProfesor
                            join es in _context.Estudiantes on lec.FkEstudianteLpriv equals es.IdEstudiante
                            where lec.IdLeccionpriv == int.Parse(idStr)
                            select CleanLecPrivadaData(lec, te, es, _protector);  

                if (lePri.Count() == 0)
                    return null;
                return lePri.First();
            } catch (CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intentar decriptar el id : {id}");
                throw;
            } catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, "Error al intentar convertir cadena anumero");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar obtener la leccion privada.");
                throw;
            }
        }

        public bool SaveLeccPrivada(Leccionprivadum lec, string hour, string idProf, string idEst)
        {
            try
            {
                _logger.LogInformation($"Registrando la informacion de la leccion privada : {lec} hour: {hour}");
                lec.FkProfesorLpriv = int.Parse(idProf.Length > Utils.LENT ? _protector_prof.Unprotect(idProf) : idProf);
                lec.FkEstudianteLpriv = int.Parse(idEst.Length > Utils.LENT ? _protector_est.Unprotect(idEst) : idEst);
                lec.HoraLeccionpriv = Utilities.Utils.ConvertirHoraToTimeSpan(hour);
                _context.Leccionprivada.Add(lec);
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar guardar la leccion privada.");
                throw;
            }
        }

        public static object CleanLecPrivadaData(Leccionprivadum lec, Profesore te, Estudiante es, IDataProtector protector)
        {
            // Falta
            string idProtect = null;
            if (protector != null)
                idProtect = protector.Protect(lec.IdLeccionpriv.ToString());
            return new {
                IdLeccionpriv = idProtect,
                NombreLeccionpriv = lec.NombreLeccionpriv,
                HoraLeccionpriv = Utilities.Utils.ConvertirTimeSpanToStringHora(lec.HoraLeccionpriv),
                FechaLeccionpriv = lec.FechaLeccionpriv,
                Profesor = $"{te.NombreProfesor} {te.ApellidoProfesor}",
                Estudiante = $"{es.NombreEstudiante} {es.ApellidoEstudiante}"
            };
        }
    }
}