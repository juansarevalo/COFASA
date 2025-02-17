using CoreContable.Entities;

namespace CoreContable.Models.ResultSet;

public class CiaResultSet {
    public required string Cod { get; set; }

    public required string RazonSocial { get; set; }

    public required string NomComercial { get; set; }

    public string? NRC { get; set; }

    public static CiaResultSet EntityToResultSet(Companias cia) {
        return new CiaResultSet {
            Cod = cia.CodCia,
            RazonSocial = cia.RazonSocial ?? "",
            NomComercial = cia.NomComercial ?? "",
            NRC = cia.NRC ?? ""
        };
    }
}