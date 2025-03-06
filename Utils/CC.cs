using CoreContable.Services;

namespace CoreContable.Utils;

public abstract class CC
{
    // GENERA VARIABLES.
    public const string ADA = "ADA";
    public const string JOJO = "JOJO";
    public const string DIO = "DIO";

    // NOMBRES DE RUTAS
    public const string SecurityControllerName = "Security";
    public const string LoginActionName = "Login";
    public const string LogOutActionName = "LogOut";
    public const string HomeControllerName = "Home";
    public const string DashboardActionName = "Index";
    public const string ErrorActionName = "Error";

    // SESSION VARIABLES KEYS
    public const string CiaCodeKey = "CiaCode";
    public const string CiaNameKey = "CiaName";
    public const string UserNameKey = "UserName";
    public const string UserIdKey = "UserId";
    public const string FullNameKey = "FullName";
    public const string UserPermissionsKey = "UserPermissions";

    // NOMBRES DE TABLAS Y SCHEMA.

    public const string SCHEMA = "CONTABLE";
    // public const string SCHEMA = "dbo";

    public const string CENTRO_COSTO = "centro_costo";
    public const string CENTRO_CUENTA = "centro_cuenta";
    public const string COMPANIAS = "companias";
    public const string DMGCUENTAS = "dmgcuentas";
    public const string TIPOPARTIDA = "TipoPartida";
    public const string TIPOMOVCUENTAS = "TipoMovCuentas";
    public const string PERMISSION = "Permission";
    public const string ROLE = "Role";
    public const string ROLEPERMISSION = "RolePermission";
    public const string USERAPP = "UserApp";
    public const string USERCIA = "UserCia";
    public const string USERROLE = "UserRole";

    public const string RoleAdmin = "admin";
    public const string RoleReports = "reports";

    // ALIAS DE PERMISOS PARA ROLES.

    // FIRST LEVEL PERMISSIONS
    public const string FIST_LEVEL_PERMISSION_DASHBOARD = "DASHBOARD";
    public const string FIST_LEVEL_PERMISSION_SECURITY_MODULE = "SECURITY_MODULE";
    public const string FIST_LEVEL_PERMISSION_CONTABILITY_MODULE = "CONTABILITY_MODULE";
    public const string FIST_LEVEL_PERMISSION_REPORTS = "REPORTS";

    // SECOND LEVEL PERMISSIONS

    // SECURITY_MODULE
    public const string SECOND_LEVEL_PERMISSION_ADMIN_USERS = "ADMIN_USERS";
    public const string THIRD_LEVEL_PERMISSION_USERS_CAN_ADD = "USERS_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_USERS_CAN_UPDATE = "USERS_CAN_UPDATE";
    public const string THIRD_LEVEL_PERMISSION_USERS_CAN_DELETE = "USERS_CAN_DELETE";
    public const string THIRD_LEVEL_PERMISSION_USERS_CAN_COMPANY = "USERS_CAN_COMPANY";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_ROLES = "ADMIN_ROLES";
    public const string THIRD_LEVEL_PERMISSION_ROLES_CAN_ADD = "ROLES_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_ROLES_CAN_UPDATE = "ROLES_CAN_UPDATE";
    public const string THIRD_LEVEL_PERMISSION_ROLES_CAN_DELETE = "ROLES_CAN_DELETE";
    public const string THIRD_LEVEL_PERMISSION_ROLES_PER_COMPANY = "ROLES_PER_COMPANY";

    // CONTABILITY_MODULE
    public const string SECOND_LEVEL_PERMISSION_ADMIN_CIAS = "ADMIN_CIAS";
    public const string THIRD_LEVEL_PERMISSION_CIAS_CAN_ADD = "CIAS_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_CIAS_CAN_UPDATE = "CIAS_CAN_UPDATE";
    public const string THIRD_LEVEL_PERMISSION_CIAS_CAN_DELETE = "CIAS_CAN_DELETE";
    public const string THIRD_LEVEL_PERMISSION_CIAS_CAN_COPY = "CIAS_CAN_COPY";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_DMGDOCTOS = "TIPOPARTIDA";
    public const string THIRD_LEVEL_PERMISSION_DMGDOCTOS_CAN_ADD = "DMGDOCTOS_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_DMGDOCTOS_CAN_UPDATE = "DMGDOCTOS_CAN_UPDATE";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_DMGCUENTAS = "DMGCUENTAS";
    public const string THIRD_LEVEL_PERMISSION_DMGCUENTAS_CAN_ADD = "DMGCUENTAS_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_DMGCUENTAS_CAN_UPDATE = "DMGCUENTAS_CAN_UPDATE";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_CENTROCOSTO = "CENTROCOSTO";
    public const string THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_ADD = "CENTROCOSTO_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_UPDATE = "CENTROCOSTO_CAN_UPDATE";
    // public const string THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_ADMIN_ACCOUNTS = "CENTROCOSTO_CAN_ADMIN_ACCOUNTS";
    public const string THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_COPY = "CENTROCOSTO_CAN_COPY";


    // CENTRO_CUENTA MODULE
    public const string THIRD_LEVEL_PERMISSION_ADMIN_CENTRO_CUENTA = "CENTRO_CUENTA";
    public const string THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_ADD = "CENTRO_CUENTA_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_UPDATE = "CENTRO_CUENTA_CAN_UPDATE";
    public const string THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_EDIT = "CENTRO_CUENTA_CAN_EDIT";
    public const string THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_DELETE = "CENTRO_CUENTA_CAN_DELETE";


    public const string SECOND_LEVEL_PERMISSION_ADMIN_TIPO_MOV_CUENTAS = "ADMIN_TIPO_MOV_CUENTAS";
    public const string THIRD_LEVEL_PERMISSION_TIPO_MOV_CUENTAS_CAN_ADD = "TIPO_MOV_CUENTAS_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_TIPO_MOV_CUENTAS_CAN_UPDATE = "TIPO_MOV_CUENTAS_CAN_UPDATE";


    public static string GetDefaultReportSwitches(string user)
    {
        return $"--footer-left \"Emitido: el {DateTime.Now:MM/dd/yyyy} a las {DateTime.Now:hh:mm tt} por {user}\" --footer-right \"[page]\" --footer-line --footer-font-size 8 --footer-spacing 13 --footer-font-name \"calibri\"";
    }

    public static string DEFAULT_REPORT_SWITCHES = "--footer-right \"[page]\" --footer-line --footer-font-size 8 --footer-spacing 13 --footer-font-name \"calibri\"";


    public const string REPORT_NAME_COMP_DIARIO = "ComprobanteDeDiarioCD";
    public const string REPORT_NAME_HISTORICO_CUENTA = "HistoricoDeCuenta";
    public const string REPORT_NAME_BALANCE_COMP = "BalanceComprobacion";

    public const string ACCOUNT_TYPE_ACTIVO = "ACTIVO";
    public const string ACCOUNT_TYPE_PASIVO = "PASIVO";
    public const string SALDO_TIPO_ACREEDOR = "A";
    public const string SALDO_TIPO_DEUDOR = "D";
}