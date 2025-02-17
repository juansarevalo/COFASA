using CoreContable.Entities;

namespace CoreContable.Models.ResultSet;

public class DmgCuentasResultSet {
    public required string COD_CIA { get; set; }

    public required int CTA_1 { get; set; }
    public required int CTA_2 { get; set; }
    public required int CTA_3 { get; set; }
    public required int CTA_4 { get; set; }
    public required int CTA_5 { get; set; }
    public required int CTA_6 { get; set; }

    public string? DESCRIP_ESP { get; set; }
    public string? DESCRIP_ING { get; set; }

    public string? ACEP_MOV { get; set; }
    public string? ACEP_PRESUP { get; set; }
    public string? ACEP_ACTIV { get; set; }
    public string? GRUPO_CTA { get; set; }

    public string? Sub_Grupo { get; set; }
    public int? Grupo_Nivel { get; set; }

    public string? CLASE_SALDO { get; set; }

    public int? CTA_1P { get; set; }
    public int? CTA_2P { get; set; }
    public int? CTA_3P { get; set; }
    public int? CTA_4P { get; set; }
    public int? CTA_5P { get; set; }
    public int? CTA_6P { get; set; }

    public string? CTA_FLUJO { get; set; }
    public string? UTIL_CTA { get; set; }
    public string? ACEP_PRESUP_COMPRAS { get; set; }
    public string? BANDERA { get; set; }

    public string? Catalogo { get; set; }

    public static DmgCuentasResultSet EntityToResultSet(DmgCuentas entity) {
        return new DmgCuentasResultSet {
            COD_CIA = entity.COD_CIA,
            CTA_1 = entity.CTA_1,
            CTA_2 = entity.CTA_2,
            CTA_3 = entity.CTA_3,
            CTA_4 = entity.CTA_4,
            CTA_5 = entity.CTA_5,
            CTA_6 = entity.CTA_6,
            DESCRIP_ESP = entity.DESCRIP_ESP ?? "",
            DESCRIP_ING = entity.DESCRIP_ING ?? "",
            ACEP_MOV = entity.ACEP_MOV ?? "",
            ACEP_PRESUP = entity.ACEP_PRESUP ?? "",
            ACEP_ACTIV = entity.ACEP_ACTIV ?? "",
            GRUPO_CTA = entity.GRUPO_CTA ?? "",
            CLASE_SALDO = entity.CLASE_SALDO ?? "",
            CTA_1P = entity.CTA_1P,
            CTA_2P = entity.CTA_2P,
            CTA_3P = entity.CTA_3P,
            CTA_4P = entity.CTA_4P,
            CTA_5P = entity.CTA_5P,
            CTA_6P = entity.CTA_6P,
            CTA_FLUJO = entity.CTA_FLUJO ?? "",
            UTIL_CTA = entity.UTIL_CTA ?? "",
            ACEP_PRESUP_COMPRAS = entity.ACEP_PRESUP_COMPRAS ?? "",
            BANDERA = entity.BANDERA ?? "",
            Catalogo = entity.Catalogo ?? "",
            Grupo_Nivel = entity.Grupo_Nivel,
            Sub_Grupo = entity.Sub_Grupo ?? "",
        };
    }
}