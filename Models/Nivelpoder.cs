using System;
using System.Collections.Generic;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class Nivelpoder
    {
        public Nivelpoder()
        {
            Estudiantes = new HashSet<Estudiante>();
        }

        public int IdNivel { get; set; }
        public string NombreNivel { get; set; }

        public virtual ICollection<Estudiante> Estudiantes { get; set; }
    }
}
