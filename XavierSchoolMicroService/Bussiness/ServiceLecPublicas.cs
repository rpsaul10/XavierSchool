using System.Linq;
using XavierSchoolMicroService.Services;
using XavierSchoolMicroService.Models;
using System.Collections.Generic;
using XavierSchoolMicroService.Utilities;
using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceLecPublicas : IServiceLecPublicas
    {
        private readonly escuela_xavierContext _context;
        private const string PURPOSE = "LeccionesPublicasProtection";
        private const string PURPOSE_EST = "EstudiantesProtection";
        private const string PROPUSE_PROF = "ProfesoresProtection";
        private readonly IDataProtector _protector;
        private readonly IDataProtector _protector_est;
        private readonly IDataProtector _protector_prof;
        private readonly ILogger<ServiceLecPrivadas> _logger;
        public ServiceLecPublicas(escuela_xavierContext context, IDataProtectionProvider provider, ILogger<ServiceLecPrivadas> logger)
        {
            _logger = logger;
            _protector = provider.CreateProtector(PURPOSE);
            _protector_est = provider.CreateProtector(PURPOSE_EST);
            _protector_prof = provider.CreateProtector(PROPUSE_PROF);
            _context = context;
        }
        public IQueryable<object> EtudiantesPorLeccion(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo los estudiantes que asistieron a la leccion con id : {idStr}");
                var estuds = from es in _context.Estudiantes
                            join es_le in _context.LeccionesEstudiantes on es.IdEstudiante equals es_le.FkEstudianteLec
                            join ni in _context.Nivelpoders on es.FkNivelpoderEst equals ni.IdNivel
                            where es_le.FkLeccionEst == int.Parse(idStr)
                            select ServiceEstudiante.CleanEstudianteData(es, ni, null);
                return estuds;
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
                _logger.LogError(e, "Error al intentar obtener la leccion en grupo.");
                throw;
            }
        }

        public IQueryable<object> GetAll()
        {
            try
            {
                _logger.LogInformation("Obteniendo la lista de todas las lecciones en grupo");
                var lecciones = from lec in _context.Leccionpublicas
                                join teach in _context.Profesores on lec.FkProfesorLpub equals teach.IdProfesor
                                select CleanLecPubliData(lec, teach, _protector);

                return lecciones;
            } catch (CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intentar encriptar los ids");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar obtener la lista de lecciones en grupo.");
                throw;
            }
        }

        public object GetLecPublica(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo la informacion de la leccion en grupo con el id : {idStr}");
                var lecciones = from lec in _context.Leccionpublicas
                                join teach in _context.Profesores on lec.FkProfesorLpub equals teach.IdProfesor
                                where lec.IdLeccionpub == int.Parse(idStr)
                                select CleanLecPubliData(lec, teach, _protector);
                
                if (lecciones.Count() == 0)
                    return null;
                return lecciones.First();
            } catch (CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intentar decriptar el id : {id}");
                throw;
            } catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, "Error al intentar convertir cadena a numero");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar obtener la leccion en grupo.");
                throw;
            }
        }

        public static object CleanLecPubliData(Leccionpublica lec, Profesore teach, IDataProtector protector)
        {
            string idProtect = null;
            if (protector != null)
                idProtect = protector.Protect(lec.IdLeccionpub.ToString());
            return new {
                        IdLeccionpub = idProtect,
                        NombreLeccionpub = lec.NombreLeccionpub,
                        HoraLeccionpub = Utils.ConvertirTimeSpanToStringHora(lec.HoraLeccionpub),
                        FechaLeccionpu = lec.FechaLeccionpu,
                        MaestroLeccionP = $"{teach.NombreProfesor} {teach.ApellidoProfesor}"
                    };
        }

        public bool SaveLeccPublica(Leccionpublica lec, List<string> estuds, string hour, string idProf)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                _logger.LogInformation($"Registrando la informacion de la leccion en grupo : {lec} hour: {lec}");
                lec.HoraLeccionpub = Utils.ConvertirHoraToTimeSpan(hour);
                lec.FkProfesorLpub = int.Parse(idProf.Length > Utils.LENT ? _protector_prof.Unprotect(idProf) : idProf);
                _context.Leccionpublicas.Add(lec);
                _context.SaveChanges();
                var recent = _context.Leccionpublicas.OrderBy(l => l.IdLeccionpub).LastOrDefault();
                
                foreach(var id in estuds)
                {
                    _context.LeccionesEstudiantes.Add( new LeccionesEstudiante
                    {
                        FkEstudianteLec = int.Parse(id.Length > Utils.LENT ? _protector_est.Unprotect(id) : id),
                        FkLeccionEst = recent.IdLeccionpub
                    });
                }

                _context.SaveChanges();
                transaction.Commit();
                return true;
            } 
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar registrar la leccion en grupo.");
                throw;
            }
        }
    }
}