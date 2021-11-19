using System.Linq;
using Microsoft.AspNetCore.DataProtection;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;
using XavierSchoolMicroService.Utilities;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceLecPrivadas : IServiceLecPrivadas
    {
        private readonly escuela_xavierContext _context;
        private readonly IDataProtector _protector;
        private const string PURPOSE = "LeccionesPrivadasProtection";
        public ServiceLecPrivadas(escuela_xavierContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector(PURPOSE);
        }
        public IQueryable<object> GetAll()
        {
            try
            {    
                var lePri = from lec in _context.Leccionprivada
                            join te in _context.Profesores on lec.FkProfesorLpriv equals te.IdProfesor
                            join es in _context.Estudiantes on lec.FkEstudianteLpriv equals es.IdEstudiante
                            select CleanLecPrivadaData(lec, te, es, _protector);
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
                var idStr = id.Length > Utils.LENT ? _protector.Unprotect(id) : id;
                var lePri = from lec in _context.Leccionprivada
                            join te in _context.Profesores on lec.FkProfesorLpriv equals te.IdProfesor
                            join es in _context.Estudiantes on lec.FkEstudianteLpriv equals es.IdEstudiante
                            where lec.IdLeccionpriv == int.Parse(idStr)
                            select CleanLecPrivadaData(lec, te, es, _protector);  

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

        public static object CleanLecPrivadaData(Leccionprivadum lec, Profesore te, Estudiante es, IDataProtector protector)
        {
            // Falta
            string idProtect = null;
            if (protector != null)
                idProtect = protector.Protect(lec.IdLeccionpriv.ToString());
            return new {
                IdLeccionpriv = idProtect,
                NombreLeccionpriv = lec.NombreLeccionpriv,
                HoraLeccionpriv = Utilities.Utils.ConvertirTimeSpanToStringHora(lec.HoraLeccionpriv),
                FechaLeccionpriv = lec.FechaLeccionpriv,
                Profesor = $"{te.NombreProfesor} {te.ApellidoProfesor}",
                Estudiante = $"{es.NombreEstudiante} {es.ApellidoEstudiante}"
            };
        }
    }
}