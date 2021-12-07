using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;
using System.Linq;
using XavierSchoolMicroService.Utilities;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceProfesores : IServiceProfesores
    {
        private readonly escuela_xavierContext _context;
        private const string PURPOSE = "ProfesoresProtection";
        private readonly IDataProtector _protector;
        private readonly ILogger<ServiceProfesores> _logger;
        public ServiceProfesores(escuela_xavierContext context, IDataProtectionProvider provider, ILogger<ServiceProfesores> logger)
        {
            _logger = logger;
            _context = context;
            _protector = provider.CreateProtector(PURPOSE);
        }
        public IQueryable<object> GetAll()
        {
            try
            {
                _logger.LogInformation("Obteniendo la lista completa de profesores.");
                var teachers = _context.Profesores.Select(p => CleanProfesorData(p, _protector));
                return teachers;
            }catch (System.Security.Cryptography.CryptographicException ce)
            {
                _logger.LogInformation(ce, $"Error al intentar encriptar los ids.");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogInformation(e, "Error al intentar obtener la lista de profesores");
                throw;
            }
        }

        public object GetProfesor(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo la informacion del profesor con el id : {idStr}");
                var teacher = _context.Profesores.Where(p => p.IdProfesor == int.Parse(idStr)).FirstOrDefault();
                if (teacher == null)
                    return null;
                
                return CleanProfesorData(teacher, _protector);
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
                _logger.LogError(e, "Error al intentar obtener la informacion del profesor.");
                throw;
            }
        }

        public bool SaveProfesor(Profesore profesor)
        {
            try
            {
                _logger.LogInformation($"Registrando la informacion de un nuevo profesor : {profesor}");
                _context.Add(profesor);
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar registrar la informacion del nuevo profesor.");
                throw;
            }
        }

        public bool UpdateProfesor(Profesore prof, string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Actualizando la informacion del profesor con id : {idStr}");
                var oldDtata = _context.Profesores.Where(p => p.IdProfesor == int.Parse(idStr)).FirstOrDefault();
            
                if (oldDtata != null)
                {
                    if (!oldDtata.ApellidoProfesor.Equals(prof.ApellidoProfesor)) oldDtata.ApellidoProfesor = prof.ApellidoProfesor;
                    if (!oldDtata.NombreProfesor.Equals(prof.NombreProfesor)) oldDtata.NombreProfesor = prof.NombreProfesor;
                    if (!oldDtata.FechaNacimientopr.Equals(prof.FechaNacimientopr)) oldDtata.FechaNacimientopr = prof.FechaNacimientopr;
                    if (!oldDtata.NssProfesor.Equals(prof.NssProfesor)) oldDtata.NssProfesor = prof.NssProfesor;
                    if (!oldDtata.ActivoOInactivo.Equals(prof.ActivoOInactivo)) oldDtata.ActivoOInactivo = prof.ActivoOInactivo;

                    _context.SaveChanges();
                    return true;
                }
                return false;
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
                _logger.LogError(e, "Error al intentar actualizar la informacion del profesor.");
                throw;
            }
        }

        public static object CleanProfesorData(Profesore teacher, IDataProtector protector)
        {
            string idProtect = null;
            if (protector != null)
                idProtect = protector.Protect(teacher.IdProfesor.ToString());
            return new {
                IdProfesor = idProtect,
                NombreProfesor = teacher.NombreProfesor,
                ApellidoProfesor = teacher.ApellidoProfesor,
                FechaNacimientopr = teacher.FechaNacimientopr,
                NssProfesor = teacher.NssProfesor,
                ActivoOInactivo = teacher.ActivoOInactivo
            };
        }

        public IQueryable<object> GetLeccionesPublicasByIdProf(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo las lecciones en grupo que impartio el profesor con el id : {idStr}");
                var leccs = from lec in _context.Leccionpublicas
                            join prof in _context.Profesores on lec.FkProfesorLpub equals prof.IdProfesor
                            where prof.IdProfesor == int.Parse(idStr)
                            select ServiceLecPublicas.CleanLecPubliData(lec, prof, null);
                return leccs;
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
                _logger.LogError(e, "Error al intentar obtener las lecciones en grupo que impartio el maestro");
                throw;
            }
        }

        public IQueryable<object> GetLeccionesPrivadasByIdProf(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo las lecciones privadas que impartio el profesor con id :{idStr}");
                var leccs = from lec in _context.Leccionprivada
                            join te in _context.Profesores on lec.FkProfesorLpriv equals te.IdProfesor
                            join es in _context.Estudiantes on lec.FkEstudianteLpriv equals es.IdEstudiante
                            where lec.FkProfesorLpriv == int.Parse(idStr)
                            select ServiceLecPrivadas.CleanLecPrivadaData(lec, te, es, null);
                return leccs;
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
                _logger.LogError(e, "Error al intentar obtener las lecciones privadas que impartio el maestro");
                throw;
            }
        }

        public IQueryable<object> GetPresentacionesByIdProf(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo las presentaciones a las que asistio el profesor con id : {idStr}");
                var pres = from pre in _context.Presentaciones
                            join pre_pro in _context.PresentacionesProfesores on pre.IdPresentacion equals pre_pro.FkPresentacionPres
                            where pre_pro.FkProfesorPres == int.Parse(idStr)
                            select ServicePresentaciones.CleanPresentacionData(pre, null);
                return pres;    
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
                _logger.LogError(e, "Error al intentar obtener las presentaciones a las que asistio el maestro");
                throw;
            }
        }
    }
}