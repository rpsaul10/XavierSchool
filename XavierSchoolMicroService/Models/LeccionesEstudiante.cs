using System;
using System.Collections.Generic;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class LeccionesEstudiante
    {
        public int FkEstudianteLec { get; set; }
        public int FkLeccionEst { get; set; }

        public virtual Estudiante FkEstudianteLecNavigation { get; set; }
        public virtual Leccionpublica FkLeccionEstNavigation { get; set; }
    }
}
