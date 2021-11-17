using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XavierSchoolMicroService.Utilities;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceEstudiante : IServiceEstudiante
    {
        private readonly escuela_xavierContext _context;
        private const string PURPOSE = "EstudiantesProtection";

        private readonly IDataProtector _protector;

        public ServiceEstudiante (escuela_xavierContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector(PURPOSE);
        }

        public IQueryable<object> GetAll()
        {
            try
            {
                var estuds = from est in _context.Estudiantes
                            join dorm in _context.Dormitorios on est.FkDormitorioEst equals dorm.IdDormitorio
                            join niv in _context.Nivelpoders on est.FkNivelpoderEst equals niv.IdNivel
                            select CleanEstudianteData(est, dorm, niv, _protector);
                return estuds;
            } catch (Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public object GetEstudiante(string id)
        {
            try 
            {
                var idStr = _protector.Unprotect(id);
                var estu = from est in _context.Estudiantes
                        join dorm in _context.Dormitorios on est.FkDormitorioEst equals dorm.IdDormitorio
                        join niv in _context.Nivelpoders on est.FkNivelpoderEst equals niv.IdNivel
                        where est.IdEstudiante == int.Parse(idStr)
                        select CleanEstudianteData(est, dorm, niv, _protector);

                if (estu.Count() == 0)
                    return null;

                return estu.First();
            } catch (Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public bool SaveEstudiante(Estudiante estudiante)
        {
            try
            {
                _context.Add(estudiante);
                _context.SaveChanges();
                return true; 
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public bool UpdateEstudiante(string id, Estudiante estudiante)
        {
            try
            {
                var idStr = _protector.Unprotect(id);
                 
                var oldDataEst = _context.Estudiantes.Where(e => e.IdEstudiante == int.Parse(idStr)).FirstOrDefault();

                if (oldDataEst != null)
                {
                    oldDataEst.NombreEstudiante = !oldDataEst.NombreEstudiante.Equals (estudiante.NombreEstudiante) ? estudiante.NombreEstudiante : oldDataEst.NombreEstudiante;
                    oldDataEst.ApellidoEstudiante = !oldDataEst.ApellidoEstudiante.Equals (estudiante.ApellidoEstudiante) ? estudiante.ApellidoEstudiante : oldDataEst.ApellidoEstudiante;
                    oldDataEst.FechaNacimiento = estudiante.FechaNacimiento;
                    oldDataEst.FkDormitorioEst = estudiante.FkDormitorioEst;
                    oldDataEst.ActivoOInactivo = estudiante.ActivoOInactivo;
                    oldDataEst.FkNivelpoderEst = estudiante.FkNivelpoderEst;
                    oldDataEst.NssEstudiante = estudiante.NssEstudiante;

                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public static object CleanEstudianteData(Estudiante est, Dormitorio dorm, Nivelpoder niv, IDataProtector prot)
        {
            return new {
                        IdEstudiante = prot.Protect(est.IdEstudiante.ToString()),
                        NombreEstudiante = est.NombreEstudiante,
                        ApellidoEstudiante = est.ApellidoEstudiante,
                        FechaNacimiento = est.FechaNacimiento,
                        DormitorioEst =  Utils.BuildDicDormitorio(dorm.NumeroDpto, dorm.Piso),
                        NssEstudiante = est.NssEstudiante,
                        ActivoOInactivo = est.ActivoOInactivo,
                        Nivelpoder = niv.NombreNivel
                    };
        }

        public IQueryable<string> GetPowersByEstudiante(string id)
        {
            var idStr = _protector.Unprotect(id);
            try
            {
                return from pod in _context.Poderes
                            join pod_est in _context.PoderesEstudiantes on pod.IdPoder equals pod_est.FkPoderEst
                            where pod_est.FkEstudiantePod == int.Parse(idStr)
                            select pod.NombrePoder;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public IQueryable<object> GetLeccionesPublicasByIdEstu(string id)
        {
            try
            {
                var lecc = from lec in _context.Leccionpublicas
                            join te in _context.Profesores on lec.FkProfesorLpub equals te.IdProfesor
                            join lec_est in _context.LeccionesEstudiantes on lec.IdLeccionpub equals lec_est.FkLeccionEst
                            where lec_est.FkEstudianteLec == int.Parse(id)
                            select ServiceLecPublicas.CleanLecPubliData(lec, te);
                return lecc;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }
    }
}
