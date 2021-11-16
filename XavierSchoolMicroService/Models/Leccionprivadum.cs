using System;
using System.Collections.Generic;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class Leccionprivadum
    {
        public int IdLeccionpriv { get; set; }
        public string NombreLeccionpriv { get; set; }
        public TimeSpan? HoraLeccionpriv { get; set; }
        public DateTime? FechaLeccionpriv { get; set; }
        public int? FkEstudianteLpriv { get; set; }
        public int? FkProfesorLpriv { get; set; }

        public virtual Estudiante FkEstudianteLprivNavigation { get; set; }
        public virtual Profesore FkProfesorLprivNavigation { get; set; }
    }
}
