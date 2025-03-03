using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.TIPOENTRADACUENTAS, Schema = CC.SCHEMA)]
public class TipoEntradaCuentas
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; set; }

    [MaxLength(3)]
    public required string CodCia { get; set; }

    public required int NumTipoEntrada { get; set; }

    [MaxLength(25)]
    public required string TipoEntrada { get; set; }

    [MaxLength(15)]
    public required string IdCatalogo { get; set; }

    [MaxLength(9)]
    public required string CentroCosto { get; set; }

    [MaxLength(5)]
    public required string TipoCuenta { get; set; }

    [ForeignKey(CC.TIPOPARTIDA)]
    public required int IdTipoPartida { get; set; }

    [MaxLength(30)]
    public required string FormaCalculo { get; set; }
}