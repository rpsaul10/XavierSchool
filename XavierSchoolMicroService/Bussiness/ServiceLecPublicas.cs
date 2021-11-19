using System.Linq;
using XavierSchoolMicroService.Services;
using XavierSchoolMicroService.Models;
using System.Collections.Generic;
using XavierSchoolMicroService.Utilities;
using System;
using Microsoft.AspNetCore.DataProtection;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceLecPublicas : IServiceLecPublicas
    {
        private readonly escuela_xavierContext _context;
        private const string PURPOSE = "LeccionesPublicasProtection";
        private readonly IDataProtector _protector;
        public ServiceLecPublicas(escuela_xavierContext context, IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(PURPOSE);
            _context = context;
        }
        public IQueryable<object> EtudiantesPorLeccion(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                var estuds = from es in _context.Estudiantes
                            join es_le in _context.LeccionesEstudiantes on es.IdEstudiante equals es_le.FkEstudianteLec
                            join ni in _context.Nivelpoders on es.FkNivelpoderEst equals ni.IdNivel
                            join dor in _context.Dormitorios on es.FkDormitorioEst equals dor.IdDormitorio
                            where es_le.FkLeccionEst == int.Parse(idStr)
                            select ServiceEstudiante.CleanEstudianteData(es, dor, ni, null);
                return estuds;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public IQueryable<object> GetAll()
        {
            try
            {
                var lecciones = from lec in _context.Leccionpublicas
                                join teach in _context.Profesores on lec.FkProfesorLpub equals teach.IdProfesor
                                select CleanLecPubliData(lec, teach, _protector);

                return lecciones;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public object GetLecPublica(string id)
        {
            try
            {
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                var lecciones = from lec in _context.Leccionpublicas
                                join teach in _context.Profesores on lec.FkProfesorLpub equals teach.IdProfesor
                                where lec.IdLeccionpub == int.Parse(idStr)
                                select CleanLecPubliData(lec, teach, _protector);
                
                if (lecciones.Count() == 0)
                    return null;
                return lecciones.First();
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
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

        public bool SaveLeccPublica(Leccionpublica lec, List<int> estuds, string hour)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                lec.HoraLeccionpub = Utils.ConvertirHoraToTimeSpan(hour);
                _context.Leccionpublicas.Add(lec);
                _context.SaveChanges();
                var recent = _context.Leccionpublicas.OrderBy(l => l.IdLeccionpub).LastOrDefault();
                
                foreach(var id in estuds)
                {
                    _context.LeccionesEstudiantes.Add( new LeccionesEstudiante
                    {
                        FkEstudianteLec = id,
                        FkLeccionEst = recent.IdLeccionpub
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
    }
}