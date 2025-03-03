using System.Data;
using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Services;

public interface ITipoEntradaCuentasRepository {
    Task<List<TipoEntradaCuentas>> GetAll (string codCia, string? query);

    Task<bool> SaveOrUpdate(TipoEntradaCuentas data);

    Task<TipoEntradaCuentas?> GetOne(int id);
}

public class TipoEntradaCuentasRepository(
    DbContext dbContext,
    ILogger<CentroCostoRepository> logger
) : ITipoEntradaCuentasRepository {

    public Task<List<TipoEntradaCuentas>> GetAll (string codCia, string? query) {
        if (query.IsNullOrEmpty ( )) {
            return dbContext.TipoEntradaCuentas
                .Where(entity => entity.CodCia == codCia)
                .ToListAsync ( );
        }

        return dbContext.TipoEntradaCuentas
                .Where(entity => entity.CodCia == codCia)
                .ToListAsync();
    }

    public async Task<bool> SaveOrUpdate(TipoEntradaCuentas data) {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try {
            command.CommandText = $"{CC.SCHEMA}.InsertarOActualizaTipoEntradaCuenta";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = data.Id });
            command.Parameters.Add(new SqlParameter("@CodCia", SqlDbType.VarChar) { Value = data.CodCia });
            command.Parameters.Add(new SqlParameter("@NumTipoEntrada", SqlDbType.Int) { Value = data.NumTipoEntrada });
            command.Parameters.Add(new SqlParameter("@TipoEntrada", SqlDbType.VarChar) { Value = data.TipoEntrada });
            command.Parameters.Add(new SqlParameter("@IdCatalogo", SqlDbType.VarChar) { Value = data.IdCatalogo });
            command.Parameters.Add(new SqlParameter("@CentroCosto", SqlDbType.VarChar) { Value = data.CentroCosto });
            command.Parameters.Add(new SqlParameter("@TipoCuenta", SqlDbType.VarChar) { Value = data.TipoCuenta });
            command.Parameters.Add(new SqlParameter("@IdTipoPartida", SqlDbType.VarChar) { Value = data.IdTipoPartida });
            command.Parameters.Add(new SqlParameter("@FormaCalculo", SqlDbType.VarChar) { Value = data.FormaCalculo });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e) {
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoEntradaCuentasRepository), nameof(SaveOrUpdate));
            return false;
        }
    }

    public Task<TipoEntradaCuentas?> GetOne(int id) =>
        dbContext.TipoEntradaCuentas
            .Where(entity => entity.Id == id)
            .FirstOrDefaultAsync();
}