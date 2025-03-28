using System.Text.Json;
using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Controllers;

public class TipoMovCuentasController(
    ILogger<TipoMovCuentasController> logger,
    ITipoMovCuentasRepository tipoMovCuentasRepository,
    ISecurityRepository securityRepository
) : Controller {
    [IsAuthorized (alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_TIPO_MOV_CUENTAS)]
    public IActionResult Index ( ) {
        return View ( );
    }

    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_TIPO_MOV_CUENTAS)]
    [HttpGet]
    public async Task<JsonResult> GetAll([FromQuery] string CodCia) {
        List<TipoMovCuentas>? data;

        try {
            data = await tipoMovCuentasRepository.GetAll(CodCia, null);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoMovCuentasController), nameof(GetAll));
            data = null;
        }

        return Json(new {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_TIPO_MOV_CUENTAS_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_TIPO_MOV_CUENTAS_CAN_UPDATE}")]
    [HttpGet]
    public async Task<JsonResult> GetToSelect2CofasaIdTipoMov([FromQuery] string NombreMov, [FromQuery] string q, [FromQuery] int page, [FromQuery] int pageSize) {
        var catalogo = new List<Select2ResultSet>();

        try {
            catalogo = await tipoMovCuentasRepository.CallGetCofasaIdTipoMovForSelect2(NombreMov, q, page, pageSize);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCuentasController), nameof(GetToSelect2CofasaIdTipoMov));
        }

        return Json(new {
            results = catalogo,
            more = catalogo.Count > 0 ? catalogo.Last().more : false
        });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_TIPO_MOV_CUENTAS_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_TIPO_MOV_CUENTAS_CAN_UPDATE}")]
    [HttpGet]
    public async Task<JsonResult> GetToSelect2CofasaIdPais([FromQuery] string q) {
        var pais = new List<Select2ResultSet>();

        try {
            pais = await tipoMovCuentasRepository.CallGetCofasaIdPaisForSelect2(q);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCuentasController), nameof(GetToSelect2CofasaIdTipoMov));
        }

        return Json(new {
            results = pais,
            more = false
        });
    }


    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_TIPO_MOV_CUENTAS_CAN_ADD}," + 
                         $"{CC.THIRD_LEVEL_PERMISSION_TIPO_MOV_CUENTAS_CAN_UPDATE}")]
    [HttpPost]
    public async Task<JsonResult> SaveOrUpdate([FromForm] TipoMovCuentas data) {
        bool result;
        var isUpdating = data.Id != 0 ? true : false;

        try {
            result = await tipoMovCuentasRepository.SaveOrUpdate(data);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoMovCuentasController), nameof(SaveOrUpdate));
            result = false;
        }

        var message = isUpdating ? "Cuenta asociada a movimiento actualizado correctamente"
            : "Cuenta asociada a tipo de movimiento correctamente";

        var errorMessage = isUpdating ? "Ocurrió un error al actualizar el registro"
            : "Ocurrió un error al asociar el registro";

        return Json(new {
            success = result,
            message = result ? message : errorMessage
        });
    }

    [HttpGet]
    public async Task<JsonResult> GetOne([FromQuery] int id) {
        TipoMovCuentas? result;

        try {
            result = await tipoMovCuentasRepository.GetOne(id);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoMovCuentasController), nameof(GetOne));
            result = null;
        }

        return Json(new {
            success = true,
            message = "Access data",
            data = result
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }
}