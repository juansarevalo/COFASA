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

    public required int TipoEntrada { get; set; }

    [MaxLength(15)]
    public required string CodContable { get; set; }

    [MaxLength(5)]
    public required string TipoCuenta { get; set; }

    [MaxLength(30)]
    public required string FormaCalculo { get; set; }
}