using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;
using System.Linq;
using XavierSchoolMicroService.Utilities;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceProfesores : IServiceProfesores
    {
        private readonly escuela_xavierContext _context;
        private const string PURPOSE = "ProfesoresProtection";
        private readonly IDataProtector _protector;
        public ServiceProfesores(escuela_xavierContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector(PURPOSE);
        }
        public IQueryable<object> GetAll(int skip, int take)
        {
            try
            {
                var teachers = _context.Profesores.Skip(skip).Take(take).Select(p => CleanProfesorData(p, _protector));
                return teachers;
            }catch (System.Security.Cryptography.CryptographicException)
            {
                return null;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public object GetProfesor(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                var teacher = _context.Profesores.Where(p => p.IdProfesor == int.Parse(idStr)).FirstOrDefault();
                if (teacher == null)
                    return null;
                
                return CleanProfesorData(teacher, _protector);
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public bool SaveProfesor(Profesore profesor)
        {
            try
            {
                _context.Add(profesor);
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public bool UpdateProfesor(Profesore prof, string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
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
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
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
                var leccs = from lec in _context.Leccionpublicas
                            join prof in _context.Profesores on lec.FkProfesorLpub equals prof.IdProfesor
                            where prof.IdProfesor == int.Parse(idStr)
                            select ServiceLecPublicas.CleanLecPubliData(lec, prof, null);
                return leccs;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public IQueryable<object> GetLeccionesPrivadasByIdProf(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                var leccs = from lec in _context.Leccionprivada
                            join te in _context.Profesores on lec.FkProfesorLpriv equals te.IdProfesor
                            join es in _context.Estudiantes on lec.FkEstudianteLpriv equals es.IdEstudiante
                            where lec.FkProfesorLpriv == int.Parse(idStr)
                            select ServiceLecPrivadas.CleanLecPrivadaData(lec, te, es, null);
                return leccs;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public IQueryable<object> GetPresentacionesByIdProf(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                var pres = from pre in _context.Presentaciones
                            join pre_pro in _context.PresentacionesProfesores on pre.IdPresentacion equals pre_pro.FkPresentacionPres
                            where pre_pro.FkProfesorPres == int.Parse(idStr)
                            select ServicePresentaciones.CleanPresentacionData(pre, null);
                return pres;    
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }
    }
}