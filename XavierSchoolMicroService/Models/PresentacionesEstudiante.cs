using System;
using System.Collections.Generic;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class PresentacionesEstudiante
    {
        public int FkEstudiantePres { get; set; }
        public int FkPresentacionEst { get; set; }
        public byte? EstadoPresentacion { get; set; }

        public virtual Estudiante FkEstudiantePresNavigation { get; set; }
        public virtual Presentacione FkPresentacionEstNavigation { get; set; }
    }
}
