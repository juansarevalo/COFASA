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

    Task<Cias?> GetCiaById(string cod);

    Task<List<CiaResultSet>> GetUserCias(int userId);

    Task<List<CiaResultSet>> CallGetCias(string? filter);

    Task<List<Select2ResultSet>> CallGetCiasForSelect2(string query);

    Task<bool> CallSaveCia(CiaDto cia);

    Task<bool> CallUpdateCia(CiaDto cia);

    Task<string> CallGenerateCiaCod();

    Task<CiaResultSet?> GetOneCia(string ciaCod);

    Task<int> GetCount();
}

public class CiasRepository(
    DbContext dbContext,
    ILogger<CiasRepository> logger
) : ICiasRepository
{
    public Task<List<CiaResultSet>> GetCias() =>
        dbContext.Cias.Select(cia => new CiaResultSet
        {
            Cod = cia.CodCia,
            RazonSocial = cia.RazonSocial ?? "",
            NomComercial = cia.NomComercial ?? "",
            CodCiaCore = cia.CodCiaCore ?? ""
        }).ToListAsync();

    public Task<Cias?> GetCiaById(string cod) =>
        dbContext.Cias
            .Where(cia => cia.CodCia == cod)
            .FirstOrDefaultAsync();

    public Task<List<CiaResultSet>> GetUserCias(int userId) =>
        dbContext.Cias
            .Include(c => c.UserCia)
            .Where(c => c.UserCia.Any(uc => uc.UserAppId == userId))
            .Select(cia => new CiaResultSet
            {
                Cod = cia.CodCia,
                RazonSocial = cia.RazonSocial ?? "",
                NomComercial = cia.NomComercial ?? "",
                CodCiaCore = cia.CodCiaCore ?? ""
            })
            .ToListAsync();

    public Task<List<CiaResultSet>> CallGetCias(string? filter = "0") => dbContext.Cias
        .FromSqlRaw("SELECT * FROM CONTABLE.Obtener_Empresas({0})", filter)
        .Select(cia => new CiaResultSet
        {
            Cod = cia.CodCia,
            RazonSocial = cia.RazonSocial ?? "",
            NomComercial = cia.NomComercial ?? "",
            CodCiaCore = cia.CodCiaCore ?? ""
        })
        .ToListAsync();

    public Task<List<Select2ResultSet>> CallGetCiasForSelect2(string query)
    {
        IQueryable<Cias> efQuery;

        if (query.IsNullOrEmpty())
        {
            efQuery = dbContext.Cias;
        }
        else
        {
            efQuery = dbContext.Cias
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

    public async Task<string> CallGenerateCiaCod()
    {
        try
        {
            var command = dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT [CONTABLE].[Obtener_Codigo_Cia] ()";

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            var result = (string)(await command.ExecuteScalarAsync())! ?? "";
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(CallGenerateCiaCod));
            throw;
        }
    }

    public Task<CiaResultSet?> GetOneCia(string ciaCod) {
        try {
            var result = dbContext.Cias
                // .FromSql($"SELECT * FROM CONTABLE.ObtenerDatosEmpresa({ciaCod})")
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

    public Task<int> GetCount() => dbContext.Cias.CountAsync();
}