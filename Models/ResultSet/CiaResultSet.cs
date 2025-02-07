using CoreContable.Entities;

namespace CoreContable.Models.ResultSet;

public class CiaResultSet {
    public required string Cod { get; set; }

    public required string RazonSocial { get; set; }

    public required string NomComercial { get; set; }

    public required string CodCiaCore { get; set; }

    public static CiaResultSet EntityToResultSet(Companias cia) {
        return new CiaResultSet {
            Cod = cia.CodCia,
            RazonSocial = cia.RazonSocial ?? "",
            NomComercial = cia.NomComercial ?? "",
            CodCiaCore = cia.CodCiaCore ?? ""
        };
    }

    public static CiaResultSet PartialEntityToResultSet(Companias cia) {
        return new CiaResultSet {
            Cod = cia.CodCia,
            RazonSocial = cia.RazonSocial ?? "",
            NomComercial = cia.NomComercial ?? "",
            CodCiaCore = cia.CodCiaCore ?? ""
        };
    }
}