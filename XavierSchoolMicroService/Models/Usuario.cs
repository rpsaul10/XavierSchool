using System;
using System.Collections.Generic;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public string Correo { get; set; }
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }
        public string Password { get; set; }
        public byte? EstadoAdministrador { get; set; }
    }
}
