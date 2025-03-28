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
    public required string NombreMov { get; set; }

    public required int IdTipoMov { get; set; }

    [MaxLength(15)]
    public required string NombreTipoMov { get; set; }

    public required int IdCatalogo { get; set; }

    [MaxLength(15)]
    public required string codContable { get; set; }

    [MaxLength(80)]
    public required string NombreCatalogo { get; set; }

    [MaxLength(9)]
    public required string CentroCostoF { get; set; }

    [MaxLength(200)]
    public required string NombreCentroCosto { get; set; }

    [MaxLength(5)]
    public required string TipoCuenta { get; set; }

    [ForeignKey(CC.TIPOPARTIDA)]
    public required int IdTipoPartida { get; set; }

    [MaxLength(100)]
    public required string TipoPartida { get; set; }

    [MaxLength(30)]
    public required string FormaCalculo { get; set; }

    public required int IdPais {  get; set; }

    [MaxLength(50)]
    public required string NombrePais { get; set; }

    [MaxLength(1)]
    public required string ParteRel { get; set; }
}