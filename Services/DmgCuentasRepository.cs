using System.Data;
using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Services;

public interface IDmgCuentasRepository
{
    Task<List<DmgCuentasResultSet>> GetAllByCia(string codCia);

    Task<DmgCuentasResultSet?> GetOne(GetAccountNameDto data);

    Task<bool> SaveOrUpdate(DmgCuentasDto data);

    Task<string> GetName(GetAccountNameDto data);

    Task<string> GetCoreContableAccountFromCONTABLEAccount(string codCia, string CONTABLEAccount);

    Task<bool> GenerarPartidaLiquidacion (string codCia, int año);

    Task<bool> MayorizarMes (string codCia, int periodo, int año, int mes);

    Task<List<Select2ResultSet>> CallGetCofasaCatalogoForSelect2(string? query = null, int pageNumber = 1, int pageSize = 10);

    Task<CofasaCatalogo?> GetCofasaCatalogDataById(int id);
}

public class DmgCuentasRepository(
    DbContext dbContext,
    ILogger<DmgCuentasRepository> logger
) : IDmgCuentasRepository
{
    public Task<List<DmgCuentasResultSet>> GetAllByCia(string codCia) =>
        dbContext.DmgCuentas
            .FromSqlRaw("SELECT * FROM CONTABLE.Obtener_Cuentas({0})", codCia)
            .Select(entity => new DmgCuentasResultSet
            {
                COD_CIA = entity.COD_CIA,
                CTA_1 = entity.CTA_1,
                CTA_2 = entity.CTA_2,
                CTA_3 = entity.CTA_3,
                CTA_4 = entity.CTA_4,
                CTA_5 = entity.CTA_5,
                CTA_6 = entity.CTA_6,
                DESCRIP_ESP = entity.DESCRIP_ESP
            })
            .ToListAsync();

    public Task<DmgCuentasResultSet?> GetOne(GetAccountNameDto data) =>
        dbContext.DmgCuentas
            .FromSqlRaw(
                "SELECT * FROM CONTABLE.Obtener_Cuenta({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                data.CodCia,
                data.Cta1,
                data.Cta2,
                data.Cta3,
                data.Cta4,
                data.Cta5,
                data.Cta6
            )
            .Select(entity => DmgCuentasResultSet.EntityToResultSet(entity))
            .FirstOrDefaultAsync();

    public async Task<bool> SaveOrUpdate (DmgCuentasDto data) {

        var command = dbContext.Database.GetDbConnection ( ).CreateCommand ( );

        try {

            command.CommandText = $"{CC.SCHEMA}.InsertarOActualizarCuenta";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add (new SqlParameter ("@COD_CIA", SqlDbType.VarChar) { Value = data.COD_CIA == null ? DBNull.Value : data.COD_CIA });
            command.Parameters.Add (new SqlParameter ("@CTA_1", SqlDbType.VarChar) { Value = data.CTA_1 == null ? DBNull.Value : data.CTA_1 });
            command.Parameters.Add (new SqlParameter ("@CTA_2", SqlDbType.VarChar) { Value = data.CTA_2 == null ? DBNull.Value : data.CTA_2 });
            command.Parameters.Add (new SqlParameter ("@CTA_3", SqlDbType.VarChar) { Value = data.CTA_3 == null ? DBNull.Value : data.CTA_3 });
            command.Parameters.Add (new SqlParameter ("@CTA_4", SqlDbType.VarChar) { Value = data.CTA_4 == null ? DBNull.Value : data.CTA_4 });
            command.Parameters.Add (new SqlParameter ("@CTA_5", SqlDbType.VarChar) { Value = data.CTA_5 == null ? DBNull.Value : data.CTA_5 });
            command.Parameters.Add (new SqlParameter ("@CTA_6", SqlDbType.VarChar) { Value = data.CTA_6 == null ? DBNull.Value : data.CTA_6 });
            command.Parameters.Add (new SqlParameter ("@DESCRIP_ESP", SqlDbType.VarChar) { Value = data.DESCRIP_ESP == null ? DBNull.Value : data.DESCRIP_ESP });
            command.Parameters.Add (new SqlParameter ("@DESCRIP_ING", SqlDbType.VarChar) { Value = data.DESCRIP_ING == null ? DBNull.Value : data.DESCRIP_ING });
            command.Parameters.Add (new SqlParameter ("@ACEP_MOV", SqlDbType.VarChar) { Value = data.ACEP_MOV ?? "N" });
            command.Parameters.Add (new SqlParameter ("@ACEP_PRESUP", SqlDbType.VarChar) { Value = data.ACEP_PRESUP ?? "N" });
            command.Parameters.Add (new SqlParameter ("@ACEP_ACTIV", SqlDbType.VarChar) { Value = data.ACEP_ACTIV ?? "N" });
            command.Parameters.Add (new SqlParameter ("@GRUPO_CTA", SqlDbType.VarChar) { Value = data.GRUPO_CTA ?? "NO DIFINIDO" });
            command.Parameters.Add (new SqlParameter ("@CLASE_SALDO", SqlDbType.VarChar) { Value = data.CLASE_SALDO == null ? DBNull.Value : data.CLASE_SALDO });
            command.Parameters.Add (new SqlParameter ("@CTA_1P", SqlDbType.VarChar) { Value = data.CTA_1P == null ? DBNull.Value : data.CTA_1P });
            command.Parameters.Add (new SqlParameter ("@CTA_2P", SqlDbType.VarChar) { Value = data.CTA_2P == null ? DBNull.Value : data.CTA_2P });
            command.Parameters.Add (new SqlParameter ("@CTA_3P", SqlDbType.VarChar) { Value = data.CTA_3P == null ? DBNull.Value : data.CTA_3P });
            command.Parameters.Add (new SqlParameter ("@CTA_4P", SqlDbType.VarChar) { Value = data.CTA_4P == null ? DBNull.Value : data.CTA_4P });
            command.Parameters.Add (new SqlParameter ("@CTA_5P", SqlDbType.VarChar) { Value = data.CTA_5P == null ? DBNull.Value : data.CTA_5P });
            command.Parameters.Add (new SqlParameter ("@CTA_6P", SqlDbType.VarChar) { Value = data.CTA_6P == null ? DBNull.Value : data.CTA_6P });
            command.Parameters.Add (new SqlParameter ("@CTA_FLUJO", SqlDbType.VarChar) { Value = data.CTA_FLUJO == null ? DBNull.Value : data.CTA_FLUJO });
            command.Parameters.Add (new SqlParameter ("@UTIL_CTA", SqlDbType.VarChar) { Value = data.UTIL_CTA == null ? DBNull.Value : data.UTIL_CTA });
            command.Parameters.Add (new SqlParameter ("@ACEP_PRESUP_COMPRAS", SqlDbType.VarChar) { Value = data.ACEP_PRESUP_COMPRAS == null ? DBNull.Value : data.ACEP_PRESUP_COMPRAS });
            command.Parameters.Add (new SqlParameter ("@BANDERA", SqlDbType.VarChar) { Value = data.BANDERA == null ? DBNull.Value : data.BANDERA });
            command.Parameters.Add (new SqlParameter ("@UsuarioCreacion", SqlDbType.VarChar) { Value = data.UsuarioCreacion == null ? DBNull.Value : data.UsuarioCreacion });
            command.Parameters.Add (new SqlParameter ("@UsuarioModificacion", SqlDbType.VarChar) { Value = data.UsuarioModificacion == null ? DBNull.Value : data.UsuarioModificacion });
            command.Parameters.Add (new SqlParameter ("@FechaCreacion", SqlDbType.DateTime) { Value = data.FechaCreacion == null ? DBNull.Value : data.FechaCreacion });
            command.Parameters.Add (new SqlParameter ("@FechaModificacion", SqlDbType.DateTime) { Value = data.FechaModificacion == null ? DBNull.Value : data.FechaModificacion });
            command.Parameters.Add (new SqlParameter ("@CATALOGO", SqlDbType.VarChar) { Value = data.Catalogo ?? "N" });
            command.Parameters.Add (new SqlParameter ("@Grupo_Nivel", SqlDbType.VarChar) { Value = data.Grupo_Nivel == null ? DBNull.Value : data.Grupo_Nivel });
            command.Parameters.Add (new SqlParameter ("@Sub_Grupo", SqlDbType.VarChar) { Value = data.Sub_Grupo == null ? DBNull.Value : data.Sub_Grupo });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync ( );
            await command.ExecuteNonQueryAsync ( );
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync ( );

            return true;
        }
        catch (Exception e) {
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync ( );
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (DmgCuentasRepository), nameof (SaveOrUpdate));
            return false;
        }
    }

    public async Task<string> GetName(GetAccountNameDto data)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = "SELECT CONTABLE.nom_cuenta (@p_codcia, @p_cta1, @p_cta2, @p_cta3, @p_cta4, @p_cta5, @p_cta6)";
            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();

            command.Parameters.Add(new SqlParameter("@p_codcia", SqlDbType.VarChar) { Value = data.CodCia });
            command.Parameters.Add(new SqlParameter("@p_cta1", SqlDbType.Int) { Value = data.Cta1 });
            command.Parameters.Add(new SqlParameter("@p_cta2", SqlDbType.Int) { Value = data.Cta2 });
            command.Parameters.Add(new SqlParameter("@p_cta3", SqlDbType.Int) { Value = data.Cta3 });
            command.Parameters.Add(new SqlParameter("@p_cta4", SqlDbType.Int) { Value = data.Cta4 });
            command.Parameters.Add(new SqlParameter("@p_cta5", SqlDbType.Int) { Value = data.Cta5 });
            command.Parameters.Add(new SqlParameter("@p_cta6", SqlDbType.Int) { Value = data.Cta6 });
            
            var result = (string)(await command.ExecuteScalarAsync())! ?? "";
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            return result;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCuentasRepository), nameof(GetName));
            return "";
        }
    }

    public async Task<string> GetCoreContableAccountFromCONTABLEAccount (string codCia, string CONTABLEAccount) {
        try {
            var result = await dbContext.CuentasContablesView
                .Where (entity => entity.Cta_CONTABLE == CONTABLEAccount && entity.COD_CIA == codCia)
                .FirstOrDefaultAsync ( );
            return result?.Cta_Nivel?.ToString ( ) ?? "";
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (DmgCuentasRepository), nameof (GetCoreContableAccountFromCONTABLEAccount));
            return "";
        }
    }

    public async Task<bool> GenerarPartidaLiquidacion (string codCia, int año) {
        try {

            // Mes siempre será diciembre (12)
            int mes = 12;

            // Concepto y documento definidos automáticamente
            string concepto = $"partida por el cierre anual {año}";
            string doc = "PC";

            // Establecer un tiempo de espera personalizado
            dbContext.Database.SetCommandTimeout (120); // Tiempo en segundos

            // Ejecución de la función CrearAsientoCierre
            await dbContext.Database.ExecuteSqlRawAsync (
                "EXEC [CONTABLE].[CrearAsientoCierre] {0}, {1}, {2}, {3}, {4}",
                codCia, año, mes, concepto, doc
            );
            return true;
        }
        catch (Exception e) {
            logger.LogError (e, "Error en {Class}.{Method}", nameof (DmgCuentasRepository), nameof (GenerarPartidaLiquidacion));
            return false;
        }
    }

    public async Task<bool> MayorizarMes(string codCia, int periodo, int año, int mes) {
        try {

            string estado = "R";

            // Establecer un tiempo de espera personalizado
            //dbContext.Database.SetCommandTimeout (1120); // Tiempo en segundos

            // Ejecución de la función EjecutarMayorizacionMasiva
            await dbContext.Database.ExecuteSqlRawAsync (
                "EXEC [CONTABLE].[EjecutarMayorizacionMasiva] {0}, {1}, {2}, {3}, {4}",
                codCia, periodo, año, mes, estado
            );
            return true;
        }
        catch (Exception e) {
            logger.LogError (e, "Error en {Class}.{Method}", nameof (DmgCuentasRepository), nameof (GenerarPartidaLiquidacion));
            return false;
        }
    }

    public Task<List<Select2ResultSet>> CallGetCofasaCatalogoForSelect2(string? query = null, int pageNumber = 1, int pageSize = 10) {
        try {
            IQueryable<CofasaCatalogo> efQuery = dbContext.CofasaCatalogo
                .FromSqlRaw("SELECT * FROM contable.fn_get_catalogo({0})", DBNull.Value);

            if (!query.IsNullOrEmpty()) {
                efQuery = dbContext.CofasaCatalogo
                .FromSqlRaw("SELECT * FROM contable.fn_get_catalogo({0})", DBNull.Value)                    
                .Where(
                        entity =>
                            EF.Functions.Like(entity.codContable, $"%{query}%")
                            || EF.Functions.Like(entity.DESCRIP_ESP, $"%{query}%")
                );
            }

            var count = efQuery.Count();

            return efQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(entity => new Select2ResultSet {
                    id = entity.idCatalogo.ToString(),
                    text = entity.codContable + " - " + entity.DESCRIP_ESP,
                    more = pageNumber * pageSize < count
                })
                .ToListAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCuentasRepository), nameof(CallGetCofasaCatalogoForSelect2));
            return Task.FromResult(new List<Select2ResultSet>());
        }
    }

    public Task<CofasaCatalogo?> GetCofasaCatalogDataById(int id) {
        try {
            var result = dbContext.CofasaCatalogo
                .FromSqlRaw("SELECT * FROM contable.fn_get_catalogo({0})", id)
                .FirstOrDefaultAsync();
            return result;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetCofasaCatalogDataById));
            return Task.FromResult<CofasaCatalogo?>(null);
        }
    }
}