using System;
using System.Collections.Generic;
using System.Linq;
using XavierSchoolMicroService.Utilities;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceEstudiante : IServiceEstudiante
    {
        private readonly escuela_xavierContext _context;
        private const string PURPOSE = "EstudiantesProtection";
        private readonly IDataProtector _protector;
        private readonly ILogger<ServiceEstudiante> _logger;

        public ServiceEstudiante (escuela_xavierContext context, IDataProtectionProvider provider, ILogger<ServiceEstudiante> logger)
        {
            _logger = logger;
            _context = context;
            _protector = provider.CreateProtector(PURPOSE);
        }

        public IQueryable<object> GetAll()
        {
            try
            {
                _logger.LogInformation("Obteniendo la lista de todos los estudiantes");
                var estuds = from est in _context.Estudiantes
                            join niv in _context.Nivelpoders on est.FkNivelpoderEst equals niv.IdNivel
                            select CleanEstudianteData(est, niv, _protector);
                return estuds;
            } catch (System.Security.Cryptography.CryptographicException ce)
            {
                _logger.LogError(ce, "Error al intentar encriptar los ids");
                throw;
            }
             catch (Exception e)
            {
                _logger.LogError(e, "Error al obtener la lista de todos los estudiantes");
                throw;
            }
        }

        public object GetEstudiante(string id, byte mode)
        {
            try 
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo la informacion del estudiante con Id: {idStr}");
                if (mode == 0)
                {
                    return _context.Estudiantes.Where(e => e.IdEstudiante == int.Parse(idStr)).Select(e => new {
                        NombreEstudiante= e.NombreEstudiante,
                        ApellidoEstudiante = e.ApellidoEstudiante,
                        FkNivelpoderEst = e.FkNivelpoderEst,
                        ActivoOInactivo = e.ActivoOInactivo,
                        FechaNacimiento = e.FechaNacimiento
                    }).FirstOrDefault();
                }
                var estu = from est in _context.Estudiantes
                        join niv in _context.Nivelpoders on est.FkNivelpoderEst equals niv.IdNivel
                        where est.IdEstudiante == int.Parse(idStr)
                        select CleanEstudianteData(est, niv, _protector);

                if (estu.Count() == 0)
                    return null;

                return estu.First();
            } catch (InvalidOperationException fe)
            {
                _logger.LogError(fe, "Error al convertir el id a numero.");
                throw;
            } catch (System.Security.Cryptography.CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intenatar decriptar el id: {id}");
                throw;
            }
             catch (Exception e)
            {
                _logger.LogError(e, $"Error a obtener la informaciondel estudiante con Id: {id}");
                throw;
            }
        }

        public bool SaveEstudiante(Estudiante estudiante, List<int> powerList)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                _logger.LogInformation("Guardando inofrmacion del nuevo estudiante.");
                _context.Estudiantes.Add(estudiante);
                _context.SaveChanges();

                var lastInsert = _context.Estudiantes.OrderBy(e => e.IdEstudiante).LastOrDefault();

                foreach (var powe in powerList)
                {
                    _context.PoderesEstudiantes.Add( new PoderesEstudiante {
                        FkEstudiantePod = lastInsert.IdEstudiante,
                        FkPoderEst = powe
                    });
                }

                _context.SaveChanges();
                transaction.Commit();
                return true; 
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar guardar la informacion del nuevo estudiante.");
                transaction.Rollback();
                throw;
            }
        }

        public bool UpdateEstudiante(string id, Estudiante estudiante, List<int> powerList)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Acualizando datos del estudiante con Id: {idStr}");
                var oldDataEst = _context.Estudiantes.Where(e => e.IdEstudiante == int.Parse(idStr)).FirstOrDefault();

                if (oldDataEst != null)
                {
                    if (!oldDataEst.NombreEstudiante.Equals(estudiante.NombreEstudiante)) oldDataEst.NombreEstudiante = estudiante.NombreEstudiante;
                    if (!oldDataEst.ApellidoEstudiante.Equals(estudiante.ApellidoEstudiante)) oldDataEst.ApellidoEstudiante = estudiante.ApellidoEstudiante;
                    if (!oldDataEst.FechaNacimiento.Equals(estudiante.FechaNacimiento)) oldDataEst.FechaNacimiento = estudiante.FechaNacimiento;
                    if (oldDataEst.ActivoOInactivo != estudiante.ActivoOInactivo) oldDataEst.ActivoOInactivo = estudiante.ActivoOInactivo;
                    if (oldDataEst.FkNivelpoderEst != estudiante.FkNivelpoderEst) oldDataEst.FkNivelpoderEst = estudiante.FkNivelpoderEst;
                    if (!oldDataEst.NssEstudiante.Equals(estudiante.NssEstudiante)) oldDataEst.NombreEstudiante = estudiante.NssEstudiante;

                    var powers = _context.PoderesEstudiantes.Where(pe => pe.FkEstudiantePod == int.Parse(idStr));
                    powerList.Sort();
                    if (!ComparePowers(powerList, powers.OrderBy(e => e.FkPoderEst).ToList()))
                    {
                        foreach (var p in powers)
                        {
                            _context.PoderesEstudiantes.Remove(p);
                        }
                        foreach (int item in powerList)
                        {
                            _context.Add(new PoderesEstudiante{
                                FkEstudiantePod = int.Parse(idStr),
                                FkPoderEst = item
                            });
                        }
                    }
                    
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                return false;
            } catch (InvalidOperationException fe)
            {
                transaction.Rollback();
                _logger.LogError(fe, "Error al convertir el id a numero.");
                throw;
            } catch (System.Security.Cryptography.CryptographicException ce)
            {
                transaction.Rollback();
                _logger.LogError(ce, $"Error al intenatar decriptar el id del estudiante: {id}");
                throw;
            }
            catch (System.Exception e)
            {
                transaction.Rollback();
                _logger.LogError(e, "Error al intentar actualizar la informacion del estudiante");
                throw;
            }
        }

        public static object CleanEstudianteData(Estudiante est, Nivelpoder niv, IDataProtector prot)
        {
            string idProtect = null;
            if (prot != null)
            {
                idProtect = prot.Protect(est.IdEstudiante.ToString());
            }
            return new {
                        IdEstudiante = idProtect,
                        NombreEstudiante = est.NombreEstudiante,
                        ApellidoEstudiante = est.ApellidoEstudiante,
                        FechaNacimiento = est.FechaNacimiento,
                        NssEstudiante = est.NssEstudiante,
                        ActivoOInactivo = est.ActivoOInactivo,
                        Nivelpoder = niv.NombreNivel
                    };
        }

        public IQueryable<object> GetPowersByEstudiante(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Intentando obtener los poderes del estudiante con id: {idStr}");
                return from pod in _context.Poderes
                            join pod_est in _context.PoderesEstudiantes on pod.IdPoder equals pod_est.FkPoderEst
                            where pod_est.FkEstudiantePod == int.Parse(idStr)
                            select ServicePoderes.CleanPoderesData(pod);
            }  catch (FormatException fe)
            {
                _logger.LogError(fe, "Error al convertir el id a numero.");
                throw;
            } catch (System.Security.Cryptography.CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intenatar decriptar el id del estudiante: {id}");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar Obtener los poderes del estudiante");
                throw;
            }
        }

        public IQueryable<object> GetLeccionesPublicasByIdEstu(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation("Intentando obtener las lecciones en grupo a las que asistio el estudiante con id: {idstr}");
                var lecc = from lec in _context.Leccionpublicas
                            join te in _context.Profesores on lec.FkProfesorLpub equals te.IdProfesor
                            join lec_est in _context.LeccionesEstudiantes on lec.IdLeccionpub equals lec_est.FkLeccionEst
                            where lec_est.FkEstudianteLec == int.Parse(idStr)
                            select ServiceLecPublicas.CleanLecPubliData(lec, te, null);
                return lecc;
            } catch (InvalidOperationException fe)
            {
                _logger.LogError(fe, "Error al convertir el id a numero.");
                throw;
            } catch (System.Security.Cryptography.CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intenatar decriptar el id del estudiante: {id}");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogInformation(e, "Error al intentar obtener las lecciones en grupo a las que asistio el estudiante.");
                throw;
            }
        }

        public IQueryable<object> GetLeccionesPrivadasByIdEstu(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo las lecciones privadas a las que asistio el estudiante con id: {idStr}");
                var leccs = from lec in _context.Leccionprivada
                            join te in _context.Profesores on lec.FkProfesorLpriv equals te.IdProfesor
                            join es in _context.Estudiantes on lec.FkEstudianteLpriv equals es.IdEstudiante
                            where lec.FkEstudianteLpriv == int.Parse(idStr)
                            select ServiceLecPrivadas.CleanLecPrivadaData(lec, te, es, null);
                return leccs;
            } catch (InvalidOperationException fe)
            {
                _logger.LogError(fe, "Error al convertir el id a numero.");
                throw;
            } catch (System.Security.Cryptography.CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intenatar decriptar el id del estudiante: {id}");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar obtener las lecciones privadas a las que asistio el estudiante.");
                throw;
            }
        }

        public static bool ComparePowers(List<int> powers_sort, List<PoderesEstudiante> pe_sort)
        {
            var p = new LinkedList<int>(powers_sort);
            var p2 = new LinkedList<PoderesEstudiante>(pe_sort.ToList());

            while(p.Count() > 0 && p2.Count() > 0)
            {
                if (p.First() == p2.First().FkPoderEst)
                {
                    p.RemoveFirst();
                    p2.RemoveFirst();
                    continue;
                }
                return false;
            }
            if (p.Count() > 0 || p2.Count() > 0)
                return false;
            return true;
        }

        public IQueryable<object> GetPresentacionesByIdEstu(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                _logger.LogInformation($"Obteniendo las presentaciones a las que asistio el estudiante con el id {idStr}");
                var pres = from pre in _context.Presentaciones
                            join es_pr in _context.PresentacionesEstudiantes on pre.IdPresentacion equals es_pr.FkPresentacionEst
                            where es_pr.FkEstudiantePres == int.Parse(idStr)
                            select new {
                                Estudiante = ServicePresentaciones.CleanPresentacionData(pre, null),
                                Asistencia = es_pr.EstadoPresentacion
                            };
                return pres;
            } catch (InvalidOperationException fe)
            {
                _logger.LogError(fe, "Error al convertir el id a numero.");
                throw;
            } catch (System.Security.Cryptography.CryptographicException ce)
            {
                _logger.LogError(ce, $"Error al intenatar decriptar el id del estudiante: {id}");
                throw;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error al intentar obtener las presentaciones a las que asistio el estudiante");
                throw;
            }
        }

        public IQueryable<object> GetNiveles()
        {
            try
            {
                return _context.Nivelpoders.Select(n => new {
                    IdNivel = n.IdNivel,
                    NombreNivel = n.NombreNivel
                });  
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
