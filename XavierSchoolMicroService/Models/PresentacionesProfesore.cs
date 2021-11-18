using System;
using System.Collections.Generic;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class PresentacionesProfesore
    {
        public int FkPresentacionPres { get; set; }
        public int FkProfesorPres { get; set; }

        public virtual Presentacione FkPresentacionPresNavigation { get; set; }
        public virtual Profesore FkProfesorPresNavigation { get; set; }
    }
}
