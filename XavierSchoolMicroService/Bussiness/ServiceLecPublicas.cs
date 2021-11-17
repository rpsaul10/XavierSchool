using System.Linq;
using XavierSchoolMicroService.Services;
using XavierSchoolMicroService.Models;
using System.Collections.Generic;
using XavierSchoolMicroService.Utilities;
using System;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceLecPublicas : IServiceLecPublicas
    {
        private readonly escuela_xavierContext _context;
        public ServiceLecPublicas(escuela_xavierContext context)
        {
            _context = context;
        }
        public IQueryable<object> EtudiantesPorLeccion(string id)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<object> GetAll()
        {
            try
            {
                var lecciones = from lec in _context.Leccionpublicas
                                join teach in _context.Profesores on lec.FkProfesorLpub equals teach.IdProfesor
                                select CleanLecPubliData(lec, teach);

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
                var lecciones = from lec in _context.Leccionpublicas
                                join teach in _context.Profesores on lec.FkProfesorLpub equals teach.IdProfesor
                                where lec.IdLeccionpub == int.Parse(id)
                                select CleanLecPubliData(lec, teach);
                
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

        public static object CleanLecPubliData(Leccionpublica lec, Profesore teach)
        {
            return new {
                        IdLeccionpub = lec.IdLeccionpub,
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

        // public static TimeSpan ConvertirHoraToTimeSpan(string hour)
        // {
        //     string[] sep = hour.Split(':');
        //     int h = int.Parse(sep[0]);
        //     int m = int.Parse(sep[1]);

        //     TimeSpan timeSpan = new DateTime(2010, 1, 1, h, m, 0)  - new DateTime(2010, 1, 1, 0, 0, 0);
        //     return timeSpan;
        // }
    }
}