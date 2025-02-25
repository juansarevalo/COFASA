using CoreContable.Entities;
using CoreContable.Entities.FuntionResult;
using CoreContable.Entities.Views;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.EntityFrameworkCore;

namespace CoreContable {
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext {
        public DbContext(DbContextOptions<DbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // Schema por defecto.
            modelBuilder.HasDefaultSchema(CC.SCHEMA);

            // Configuración de modelos sin llave primaria.
            modelBuilder.Entity<UserMenuPermissionFromFunctionResult>(e => e.HasNoKey());
            modelBuilder.Entity<ValidateUserOnLoginFromFunctionResult>(e => e.HasNoKey());
            modelBuilder.Entity<ConsultarCentroCuentaFromFunc>(e => e.HasNoKey());
            modelBuilder.Entity<GetCofasaCodCiasFromFunctionResult>(e => e.HasNoKey());
            modelBuilder.Entity<ConsultarCofasaCatalogoFromFunc>(e => e.HasNoKey());

            // Varias llaves primarias
            modelBuilder.Entity<CentroCuenta>().HasKey(x => new { x.COD_CIA, x.CENTRO_COSTO, x.CTA_1, x.CTA_2, x.CTA_3, x.CTA_4, x.CTA_5, x.CTA_6 });

            // Configuración de vistas existentes
            modelBuilder.Entity<CentroCuentaView>().ToView("vw_centro_cuenta").HasKey(x => new { x.COD_CIA, x.CENTRO_COSTO, x.CTA_1, x.CTA_2, x.CTA_3, x.CTA_4, x.CTA_5, x.CTA_6 });

            modelBuilder.Entity<CuentasContablesView>().ToView("V_CuentasContables").HasKey(x => new { x.COD_CIA, x.CuentasConcatenadas, x.CuentaContable });

            // Configuración de la nueva vista CentroCuentaFormatoView
            modelBuilder.Entity<CentroCuentaFormatoView>()
                .ToView("vw_centro_cuenta_formato", "CONTABLE")
                .HasKey(x => new { x.COD_CIA, x.CENTRO_COSTO, x.CTA_1, x.CTA_2, x.CTA_3, x.CTA_4, x.CTA_5, x.CTA_6 });
        }

        // DbSets de entidades
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<UserApp> UserApp { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Companias> Companias { get; set; }
        public DbSet<UserCia> UserCia { get; set; }
        public DbSet<DmgCuentas> DmgCuentas { get; set; }
        public DbSet<DmgDoctos> DmgDoctos { get; set; }
        public DbSet<CentroCosto> CentroCosto { get; set; }
        public DbSet<CentroCuenta> CentroCuenta { get; set; }
        public DbSet<TipoEntradaCuentas> TipoEntradaCuentas { get; set; }

        // DbSets de vistas
        public DbSet<CentroCuentaView> CentroCuentaView { get; set; }

        public DbSet<CuentasContablesView> CuentasContablesView { get; set; }

        // Nuevo DbSet para la vista CentroCuentaFormatoView
        public DbSet<CentroCuentaFormatoView> CentroCuentaFormatoView { get; set; }

        // DbSets de resultados de funciones
        public DbSet<UserMenuPermissionFromFunctionResult> UserMenuPermissionFromFunctionResult { get; set; }
        public DbSet<ValidateUserOnLoginFromFunctionResult> ValidateUserOnLoginFromFunctionResult { get; set; }
        public DbSet<ConsultarCentroCuentaFromFunc> ConsultarCentroCuentaFromFunc { get; set; }
        public DbSet<GetCofasaCodCiasFromFunctionResult> GetCofasaCodCiasFromFunctionResult { get; set; }

        public DbSet<ConsultarCofasaCatalogoFromFunc> ConsultarCofasaCatalogoFromFunc { get; set; }
    }
}
