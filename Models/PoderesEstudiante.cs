using System;
using System.Collections.Generic;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class PoderesEstudiante
    {
        public int FkPoderEst { get; set; }
        public int FkEstudiantePod { get; set; }

        public virtual Estudiante FkEstudiantePodNavigation { get; set; }
        public virtual Podere FkPoderEstNavigation { get; set; }
    }
}
