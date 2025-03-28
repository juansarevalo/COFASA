using System.Data;
using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Services;

public interface ITipoMovCuentasRepository {
    Task<List<TipoMovCuentas>> GetAll (string codCia, string? query);

    Task<List<Select2ResultSet>> CallGetCofasaIdTipoMovForSelect2(string NombreMov, string? query = null, int pageNumber = 1, int pageSize = 10);

    Task<List<Select2ResultSet>> CallGetCofasaIdPaisForSelect2(string? query = null);

    Task<bool> SaveOrUpdate(TipoMovCuentas data);

    Task<TipoMovCuentas?> GetOne(int id);
}

public class TipoMovCuentasRepository(
    DbContext dbContext,
    ILogger<CentroCostoRepository> logger
) : ITipoMovCuentasRepository {

    public Task<List<TipoMovCuentas>> GetAll (string codCia, string? query) {
        if (query.IsNullOrEmpty ( )) {
            return dbContext.TipoMovCuentas
                .FromSqlRaw("SELECT * FROM [CONTABLE].[fn_get_tipo_mov_cuentas]({0})", DBNull.Value)
                .Where(entity => entity.CodCia == codCia)
                .ToListAsync ( );
        }

        return dbContext.TipoMovCuentas
                .Where(entity => entity.CodCia == codCia)
                .ToListAsync();
    }

    public Task<List<Select2ResultSet>> CallGetCofasaIdTipoMovForSelect2(string NombreMov, string? query = null, int pageNumber = 1, int pageSize = 10) {
        try {
            IQueryable<CofasaTipoMov> efQuery = dbContext.CofasaTipoMov
                .FromSqlRaw("EXEC [CONTABLE].[fn_get_tipo_mov] @NombreMov = {0}", NombreMov);

            var result = efQuery.AsEnumerable();

            if (!query.IsNullOrEmpty()) {
                result = result
                    .Where(entity => entity.nombre.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            var count = result.Count();

            var paginatedList = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(entity => new Select2ResultSet {
                    id = entity.idTipoMov.ToString(),
                    text = entity.idTipoMov + " - " + entity.nombre,
                    more = pageNumber * pageSize < count
                })
                .ToList();

            return Task.FromResult(paginatedList);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCuentasRepository), nameof(CallGetCofasaIdTipoMovForSelect2));
            return Task.FromResult(new List<Select2ResultSet>());
        }
    }

    public Task<List<Select2ResultSet>> CallGetCofasaIdPaisForSelect2(string? query = null) {
        try {
            IQueryable<CofasaPais> efQuery = dbContext.CofasaPais
                .FromSqlRaw("SELECT * FROM [CONTABLE].[fn_get_pais]({0})", DBNull.Value);

            var result = efQuery.AsEnumerable();

            if (!query.IsNullOrEmpty()) {
                result = result
                    .Where(entity => entity.nombre.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            var count = result.Count();

            var paginatedList = result
                .Select(entity => new Select2ResultSet {
                    id = entity.idPais.ToString(),
                    text = entity.idPais + " - " + entity.nombre,
                    more = false
                })
                .ToList();

            return Task.FromResult(paginatedList);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCuentasRepository), nameof(CallGetCofasaIdTipoMovForSelect2));
            return Task.FromResult(new List<Select2ResultSet>());
        }
    }

    public async Task<bool> SaveOrUpdate(TipoMovCuentas data) {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try {
            command.CommandText = $"{CC.SCHEMA}.InsertarOActualizaTipoMovCuenta";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = data.Id });
            command.Parameters.Add(new SqlParameter("@CodCia", SqlDbType.VarChar) { Value = data.CodCia });
            command.Parameters.Add(new SqlParameter("@NombreMov", SqlDbType.VarChar) { Value = data.NombreMov });
            command.Parameters.Add(new SqlParameter("@IdTipoMov", SqlDbType.VarChar) { Value = data.IdTipoMov });
            command.Parameters.Add(new SqlParameter("@IdCatalogo", SqlDbType.VarChar) { Value = data.IdCatalogo });
            command.Parameters.Add(new SqlParameter("@CentroCosto", SqlDbType.VarChar) { Value = data.CentroCostoF });
            command.Parameters.Add(new SqlParameter("@TipoCuenta", SqlDbType.VarChar) { Value = data.TipoCuenta });
            command.Parameters.Add(new SqlParameter("@IdTipoPartida", SqlDbType.VarChar) { Value = data.IdTipoPartida });
            command.Parameters.Add(new SqlParameter("@FormaCalculo", SqlDbType.VarChar) { Value = data.FormaCalculo });
            command.Parameters.Add(new SqlParameter("@IdPais", SqlDbType.Int) { Value = data.IdPais });
            command.Parameters.Add(new SqlParameter("@ParteRel", SqlDbType.Char) { Value = data.ParteRel ?? "N" });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e) {
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoMovCuentasRepository), nameof(SaveOrUpdate));
            return false;
        }
    }

    public Task<TipoMovCuentas?> GetOne(int id) =>
        dbContext.TipoMovCuentas
            .FromSqlRaw("SELECT * FROM [CONTABLE].[fn_get_tipo_mov_cuentas]({0})", id)
            .FirstOrDefaultAsync();
}