using System.Linq;
using XavierSchoolMicroService.Services;
using XavierSchoolMicroService.Models;
using System.Collections.Generic;
using XavierSchoolMicroService.Utilities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServicePresentaciones : IServicePresentaciones
    {
        private readonly escuela_xavierContext _context;
        private const string PURPOSE = "PresentacionesProtection";
        private readonly IDataProtector _protector;
        private readonly ILogger<ServicePresentaciones> _logger;

        public ServicePresentaciones(escuela_xavierContext context, IDataProtectionProvider provider, ILogger<ServicePresentaciones> logger)
        {
            _logger = logger;
            _context = context;
            _protector = provider.CreateProtector(PURPOSE);
        }

        public IQueryable<object> GetAll()
        {
            try
            {
               _logger.LogInformation("Obteniendo la lista completa de presentaciones");
               var pres = _context.Presentaciones.Select(p => CleanPresentacionData(p, _protector));
               return pres;
            } catch (CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intentar encriptar los ids");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar obtener la lista de presentaciones.");
                throw;
            }
        }

        public IQueryable<object> GetEstudiantesById(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo la lista de estudiantes que asistieron a la presentacion con id: {idStr}");
                var ests = from es in _context.Estudiantes
                        join es_tr in _context.PresentacionesEstudiantes on es.IdEstudiante equals es_tr.FkEstudiantePres
                        join niv in _context.Nivelpoders on es.FkNivelpoderEst equals niv.IdNivel
                        join dor in _context.Dormitorios on es.FkDormitorioEst equals dor.IdDormitorio
                        where es_tr.FkPresentacionEst == int.Parse(idStr)
                        select new {
                            Estudiante = ServiceEstudiante.CleanEstudianteData(es, dor, niv, null),
                            EstadoPresentacion = es_tr.EstadoPresentacion
                        };
                return ests;    
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
                _logger.LogError(e, "Error al intentar obtener los estudiantes de la presentacion.");
                throw;
            }
        }

        public object GetPresentacion(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo la informacion de la presentacion con id: {idStr}");
                var pre = _context.Presentaciones.Where(p => p.IdPresentacion == int.Parse(idStr)).FirstOrDefault();
                if (pre == null)
                    return null;
                return CleanPresentacionData(pre, _protector);
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
                _logger.LogError(e, "Error al intentar obtener la informacion de la presentacion.");
                throw;
            }
        }

        public IQueryable<object> GetProfesoresById(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo los profesores que asistieron a la presentacion con Id: {idStr}");
                var profs = from te in _context.Profesores
                            join te_pr in _context.PresentacionesProfesores on te.IdProfesor equals te_pr.FkProfesorPres
                            where te_pr.FkPresentacionPres == int.Parse(idStr)
                            select ServiceProfesores.CleanProfesorData(te, null);
                return profs;    
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
                _logger.LogError(e, "Error al intentar obtener los profesores que asistieron a la presentacion.");
                throw;
            }
        }

        public bool SavePresentacion(Presentacione presentacion, List<int[]> idEstus, List<int> idProfs, string hora)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                _logger.LogInformation($"Registrando la informacion de una nueva presentacion : {presentacion} hora: {hora}");
                presentacion.HoraPresentacion = Utils.ConvertirHoraToTimeSpan(hora);
                _context.Presentaciones.Add(presentacion);
                _context.SaveChanges();
                var lastInput = _context.Presentaciones.OrderBy(p => p.IdPresentacion).LastOrDefault();

                foreach (var item in idEstus)
                {
                    _context.PresentacionesEstudiantes.Add(new PresentacionesEstudiante {
                        FkEstudiantePres = item[0],
                        EstadoPresentacion = (byte) item[1],
                        FkPresentacionEst = lastInput.IdPresentacion
                    });
                }

                foreach (var item in idProfs)
                {
                    _context.PresentacionesProfesores.Add(new PresentacionesProfesore {
                        FkPresentacionPres = lastInput.IdPresentacion,
                        FkProfesorPres = item
                    });
                }

                _context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar guardar la presentacion.");
                throw;
            }
        }

        public static object CleanPresentacionData(Presentacione p, IDataProtector protector)
        {
            string idProtect = null;
            if (protector != null)
                idProtect = protector.Protect(p.IdPresentacion.ToString());
            
            return new {
                   IdPresentacion = idProtect,
                   NombrePresentacion = p.NombrePresentacion,
                   FechaPresentacion = p.FechaPresentacion,
                   HoraPresentacion = Utils.ConvertirTimeSpanToStringHora(p.HoraPresentacion)
               };
        }
    }
}