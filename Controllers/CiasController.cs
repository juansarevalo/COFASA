using CoreContable.Models;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Controllers {
    public class CiasController(
        ICiasRepository ciasRepository,
        ISecurityRepository securityRepository,
        IDmgCuentasRepository dmgCuentasRepository,
        ILogger<CiasController> logger
    ) : Controller {
        [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_CIAS)]
        public IActionResult Index() {
            return View();
        }

        [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_CIAS)]
        [HttpGet]
        public async Task<JsonResult> GetCias() {
            List<CiaResultSet>? data;

            try {
                data = await ciasRepository.CallGetCias("1");
            }
            catch (Exception e) {
                data = null;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(GetCias));
            }

            return Json(new {
                success = true,
                message = "Access data",
                data
            });
        }

        [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_ADD)]
        [HttpPost]
        public async Task<JsonResult> DoSaveCia([FromForm] CiaDto data) {
            bool result;

            try {
                result = await ciasRepository.CallSaveCia(cia: data);
            }
            catch (Exception e) {
                result = false;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(DoSaveCia));
            }

            return Json(new {
                success = result,
                message = result ? "Compañía guardada" : "Ocurrió un error al guardar el registro",
            });
        }

        [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_UPDATE}," +
                             $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_COPY}")]
        [HttpPost]
        public async Task<JsonResult> UpdateCia([FromForm] CiaDto data) {
            bool result;

            try {
                result = await ciasRepository.CallUpdateCia(data);
            }
            catch (Exception e) {
                result = false;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(UpdateCia));
            }

            return Json(new {
                success = result,
                message = result ? "Compañía actualizada" : "Ocurrió un error al actualizar el registro",
            });
        }

        [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_UPDATE}," +
                             $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_COPY}")]
        [HttpGet]
        public async Task<JsonResult> GetCiaById([FromQuery] string ciaCod) {
            bool result;
            CiaResultSet? cia = null;

            try {
                cia = await ciasRepository.GetOneCia(ciaCod);
                result = true;
            }
            catch (Exception e) {
                result = false;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(GetCiaById));
            }

            return Json(new {
                success = result,
                message = result ? "Access data" : "Ocurrió un error al obtener la empresa",
                data = cia
            });
        }

        [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_ADD)]
        [HttpGet]
        public async Task<JsonResult> GetCofasaCiaDataByCod([FromQuery] string ciaCod) {
            bool result;
            CiaResultSet? cia = null;

            try {
                cia = await ciasRepository.GetCofasaCiaData(ciaCod);
                result = true;
            }
            catch (Exception e) {
                result = false;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(GetCiaById));
            }

            return Json(new {
                success = result,
                message = result ? "Access data" : "Ocurrió un error al obtener los datos de la empresa",
                data = cia
            });
        }

        [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_ADD}," +
                                $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_UPDATE}," +
                                $"{CC.THIRD_LEVEL_PERMISSION_DMGCUENTAS_CAN_ADD}," +
                                $"{CC.THIRD_LEVEL_PERMISSION_DMGCUENTAS_CAN_UPDATE}," +
                                $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_COPY}")] 

        [HttpPost]
        public async Task<JsonResult> GetAccountName([FromForm] GetAccountNameDto data) {
            bool result;
            var accountName = "";

            try {
                var actualCiaCod = securityRepository.GetSessionCiaCode();
                data.CodCia = actualCiaCod;
                accountName = await dmgCuentasRepository.GetName(data);
                result = !accountName.IsNullOrEmpty();
            }
            catch (Exception e) {
                result = false;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(GetAccountName));
            }

            return Json(new {
                success = result,
                message = result ? "Access data" : "Ocurrió un error al obtener el registro",
                data = accountName
            });
        }

        [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_ADD}," +
                             $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_UPDATE}" +
                             $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_COMPANY}")]
        [HttpGet]
        public async Task<JsonResult> GetToSelect2([FromQuery] string q) {
            var cias = new List<Select2ResultSet>();

            try {
                cias = await ciasRepository.CallGetCiasForSelect2(q);
            }
            catch (Exception e) {
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(GetToSelect2));
            }

            return Json(new {
                results = cias
            });
        }

        [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_ADD}," +
                             $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_UPDATE}" +
                             $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_COPY}")]
        [HttpGet]
        public async Task<JsonResult> GetToSelect2CiasCofasa([FromQuery] string q, [FromQuery] int page, [FromQuery] int pageSize) {
            var cias = new List<Select2ResultSet>();
            var currentCia = securityRepository.GetSessionCiaCode();

            try {
                cias = await ciasRepository.CallGetCofasaCiasForSelect2(currentCia, q, page, pageSize);
            }
            catch (Exception e) {
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(ciasRepository), nameof(GetToSelect2));
            }

            return Json(new {
                results = cias,
                more = cias.Count > 0 ? cias.Last().more : false
            });
        }
    }
}