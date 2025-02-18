using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities.FuntionResult;

[Keyless]
public class ConsultarCofasaCatalogoFromFunc {
    [MaxLength(70)]
    public required string DESCRIP_ESP { get; set; }
    [MaxLength(70)]
    public string? DESCRIP_ING { get; set; }

    public string? ACEP_MOV { get; set; }
    public string? GRUPO_CTA { get; set; }
    public string? CLASE_SALDO { get; set; }
    public int? Grupo_Nivel { get; set; }
    public string? Sub_Grupo { get; set; }
}