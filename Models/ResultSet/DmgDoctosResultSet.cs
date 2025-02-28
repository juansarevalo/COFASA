using CoreContable.Entities;

namespace CoreContable.Models.ResultSet;

public class DmgDoctosResultSet
{
    public DmgDoctosResultSet(TipoPartidaC entity)
    {
        IdTipoPartida = entity.IdTipoPartida;
        CodCia = entity.CodCia;
        TipoPartida = entity.TipoPartida;
        TipoHomologar = entity.TipoHomologar;
        Nombre = entity.Nombre;
        UsuarioCreacion = entity.UsuarioCreacion;
        FechaCreacion = entity.FechaCreacion;
        UsuarioModificacion = entity.UsuarioModificacion;
        FechaModificacion = entity.FechaModificacion;
    }

    public DmgDoctosResultSet()
    {
    }

    public int? IdTipoPartida { get; set; }
    public string? CodCia { get; set; }
    public string? TipoPartida { get; set; }
    public string? TipoHomologar { get; set; }
    public string? Nombre { get; set; }
    public string? UsuarioCreacion { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public string? UsuarioModificacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}