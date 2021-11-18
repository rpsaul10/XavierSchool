using System;
using System.Collections.Generic;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class Dormitorio
    {
        public Dormitorio()
        {
            Estudiantes = new HashSet<Estudiante>();
        }

        public int IdDormitorio { get; set; }
        public int? Piso { get; set; }
        public int? NumeroDpto { get; set; }

        public virtual ICollection<Estudiante> Estudiantes { get; set; }
    }
}
