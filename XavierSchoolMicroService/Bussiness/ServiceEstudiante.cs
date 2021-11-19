using System;
using System.Collections.Generic;
using System.Linq;
using XavierSchoolMicroService.Utilities;
using XavierSchoolMicroService.Models;
using XavierSchoolMicroService.Services;
using Microsoft.AspNetCore.DataProtection;

namespace XavierSchoolMicroService.Bussiness
{
    public class ServiceEstudiante : IServiceEstudiante
    {
        public static readonly int LENT = 4;
        private readonly escuela_xavierContext _context;
        private const string PURPOSE = "EstudiantesProtection";

        private readonly IDataProtector _protector;

        public ServiceEstudiante (escuela_xavierContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector(PURPOSE);
        }

        public IQueryable<object> GetAll()
        {
            try
            {
                var estuds = from est in _context.Estudiantes
                            join dorm in _context.Dormitorios on est.FkDormitorioEst equals dorm.IdDormitorio
                            join niv in _context.Nivelpoders on est.FkNivelpoderEst equals niv.IdNivel
                            select CleanEstudianteData(est, dorm, niv, _protector);
                return estuds;
            } catch (Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public object GetEstudiante(string id)
        {
            try 
            {
                var idStr = id.Count() > LENT ? _protector.Unprotect(id) : id;
                var estu = from est in _context.Estudiantes
                        join dorm in _context.Dormitorios on est.FkDormitorioEst equals dorm.IdDormitorio
                        join niv in _context.Nivelpoders on est.FkNivelpoderEst equals niv.IdNivel
                        where est.IdEstudiante == int.Parse(idStr)
                        select CleanEstudianteData(est, dorm, niv, _protector);

                if (estu.Count() == 0)
                    return null;

                return estu.First();
            } catch (Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public bool SaveEstudiante(Estudiante estudiante, List<int> powerList)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Estudiantes.Add(estudiante);
                _context.SaveChanges();

                var lastInsert = _context.Estudiantes.OrderBy(e => e.IdEstudiante).LastOrDefault();

                foreach (var powe in powerList)
                {
                    _context.PoderesEstudiantes.Add( new PoderesEstudiante {
                        FkEstudiantePod = lastInsert.IdEstudiante,
                        FkPoderEst = powe
                    });
                }
                _context.SaveChanges();
                transaction.Commit();
                return true; 
            }
            catch (System.Exception)
            {
                transaction.Rollback();
                throw;
            }
            throw new NotImplementedException();
        }

        public bool UpdateEstudiante(string id, Estudiante estudiante, List<int> powerList)
        {
            var idStr = id.Count() > LENT ? _protector.Unprotect(id) : id;
            var transaction = _context.Database.BeginTransaction();
            try
            {
                 
                var oldDataEst = _context.Estudiantes.Where(e => e.IdEstudiante == int.Parse(idStr)).FirstOrDefault();

                if (oldDataEst != null)
                {
                    if (!oldDataEst.NombreEstudiante.Equals(estudiante.NombreEstudiante)) oldDataEst.NombreEstudiante = estudiante.NombreEstudiante;
                    if (!oldDataEst.ApellidoEstudiante.Equals(estudiante.ApellidoEstudiante)) oldDataEst.ApellidoEstudiante = estudiante.ApellidoEstudiante;
                    if (!oldDataEst.FechaNacimiento.Equals(estudiante.FechaNacimiento)) oldDataEst.FechaNacimiento = estudiante.FechaNacimiento;
                    if (oldDataEst.FkDormitorioEst != estudiante.FkDormitorioEst) oldDataEst.FkDormitorioEst = estudiante.FkDormitorioEst;
                    if (oldDataEst.ActivoOInactivo != estudiante.ActivoOInactivo) oldDataEst.ActivoOInactivo = estudiante.ActivoOInactivo;
                    if (oldDataEst.FkNivelpoderEst != estudiante.FkNivelpoderEst) oldDataEst.FkNivelpoderEst = estudiante.FkNivelpoderEst;
                    if (!oldDataEst.NssEstudiante.Equals(estudiante.NssEstudiante)) oldDataEst.NombreEstudiante = estudiante.NssEstudiante;

                    var powers = _context.PoderesEstudiantes.Where(pe => pe.FkEstudiantePod == int.Parse(idStr));
                    powerList.Sort();
                    if (!ComparePowers(powerList, powers.OrderBy(e => e.FkPoderEst).ToList()))
                    {
                        foreach (var p in powers)
                        {
                            _context.PoderesEstudiantes.Remove(p);
                        }
                        foreach (int item in powerList)
                        {
                            _context.Add(new PoderesEstudiante{
                                FkEstudiantePod = int.Parse(idStr),
                                FkPoderEst = item
                            });
                        }
                    }
                    
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                return false;
            }
            catch (System.Exception)
            {
                transaction.Rollback();
                throw;
            }
            throw new NotImplementedException();
        }

        public static object CleanEstudianteData(Estudiante est, Dormitorio dorm, Nivelpoder niv, IDataProtector prot)
        {
            string idProtect = null;
            if (prot != null)
            {
                idProtect = prot.Protect(est.IdEstudiante.ToString());
            }
            return new {
                        IdEstudiante = idProtect,
                        NombreEstudiante = est.NombreEstudiante,
                        ApellidoEstudiante = est.ApellidoEstudiante,
                        FechaNacimiento = est.FechaNacimiento,
                        DormitorioEst =  Utils.BuildDicDormitorio(dorm.NumeroDpto, dorm.Piso),
                        NssEstudiante = est.NssEstudiante,
                        ActivoOInactivo = est.ActivoOInactivo,
                        Nivelpoder = niv.NombreNivel
                    };
        }

        public IQueryable<object> GetPowersByEstudiante(string id)
        {
            var idStr = id.Count() > LENT ? _protector.Unprotect(id) : id;
            try
            {
                return from pod in _context.Poderes
                            join pod_est in _context.PoderesEstudiantes on pod.IdPoder equals pod_est.FkPoderEst
                            where pod_est.FkEstudiantePod == int.Parse(idStr)
                            select ServicePoderes.CleanPoderesData(pod);
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public IQueryable<object> GetLeccionesPublicasByIdEstu(string id)
        {
            try
            {
                var idStr = id.Count() > LENT ? _protector.Unprotect(id) : id;
                var lecc = from lec in _context.Leccionpublicas
                            join te in _context.Profesores on lec.FkProfesorLpub equals te.IdProfesor
                            join lec_est in _context.LeccionesEstudiantes on lec.IdLeccionpub equals lec_est.FkLeccionEst
                            where lec_est.FkEstudianteLec == int.Parse(idStr)
                            select ServiceLecPublicas.CleanLecPubliData(lec, te);
                return lecc;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public IQueryable<object> GetLeccionesPrivadasByIdEstu(string id)
        {
            try
            {
                var idStr = id.Count() > LENT ? _protector.Unprotect(id) : id;
                var leccs = from lec in _context.Leccionprivada
                            join te in _context.Profesores on lec.FkProfesorLpriv equals te.IdProfesor
                            join es in _context.Estudiantes on lec.FkEstudianteLpriv equals es.IdEstudiante
                            where lec.FkEstudianteLpriv == int.Parse(idStr)
                            select ServiceLecPrivadas.CleanLecPrivadaData(lec, te, es);
                return leccs;
            }
            catch (System.Exception)
            {
                
                throw;
            }
            throw new NotImplementedException();
        }


        public static bool ComparePowers(List<int> powers_sort, List<PoderesEstudiante> pe_sort)
        {
            var p = new LinkedList<int>(powers_sort);
            var p2 = new LinkedList<PoderesEstudiante>(pe_sort.ToList());

            while(p.Count() > 0 && p2.Count() > 0)
            {
                if (p.First() == p2.First().FkPoderEst)
                {
                    p.RemoveFirst();
                    p2.RemoveFirst();
                    continue;
                }
                return false;
            }
            if (p.Count() > 0 || p2.Count() > 0)
                return false;
            return true;
        }

        public IQueryable<object> GetPresentacionesByIdEstu(string id)
        {
            try
            {
                var idStr = id.Count() > LENT ? _protector.Unprotect(id) : id;
                var pres = from pre in _context.Presentaciones
                            join es_pr in _context.PresentacionesEstudiantes on pre.IdPresentacion equals es_pr.FkPresentacionEst
                            where es_pr.FkEstudiantePres == int.Parse(idStr)
                            select ServicePresentaciones.CleanPresentacionData(pre);
                return pres;
            }
            catch (System.Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }
    }
}
