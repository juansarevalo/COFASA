using CoreContable.Entities;
using CoreContable.Entities.FuntionResult;

namespace CoreContable.Models.ResultSet;

public class CofasaCatalogoResultSet {
    public string? DESCRIP_ESP { get; set; }
    public string? DESCRIP_ING { get; set; }

    public string? ACEP_MOV { get; set; }
    public string? GRUPO_CTA { get; set; }

    public string? Sub_Grupo { get; set; }
    public int? Grupo_Nivel { get; set; }

    public string? CLASE_SALDO { get; set; }

    public static CofasaCatalogoResultSet EntityToResultSet(ConsultarCofasaCatalogoFromFunc entity) {
        return new CofasaCatalogoResultSet {
            DESCRIP_ESP = entity.DESCRIP_ESP ?? "",
            DESCRIP_ING = entity.DESCRIP_ING ?? "",
            ACEP_MOV = entity.ACEP_MOV ?? "",
            GRUPO_CTA = entity.GRUPO_CTA ?? "",
            CLASE_SALDO = entity.CLASE_SALDO ?? "",        
            Grupo_Nivel = entity.Grupo_Nivel,
            Sub_Grupo = entity.Sub_Grupo ?? "",
        };
    }
}