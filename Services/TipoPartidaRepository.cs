using System.Data;
using System.Data.Common;
using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Services;

public interface ITipoPartidaRepository
{
    Task<List<DmgDoctosResultSet>> GetAllBy(string codCia, string query);

    Task<List<DmgDoctosResultSet>> GetDmgDoctosByCia(string codCia);

    Task<DmgDoctosResultSet?> GetOneDmgDoctoByCia(string codCia, int IdTipoPartida);

    Task<bool> SaveOrUpdateDmgDocto(DmgDoctosDto data);
    
    Task<List<Select2ResultSet>> GetForSelect2(
        string codCia, string? query = null, int pageNumber = 1, int pageSize = 10);
}

public class TipoPartidaRepository(
    DbContext dbContext,
    ILogger<TipoPartidaRepository> logger
) : ITipoPartidaRepository
{
    public Task<List<DmgDoctosResultSet>> GetAllBy(string codCia, string query)
    {
        try
        {
            if (query.IsNullOrEmpty())
            {
                return dbContext.TipoPartida
                    .Where(entity => entity.CodCia == codCia)
                    .Select(entity => new DmgDoctosResultSet
                    {
                        IdTipoPartida = entity.IdTipoPartida,
                        CodCia = entity.CodCia,
                        TipoPartida = entity.TipoPartida,
                        Nombre = entity.Nombre,
                        UsuarioCreacion = entity.UsuarioCreacion,
                        FechaCreacion = entity.FechaCreacion,
                        UsuarioModificacion = entity.UsuarioModificacion,
                        FechaModificacion = entity.FechaModificacion
                    })
                    .ToListAsync();
            }

            return dbContext.TipoPartida
                .Where(entity => entity.CodCia == codCia)
                .Where(entity => EF.Functions.Like(entity.Nombre, $"%{query}%"))
                .Select(entity => new DmgDoctosResultSet
                {
                    CodCia = entity.CodCia,
                    TipoPartida = entity.TipoPartida,
                    Nombre = entity.Nombre,
                    UsuarioCreacion = entity.UsuarioCreacion,
                    FechaCreacion = entity.FechaCreacion,
                    UsuarioModificacion = entity.UsuarioModificacion,
                    FechaModificacion = entity.FechaModificacion
                })
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoPartidaRepository), nameof(GetAllBy));
            return Task.FromResult(new List<DmgDoctosResultSet>());
        }
    }

    public Task<List<DmgDoctosResultSet>> GetDmgDoctosByCia(string codCia)
    {
        try
        {
            return dbContext.TipoPartida
                .FromSqlRaw("SELECT * FROM CONTABLE.ConsultarDocumentos({0}, {1})", codCia, DBNull.Value)
                .Select(entity => new DmgDoctosResultSet()
                {
                    IdTipoPartida = entity.IdTipoPartida,
                    CodCia = entity.CodCia,
                    TipoPartida = entity.TipoPartida,
                    TipoHomologar = entity.TipoHomologar,
                    Nombre = entity.Nombre,
                    UsuarioCreacion = entity.UsuarioCreacion,
                    FechaCreacion = entity.FechaCreacion,
                    UsuarioModificacion = entity.UsuarioModificacion,
                    FechaModificacion = entity.FechaModificacion
                })
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoPartidaRepository), nameof(GetDmgDoctosByCia));
            return Task.FromResult(new List<DmgDoctosResultSet>());
        }
    }

    public Task<DmgDoctosResultSet?> GetOneDmgDoctoByCia(string codCia, int IdTipoPartida)
    {
        try
        {
            return dbContext.TipoPartida
                .FromSqlRaw("SELECT * FROM CONTABLE.ConsultarDocumentos({0}, {1})", codCia, IdTipoPartida)
                .Select(entity => new DmgDoctosResultSet()
                {
                    IdTipoPartida = entity.IdTipoPartida,
                    CodCia = entity.CodCia,
                    TipoPartida = entity.TipoPartida,
                    TipoHomologar = entity.TipoHomologar,
                    Nombre = entity.Nombre,
                    UsuarioCreacion = entity.UsuarioCreacion,
                    FechaCreacion = entity.FechaCreacion,
                    UsuarioModificacion = entity.UsuarioModificacion,
                    FechaModificacion = entity.FechaModificacion
                })
                .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoPartidaRepository), nameof(GetOneDmgDoctoByCia));
            return Task.FromResult<DmgDoctosResultSet?>(null);
        }
    }

    public async Task<bool> SaveOrUpdateDmgDocto(DmgDoctosDto data)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.InsertarOActualizarDocumento";
            command.CommandType = CommandType.StoredProcedure;

            command = AddParamsForSaveOrUpdate(command, data);

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoPartidaRepository), nameof(SaveOrUpdateDmgDocto));
            return false;
        }
    }

    public Task<List<Select2ResultSet>> GetForSelect2(string codCia, string? query = null, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            IQueryable<TipoPartidaC> efQuery = dbContext.TipoPartida
                .Where(entity => entity.CodCia == codCia);

            if (!query.IsNullOrEmpty())
            {
                efQuery = dbContext.TipoPartida
                    .Where(
                        entity => 
                            EF.Functions.Like(entity.TipoPartida, $"%{query}%")
                            || EF.Functions.Like(entity.Nombre, $"%{query}%")
                    );
            }

            var count = efQuery.Count();

            return efQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(entity => new Select2ResultSet
                {
                    id = entity.IdTipoPartida.ToString(),
                    text = entity.TipoPartida + " - " + entity.Nombre,
                    more = pageNumber * pageSize < count
                })
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoPartidaRepository), nameof(GetForSelect2));
            return Task.FromResult(new List<Select2ResultSet>());
        }
    }

    private DbCommand AddParamsForSaveOrUpdate(DbCommand command, DmgDoctosDto data)
    {
        command.Parameters.Add(new SqlParameter("@IdTipoPartida", SqlDbType.VarChar) { Value = data.IdTipoPartida == null ? DBNull.Value : data.IdTipoPartida });
        command.Parameters.Add(new SqlParameter("@CodCia", SqlDbType.VarChar) { Value = data.CodCia==null ? DBNull.Value : data.CodCia });
        command.Parameters.Add(new SqlParameter("@TipoPartida", SqlDbType.VarChar) { Value = data.TipoPartida==null ? DBNull.Value : data.TipoPartida });
        command.Parameters.Add(new SqlParameter("@TipoHomologar", SqlDbType.VarChar) { Value = data.TipoHomologar == null ? DBNull.Value : data.TipoHomologar });
        command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar) { Value = data.Nombre==null ? DBNull.Value : data.Nombre });
        command.Parameters.Add(new SqlParameter("@UsuarioCreacion", SqlDbType.VarChar) { Value = data.UsuarioCreacion==null ? DBNull.Value : data.UsuarioCreacion });
        command.Parameters.Add(new SqlParameter("@FechaCreacion", SqlDbType.DateTime) { Value = data.FechaCreacion==null ? DBNull.Value : data.FechaCreacion });
        command.Parameters.Add(new SqlParameter("@UsuarioModificacion", SqlDbType.VarChar) { Value = data.UsuarioModificacion==null ? DBNull.Value : data.UsuarioModificacion });
        command.Parameters.Add(new SqlParameter("@FechaModificacion", SqlDbType.DateTime) { Value = data.FechaModificacion==null ? DBNull.Value : data.FechaModificacion });

        return command;
    }
}