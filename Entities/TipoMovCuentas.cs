using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.TIPOMOVCUENTAS, Schema = CC.SCHEMA)]
public class TipoMovCuentas
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; set; }

    [MaxLength(3)]
    public required string CodCia { get; set; }

    [MaxLength(10)]
    public required string TipoMov { get; set; }

    public required int IdTipoMov { get; set; }

    public required int IdCatalogo { get; set; }

    [MaxLength(9)]
    public required string CentroCostoF { get; set; }

    [MaxLength(5)]
    public required string TipoCuenta { get; set; }

    [ForeignKey(CC.TIPOPARTIDA)]
    public required int IdTipoPartida { get; set; }

    [MaxLength(30)]
    public required string FormaCalculo { get; set; }

    public virtual CentroCosto? CentroCosto { get; set; }

    public virtual TipoPartidaC? TipoPartida { get; set; }
}