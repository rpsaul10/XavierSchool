using System;
using System.Collections.Generic;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class Presentacione
    {
        public Presentacione()
        {
            PresentacionesEstudiantes = new HashSet<PresentacionesEstudiante>();
            PresentacionesProfesores = new HashSet<PresentacionesProfesore>();
        }

        public int IdPresentacion { get; set; }
        public string NombrePresentacion { get; set; }
        public TimeSpan? HoraPresentacion { get; set; }
        public DateTime? FechaPresentacion { get; set; }

        public virtual ICollection<PresentacionesEstudiante> PresentacionesEstudiantes { get; set; }
        public virtual ICollection<PresentacionesProfesore> PresentacionesProfesores { get; set; }
    }
}
