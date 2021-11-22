using System;
using System.Collections.Generic;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class Leccionpublica
    {
        public Leccionpublica()
        {
            LeccionesEstudiantes = new HashSet<LeccionesEstudiante>();
        }

        public int IdLeccionpub { get; set; }
        public string NombreLeccionpub { get; set; }
        public TimeSpan? HoraLeccionpub { get; set; }
        public DateTime? FechaLeccionpu { get; set; }
        public int? FkProfesorLpub { get; set; }

        public virtual Profesore FkProfesorLpubNavigation { get; set; }
        public virtual ICollection<LeccionesEstudiante> LeccionesEstudiantes { get; set; }
    }
}
