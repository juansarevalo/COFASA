using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.TIPOPARTIDA, Schema = CC.SCHEMA)]
public class TipoPartidaC
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required int IdTipoPartida { get; set; }
    [MaxLength(3)]
    public required string CodCia { get; set; }

    public string? TipoPartida { get; set; }
    public string? TipoHomologar { get; set; }
    public string? Nombre { get; set; }
    public string? UsuarioCreacion { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public string? UsuarioModificacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}