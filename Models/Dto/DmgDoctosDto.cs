namespace CoreContable.Models.Dto;

public class DmgDoctosDto
{
    public int IdTipoPartida { get; set; }
    public string? CodCia { get; set; }
    public string? TipoPartida { get; set; }
    public string? TipoHomologar { get; set; }
    public string? Nombre { get; set; }
    public string? UsuarioCreacion { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public string? UsuarioModificacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}