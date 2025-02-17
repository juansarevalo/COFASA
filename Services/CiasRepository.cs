using System.Data;
using CoreContable.Entities;
using CoreContable.Models;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Services;

public interface ICiasRepository
{
    Task<List<CiaResultSet>> GetCias();

    Task<Companias?> GetCiaById(string cod);

    Task<List<CiaResultSet>> GetUserCias(int userId);

    Task<List<CiaResultSet>> CallGetCias(string? filter);

    Task<List<Select2ResultSet>> CallGetCiasForSelect2(string query);

    Task<bool> CallSaveCia(CiaDto cia);

    Task<bool> CallUpdateCia(CiaDto cia);

    Task<CiaResultSet?> GetOneCia(string ciaCod);

    Task<int> GetCount();

    Task<List<Select2ResultSet>> CallGetCofasaCiasForSelect2(
    string codCia, string? query = null, int pageNumber = 1, int pageSize = 10);

    Task<CiaResultSet?> GetCofasaCiaData(string ciaCod);
}

public class CiasRepository(
    DbContext dbContext,
    ILogger<CiasRepository> logger
) : ICiasRepository
{
    public Task<List<CiaResultSet>> GetCias() =>
        dbContext.Companias.Select(cia => new CiaResultSet
        {
            Cod = cia.CodCia,
            RazonSocial = cia.RazonSocial ?? "",
            NomComercial = cia.NomComercial ?? "",
            NRC = cia.NRC ?? ""
        }).ToListAsync();

    public Task<Companias?> GetCiaById(string cod) =>
        dbContext.Companias
            .Where(cia => cia.CodCia == cod)
            .FirstOrDefaultAsync();

    public Task<List<CiaResultSet>> GetUserCias(int userId) =>
        dbContext.Companias
            .Include(c => c.UserCia)
            .Where(c => c.UserCia.Any(uc => uc.UserAppId == userId))
            .Select(cia => new CiaResultSet
            {
                Cod = cia.CodCia,
                RazonSocial = cia.RazonSocial ?? "",
                NomComercial = cia.NomComercial ?? "",
            })
            .ToListAsync();

    public Task<List<CiaResultSet>> CallGetCias(string? filter = "0") => dbContext.Companias
        .FromSqlRaw("SELECT * FROM CONTABLE.Obtener_Empresas({0})", filter)
        .Select(cia => new CiaResultSet
        {
            Cod = cia.CodCia,
            RazonSocial = cia.RazonSocial ?? "",
            NomComercial = cia.NomComercial ?? "",
            NRC = cia.NRC ?? ""
        })
        .ToListAsync();

    public Task<List<Select2ResultSet>> CallGetCiasForSelect2(string query)
    {
        IQueryable<Companias> efQuery;

        if (query.IsNullOrEmpty())
        {
            efQuery = dbContext.Companias;
        }
        else
        {
            efQuery = dbContext.Companias
                .Where(cia => EF.Functions.Like(cia.NomComercial, $"%{query}%")
                              || EF.Functions.Like(cia.RazonSocial, $"%{query}%"));
        }

        return efQuery
            .Select(cia => new Select2ResultSet
            {
                id = cia.CodCia,
                text = cia.NomComercial ?? ""
            })
            .ToListAsync();
    }

    public async Task<bool> CallSaveCia(CiaDto cia)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.Inserta_Empresa";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@COD_CIA", SqlDbType.VarChar) { Value = cia.COD_CIA });
            command.Parameters.Add(new SqlParameter("@RAZON_SOCIAL", SqlDbType.VarChar) { Value = cia.RAZON_SOCIAL==null ? DBNull.Value : cia.RAZON_SOCIAL });
            command.Parameters.Add(new SqlParameter("@NOM_COMERCIAL", SqlDbType.VarChar) { Value = cia.NOM_COMERCIAL==null ? DBNull.Value : cia.NOM_COMERCIAL });
            command.Parameters.Add(new SqlParameter("@NRC", SqlDbType.VarChar) { Value = cia.NRC == null ? DBNull.Value : cia.NRC });
            command.Parameters.Add(new SqlParameter("@UsuarioCreacion", SqlDbType.VarChar) { Value = cia.UsuarioCreacion == null ? DBNull.Value : cia.UsuarioCreacion });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(CallSaveCia));
            return false;
        }
    }

    public async Task<bool> CallUpdateCia(CiaDto cia)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.ActualizarCia";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@CodigoCia", SqlDbType.VarChar) { Value = cia.COD_CIA });
            command.Parameters.Add(new SqlParameter("@RAZON_SOCIAL", SqlDbType.VarChar) { Value = cia.RAZON_SOCIAL==null ? DBNull.Value : cia.RAZON_SOCIAL });
            command.Parameters.Add(new SqlParameter("@NOM_COMERCIAL", SqlDbType.VarChar) { Value = cia.NOM_COMERCIAL==null ? DBNull.Value : cia.NOM_COMERCIAL });
            command.Parameters.Add(new SqlParameter("@NRC", SqlDbType.VarChar) { Value = cia.NRC == null ? DBNull.Value : cia.NRC });
            command.Parameters.Add(new SqlParameter("@UsuarioModificacion", SqlDbType.VarChar) { Value = cia.UsuarioModificacion == null ? DBNull.Value : cia.UsuarioModificacion });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(CallUpdateCia));
            return false;
        }
    }

    public Task<CiaResultSet?> GetOneCia(string ciaCod) {
        try {
            var result = dbContext.Companias
                .Where (cia => cia.CodCia == ciaCod)
                .Select(cia => CiaResultSet.EntityToResultSet(cia))
                .FirstOrDefaultAsync();
            return result;
        } catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetOneCia));
            return Task.FromResult<CiaResultSet?>(null);
        }
    }

    public Task<int> GetCount() => dbContext.Companias.CountAsync();

    public Task<List<Select2ResultSet>> CallGetCofasaCiasForSelect2(string codCia, string? query = null, int pageNumber = 1, int pageSize = 10) {
        IQueryable<Companias> efQuery;

        if (query.IsNullOrEmpty()) {
            efQuery = dbContext.Companias
                .FromSqlRaw("SELECT * FROM contable.fn_get_codigos_compania()");
        }
        else {
            efQuery = dbContext.Companias
                .FromSqlRaw("SELECT * FROM contable.fn_get_codigos_compania()")
                .Where(cia => EF.Functions.Like(cia.CodCia, $"%{query}%")
                              || EF.Functions.Like(cia.NomComercial, $"%{query}%"));
        }

        return efQuery
            .Select(cia => new Select2ResultSet {
                id = cia.CodCia,
                text = cia.NomComercial ?? ""
            })
            .ToListAsync();
    }

    public Task<CiaResultSet?> GetCofasaCiaData(string ciaCod) {
        try {
            var result = dbContext.Companias
                .FromSqlRaw("SELECT * FROM contable.fn_get_compania({0})", ciaCod)
                .Select(cia => CiaResultSet.EntityToResultSet(cia))
                .FirstOrDefaultAsync();
            return result;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetOneCia));
            return Task.FromResult<CiaResultSet?>(null);
        }
    }
}