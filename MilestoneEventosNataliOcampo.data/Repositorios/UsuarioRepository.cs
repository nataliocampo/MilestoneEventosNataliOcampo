
using Microsoft.EntityFrameworkCore;
using MilestoneEventosNataliOcampo.data.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using MilestoneEventosNataliOcampo.data.Db;
namespace MilestoneEventosNataliOcampo.data.Repositorios;

public interface IUsuarioRepository
{
    Usuario ObtenerOCrearUsuario(string email, string nombre);
    List<string> ObtenerPermisos(string email);
}

public class UsuarioRepository : IUsuarioRepository
{
    private readonly DbTurnoContext _db;

    public UsuarioRepository(DbTurnoContext db)
    {
        _db = db;
    }

    public List<string> ObtenerPermisos(string email)
    {
        var usuario = _db.Usuarios.FirstOrDefault(u => u.Email == email);

        if (usuario == null)
        {
            return new List<string>();
        }

        return _db.RolPermiso
            .Where(rp => rp.RolId == usuario.RolId)
            .Select(rp => rp.Permiso.NombrePermiso)
            .Distinct()
            .ToList();
    }

    public Usuario ObtenerOCrearUsuario(string email, string nombre)
    {
        var usuario = _db.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefault(u => u.Email == email);

        if (usuario != null)
        {
            return usuario;
        }

        var rolPaciente = _db.Rol.First(r => r.NombreRol == "Paciente");

        usuario = new Usuario
        {
            Email = email,
            RolId = rolPaciente.RolId,
            Rol = rolPaciente,
            FechaAlta = DateTime.Now
        };

        _db.Usuarios.Add(usuario);
        _db.SaveChanges();

        return usuario;
    }
}

