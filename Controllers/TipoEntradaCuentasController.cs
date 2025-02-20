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

public class TipoEntradaCuentasController(
    ILogger<TipoEntradaCuentasController> logger,
    ITipoEntradaCuentasRepository tipoEntradaCuentasRepository,
    ISecurityRepository securityRepository
) : Controller {
    public IActionResult Index ( ) {
        return View ( );
    }

    [HttpGet]
    public async Task<JsonResult> GetAll([FromQuery] string CodCia) {
        List<TipoEntradaCuentas>? data;

        try {
            data = await tipoEntradaCuentasRepository.GetAll(CodCia, null);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoEntradaCuentasController), nameof(GetAll));
            data = null;
        }

        return Json(new {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [HttpPost]
    public async Task<JsonResult> SaveOrUpdate([FromForm] TipoEntradaCuentas data) {
        bool result;
        var isUpdating = data.Id != 0 ? true : false;

        try {
            result = await tipoEntradaCuentasRepository.SaveOrUpdate(data);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoEntradaCuentasController), nameof(SaveOrUpdate));
            result = false;
        }

        var message = isUpdating ? "Cuenta asociada a tipo de entrada actualizado correctamente"
            : "Cuenta asociada a tipo de entrada correctamente";

        var errorMessage = isUpdating ? "Ocurrió un error al actualizar el registro"
            : "Ocurrió un error al asociar el registro";

        return Json(new {
            success = result,
            message = result ? message : errorMessage
        });
    }

    [HttpGet]
    public async Task<JsonResult> GetOne([FromQuery] int id) {
        TipoEntradaCuentas? result;

        try {
            result = await tipoEntradaCuentasRepository.GetOne(id);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(TipoEntradaCuentasController), nameof(GetOne));
            result = null;
        }

        return Json(new {
            success = true,
            message = "Access data",
            data = result
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }
}