using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace XavierSchoolMicroService.Models
{
    public partial class escuela_xavierContext : DbContext
    {
        public escuela_xavierContext()
        {
        }

        public escuela_xavierContext(DbContextOptions<escuela_xavierContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dormitorio> Dormitorios { get; set; }
        public virtual DbSet<Estudiante> Estudiantes { get; set; }
        public virtual DbSet<LeccionesEstudiante> LeccionesEstudiantes { get; set; }
        public virtual DbSet<Leccionprivadum> Leccionprivada { get; set; }
        public virtual DbSet<Leccionpublica> Leccionpublicas { get; set; }
        public virtual DbSet<Nivelpoder> Nivelpoders { get; set; }
        public virtual DbSet<Podere> Poderes { get; set; }
        public virtual DbSet<PoderesEstudiante> PoderesEstudiantes { get; set; }
        public virtual DbSet<Presentacione> Presentaciones { get; set; }
        public virtual DbSet<PresentacionesEstudiante> PresentacionesEstudiantes { get; set; }
        public virtual DbSet<PresentacionesProfesore> PresentacionesProfesores { get; set; }
        public virtual DbSet<Profesore> Profesores { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

//         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//         {
//             if (!optionsBuilder.IsConfigured)
//             {
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                 optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=12345;database=escuela_xavier");
//             }
//         }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dormitorio>(entity =>
            {
                entity.HasKey(e => e.IdDormitorio)
                    .HasName("PRIMARY");

                entity.ToTable("dormitorios");

                entity.Property(e => e.IdDormitorio).HasColumnName("idDormitorio");

                entity.Property(e => e.NumeroDpto).HasColumnName("numero_dpto");

                entity.Property(e => e.Piso).HasColumnName("piso");
            });

            modelBuilder.Entity<Estudiante>(entity =>
            {
                entity.HasKey(e => e.IdEstudiante)
                    .HasName("PRIMARY");

                entity.ToTable("estudiantes");

                entity.HasIndex(e => e.FkDormitorioEst, "fk_dormitorio_idx");

                entity.HasIndex(e => e.FkNivelpoderEst, "fk_nivelpoder_idx");

                entity.Property(e => e.IdEstudiante).HasColumnName("idEstudiante");

                entity.Property(e => e.ActivoOInactivo)
                    .HasColumnType("tinyint")
                    .HasColumnName("activo_o_inactivo");

                entity.Property(e => e.ApellidoEstudiante)
                    .HasMaxLength(45)
                    .HasColumnName("apellido_estudiante");

                entity.Property(e => e.FechaNacimiento)
                    .HasColumnType("date")
                    .HasColumnName("fecha_nacimiento");

                entity.Property(e => e.FkDormitorioEst).HasColumnName("fk_dormitorio_est");

                entity.Property(e => e.FkNivelpoderEst).HasColumnName("fk_nivelpoder_est");

                entity.Property(e => e.NombreEstudiante)
                    .HasMaxLength(45)
                    .HasColumnName("nombre_estudiante");

                entity.Property(e => e.NssEstudiante)
                    .HasMaxLength(10)
                    .HasColumnName("nss_estudiante");

                entity.HasOne(d => d.FkDormitorioEstNavigation)
                    .WithMany(p => p.Estudiantes)
                    .HasForeignKey(d => d.FkDormitorioEst)
                    .HasConstraintName("idDormitorio");

                entity.HasOne(d => d.FkNivelpoderEstNavigation)
                    .WithMany(p => p.Estudiantes)
                    .HasForeignKey(d => d.FkNivelpoderEst)
                    .HasConstraintName("idNivel");
            });

            modelBuilder.Entity<LeccionesEstudiante>(entity =>
            {
                entity.HasKey(e => new { e.FkEstudianteLec, e.FkLeccionEst })
                    .HasName("PRIMARY");

                entity.ToTable("lecciones_estudiantes");

                entity.HasIndex(e => e.FkEstudianteLec, "fk_estudiante_idx");

                entity.HasIndex(e => e.FkLeccionEst, "fk_leccionpu_idx");

                entity.Property(e => e.FkEstudianteLec).HasColumnName("fk_estudiante_lec");

                entity.Property(e => e.FkLeccionEst).HasColumnName("fk_leccion_est");

                entity.HasOne(d => d.FkEstudianteLecNavigation)
                    .WithMany(p => p.LeccionesEstudiantes)
                    .HasForeignKey(d => d.FkEstudianteLec)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("idEstudiante");

                entity.HasOne(d => d.FkLeccionEstNavigation)
                    .WithMany(p => p.LeccionesEstudiantes)
                    .HasForeignKey(d => d.FkLeccionEst)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("idLeccionpu");
            });

            modelBuilder.Entity<Leccionprivadum>(entity =>
            {
                entity.HasKey(e => e.IdLeccionpriv)
                    .HasName("PRIMARY");

                entity.ToTable("leccionprivada");

                entity.HasIndex(e => e.FkEstudianteLpriv, "idEstudiante_idx");

                entity.HasIndex(e => e.FkProfesorLpriv, "idProfesor_idx");

                entity.Property(e => e.IdLeccionpriv).HasColumnName("idLeccionpriv");

                entity.Property(e => e.FechaLeccionpriv)
                    .HasColumnType("date")
                    .HasColumnName("fecha_leccionpriv");

                entity.Property(e => e.FkEstudianteLpriv).HasColumnName("fk_estudiante_lpriv");

                entity.Property(e => e.FkProfesorLpriv).HasColumnName("fk_profesor_lpriv");

                entity.Property(e => e.HoraLeccionpriv).HasColumnName("hora_leccionpriv");

                entity.Property(e => e.NombreLeccionpriv)
                    .HasMaxLength(45)
                    .HasColumnName("nombre_leccionpriv");

                entity.HasOne(d => d.FkEstudianteLprivNavigation)
                    .WithMany(p => p.Leccionprivada)
                    .HasForeignKey(d => d.FkEstudianteLpriv)
                    .HasConstraintName("idEstudiantePriv");

                entity.HasOne(d => d.FkProfesorLprivNavigation)
                    .WithMany(p => p.Leccionprivada)
                    .HasForeignKey(d => d.FkProfesorLpriv)
                    .HasConstraintName("idProfesorPriv");
            });

            modelBuilder.Entity<Leccionpublica>(entity =>
            {
                entity.HasKey(e => e.IdLeccionpub)
                    .HasName("PRIMARY");

                entity.ToTable("leccionpublica");

                entity.HasIndex(e => e.FkProfesorLpub, "idProfesor_idx");

                entity.Property(e => e.IdLeccionpub).HasColumnName("idLeccionpub");

                entity.Property(e => e.FechaLeccionpu)
                    .HasColumnType("date")
                    .HasColumnName("fecha_leccionpu");

                entity.Property(e => e.FkProfesorLpub).HasColumnName("fk_profesor_lpub");

                entity.Property(e => e.HoraLeccionpub).HasColumnName("hora_leccionpub");

                entity.Property(e => e.NombreLeccionpub)
                    .HasMaxLength(45)
                    .HasColumnName("nombre_leccionpub");

                entity.HasOne(d => d.FkProfesorLpubNavigation)
                    .WithMany(p => p.Leccionpublicas)
                    .HasForeignKey(d => d.FkProfesorLpub)
                    .HasConstraintName("idProfesorPub");
            });

            modelBuilder.Entity<Nivelpoder>(entity =>
            {
                entity.HasKey(e => e.IdNivel)
                    .HasName("PRIMARY");

                entity.ToTable("nivelpoder");

                entity.Property(e => e.IdNivel).HasColumnName("idNivel");

                entity.Property(e => e.NombreNivel)
                    .HasMaxLength(45)
                    .HasColumnName("nombre_nivel");
            });

            modelBuilder.Entity<Podere>(entity =>
            {
                entity.HasKey(e => e.IdPoder)
                    .HasName("PRIMARY");

                entity.ToTable("poderes");

                entity.Property(e => e.IdPoder).HasColumnName("idPoder");

                entity.Property(e => e.NombrePoder)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("nombre_poder");
            });

            modelBuilder.Entity<PoderesEstudiante>(entity =>
            {
                entity.HasKey(e => new { e.FkPoderEst, e.FkEstudiantePod })
                    .HasName("PRIMARY");

                entity.ToTable("poderes_estudiantes");

                entity.HasIndex(e => e.FkEstudiantePod, "fk_estudiante_idx");

                entity.HasIndex(e => e.FkPoderEst, "fk_poder_idx");

                entity.Property(e => e.FkPoderEst).HasColumnName("fk_poder_est");

                entity.Property(e => e.FkEstudiantePod).HasColumnName("fk_estudiante_pod");

                entity.HasOne(d => d.FkEstudiantePodNavigation)
                    .WithMany(p => p.PoderesEstudiantes)
                    .HasForeignKey(d => d.FkEstudiantePod)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_estudiante");

                entity.HasOne(d => d.FkPoderEstNavigation)
                    .WithMany(p => p.PoderesEstudiantes)
                    .HasForeignKey(d => d.FkPoderEst)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_poder");
            });

            modelBuilder.Entity<Presentacione>(entity =>
            {
                entity.HasKey(e => e.IdPresentacion)
                    .HasName("PRIMARY");

                entity.ToTable("presentaciones");

                entity.Property(e => e.IdPresentacion).HasColumnName("idPresentacion");

                entity.Property(e => e.FechaPresentacion)
                    .HasColumnType("date")
                    .HasColumnName("fecha_presentacion");

                entity.Property(e => e.HoraPresentacion).HasColumnName("hora_presentacion");

                entity.Property(e => e.NombrePresentacion)
                    .HasMaxLength(45)
                    .HasColumnName("nombre_presentacion");
            });

            modelBuilder.Entity<PresentacionesEstudiante>(entity =>
            {
                entity.HasKey(e => new { e.FkEstudiantePres, e.FkPresentacionEst })
                    .HasName("PRIMARY");

                entity.ToTable("presentaciones_estudiantes");

                entity.HasIndex(e => e.FkEstudiantePres, "FK_estudiante_idx");

                entity.HasIndex(e => e.FkPresentacionEst, "FK_presentacion_idx");

                entity.Property(e => e.FkEstudiantePres).HasColumnName("fk_estudiante_pres");

                entity.Property(e => e.FkPresentacionEst).HasColumnName("fk_presentacion_est");

                entity.Property(e => e.EstadoPresentacion)
                    .HasColumnType("tinyint")
                    .HasColumnName("estado_presentacion");

                entity.HasOne(d => d.FkEstudiantePresNavigation)
                    .WithMany(p => p.PresentacionesEstudiantes)
                    .HasForeignKey(d => d.FkEstudiantePres)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("idEstudiantePres");

                entity.HasOne(d => d.FkPresentacionEstNavigation)
                    .WithMany(p => p.PresentacionesEstudiantes)
                    .HasForeignKey(d => d.FkPresentacionEst)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("idPresentacionEst");
            });

            modelBuilder.Entity<PresentacionesProfesore>(entity =>
            {
                entity.HasKey(e => new { e.FkPresentacionPres, e.FkProfesorPres })
                    .HasName("PRIMARY");

                entity.ToTable("presentaciones_profesores");

                entity.HasIndex(e => e.FkPresentacionPres, "fk_presentacion_idx");

                entity.HasIndex(e => e.FkProfesorPres, "fk_profesor_idx");

                entity.Property(e => e.FkPresentacionPres).HasColumnName("fk_presentacion_pres");

                entity.Property(e => e.FkProfesorPres).HasColumnName("fk_profesor_pres");

                entity.HasOne(d => d.FkPresentacionPresNavigation)
                    .WithMany(p => p.PresentacionesProfesores)
                    .HasForeignKey(d => d.FkPresentacionPres)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("idPresentacion");

                entity.HasOne(d => d.FkProfesorPresNavigation)
                    .WithMany(p => p.PresentacionesProfesores)
                    .HasForeignKey(d => d.FkProfesorPres)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("idProfesor");
            });

            modelBuilder.Entity<Profesore>(entity =>
            {
                entity.HasKey(e => e.IdProfesor)
                    .HasName("PRIMARY");

                entity.ToTable("profesores");

                entity.Property(e => e.IdProfesor).HasColumnName("idProfesor");

                entity.Property(e => e.ActivoOInactivo)
                    .HasColumnType("tinyint")
                    .HasColumnName("activo_o_inactivo");

                entity.Property(e => e.ApellidoProfesor)
                    .HasMaxLength(45)
                    .HasColumnName("apellido_profesor");

                entity.Property(e => e.FechaNacimientopr)
                    .HasColumnType("date")
                    .HasColumnName("fecha_nacimientopr");

                entity.Property(e => e.NombreProfesor)
                    .HasMaxLength(45)
                    .HasColumnName("nombre_profesor");

                entity.Property(e => e.NssProfesor)
                    .HasMaxLength(45)
                    .HasColumnName("nss_profesor");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario)
                    .HasName("PRIMARY");

                entity.ToTable("usuarios");

                entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

                entity.Property(e => e.ApellidoUsuario)
                    .HasMaxLength(45)
                    .HasColumnName("apellido_usuario");

                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasMaxLength(65)
                    .HasColumnName("correo");

                entity.Property(e => e.EstadoAdministrador)
                    .HasColumnType("tinyint")
                    .HasColumnName("estadoAdministrador");

                entity.Property(e => e.NombreUsuario)
                    .HasMaxLength(45)
                    .HasColumnName("nombre_usuario");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasColumnName("password");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
