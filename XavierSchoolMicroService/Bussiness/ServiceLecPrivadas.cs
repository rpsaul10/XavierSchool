using System.Linq;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceLecPrivadas : IServiceLecPrivadas
    {
        private readonly escuela_xavierContext _context;
        public ServiceLecPrivadas(escuela_xavierContext context)
        {
            _context = context;
        }
        public IQueryable<object> GetAll()
        {
            try
            {    
                var lePri = from lec in _context.Leccionprivada
                            join te in _context.Profesores on lec.FkProfesorLpriv equals te.IdProfesor
                            join es in _context.Estudiantes on lec.FkEstudianteLpriv equals es.IdEstudiante
                            select CleanLecPublicaData(lec, te, es);
                return lePri;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public object GetLeccionPrivada(string id)
        {
            try
            {
                var lePri = from lec in _context.Leccionprivada
                            join te in _context.Profesores on lec.FkProfesorLpriv equals te.IdProfesor
                            join es in _context.Estudiantes on lec.FkEstudianteLpriv equals es.IdEstudiante
                            where lec.IdLeccionpriv == int.Parse(id)
                            select CleanLecPublicaData(lec, te, es);  

                if (lePri.Count() == 0)
                    return null;
                return lePri.First();
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public bool SaveLeccPrivada(Leccionprivadum lec, string hour)
        {
            try
            {
                lec.HoraLeccionpriv = Utilities.Utils.ConvertirHoraToTimeSpan(hour);
                _context.Leccionprivada.Add(lec);
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new System.NotImplementedException();
        }

        public static object CleanLecPublicaData(Leccionprivadum lec, Profesore te, Estudiante es)
        {
            return new {
                IdLeccionpriv = lec.IdLeccionpriv,
                NombreLeccionpriv = lec.NombreLeccionpriv,
                HoraLeccionpriv = Utilities.Utils.ConvertirTimeSpanToStringHora(lec.HoraLeccionpriv),
                FechaLeccionpriv = lec.FechaLeccionpriv,
                Profesor = $"{te.NombreProfesor} {te.ApellidoProfesor}",
                Estudiante = $"{es.NombreEstudiante} {es.ApellidoEstudiante}"
            };
        }
    }
}