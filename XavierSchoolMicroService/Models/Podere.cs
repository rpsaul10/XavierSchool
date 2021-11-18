using System;
using System.Collections.Generic;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class Podere
    {
        public Podere()
        {
            PoderesEstudiantes = new HashSet<PoderesEstudiante>();
        }

        public int IdPoder { get; set; }
        public string NombrePoder { get; set; }

        public virtual ICollection<PoderesEstudiante> PoderesEstudiantes { get; set; }
    }
}
