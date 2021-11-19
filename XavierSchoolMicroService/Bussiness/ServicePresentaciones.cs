using System.Linq;
using XavierSchoolMicroService.Services;
using XavierSchoolMicroService.Models;
using System.Collections.Generic;
using XavierSchoolMicroService.Utilities;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServicePresentaciones : IServicePresentaciones
    {
        private readonly escuela_xavierContext _context;

        public ServicePresentaciones(escuela_xavierContext context)
        {
            _context = context;
        }

        public IQueryable<object> GetAll()
        {
            try
            {
               var pres = _context.Presentaciones.Select(p => CleanPresentacionData(p));
               return pres;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public IQueryable<object> GetEstudiantesById(string id)
        {
            try
            {
                var ests = from es in _context.Estudiantes
                        join es_tr in _context.PresentacionesEstudiantes on es.IdEstudiante equals es_tr.FkEstudiantePres
                        join niv in _context.Nivelpoders on es.FkNivelpoderEst equals niv.IdNivel
                        join dor in _context.Dormitorios on es.FkDormitorioEst equals dor.IdDormitorio
                        where es_tr.FkPresentacionEst == int.Parse(id)
                        select new {
                            Estudiante = ServiceEstudiante.CleanEstudianteData(es, dor, niv, null),
                            EstadoPresentacion = es_tr.EstadoPresentacion
                        };
                return ests;    
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public object GetPresentacion(string id)
        {
            try
            {
                var pre = _context.Presentaciones.Where(p => p.IdPresentacion == int.Parse(id)).FirstOrDefault();
                if (pre == null)
                    return null;
                return CleanPresentacionData(pre);
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public IQueryable<object> GetProfesoresById(string id)
        {
            try
            {
                var profs = from te in _context.Profesores
                            join te_pr in _context.PresentacionesProfesores on te.IdProfesor equals te_pr.FkProfesorPres
                            where te_pr.FkPresentacionPres == int.Parse(id)
                            select ServiceProfesores.CleanProfesorData(te);
                return profs;    
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public bool SavePresentacion(Presentacione presentacion, List<int[]> idEstus, List<int> idProfs, string hora)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
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
            catch (System.Exception)
            {
                transaction.Rollback();
                throw;
            }
            throw new System.NotImplementedException();
        }

        public static object CleanPresentacionData(Presentacione p)
        {
            return new {
                   IdPresentacion = p.IdPresentacion,
                   NombrePresentacion = p.NombrePresentacion,
                   FechaPresentacion = p.FechaPresentacion,
                   HoraPresentacion = Utils.ConvertirTimeSpanToStringHora(p.HoraPresentacion)
               };
        }
    }
}