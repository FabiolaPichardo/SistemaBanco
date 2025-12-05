using System;
using System.Collections.Generic;

namespace SistemaBanco
{
    public static class RoleManager
    {

        private static readonly Dictionary<string, HashSet<string>> permisos = new Dictionary<string, HashSet<string>>
        {
            ["Cliente"] = new HashSet<string>
            {
                "ConsultarSaldo",
                "Transferencias",
                "Historial",
                "EstadoCuenta"
            },
            ["Cajero"] = new HashSet<string>
            {
                "ConsultarSaldo",
                "ConsultarSaldoActual", // Solo saldo actual, sin histórico
                "RegistrarMovimientos",
                "ExportarPDFBasico"
            },
            ["Ejecutivo"] = new HashSet<string>
            {
                "ConsultarSaldo",
                "ConsultarSaldoActual",
                "ConsultarSaldoHistorico", // Acceso a históricos
                "Transferencias",
                "Historial",
                "EstadoCuenta",
                "RegistrarMovimientos",
                "ConsultarClientes",
                "ExportarCompleto", // PDF, Word, Excel
                "FiltrosAvanzados",
                "AutorizarDivisas", // Autorización de operaciones en divisas
                "ConsultarSolicitudesDivisas"
            },
            ["Gerente"] = new HashSet<string>
            {
                "ConsultarSaldo",
                "ConsultarSaldoActual",
                "ConsultarSaldoHistorico",
                "Transferencias",
                "Historial",
                "EstadoCuenta",
                "RegistrarMovimientos",
                "ConsultarClientes",
                "AprobarTransferencias",
                "ReportesGerenciales",
                "ExportarCompleto",
                "FiltrosAvanzados",
                "AutorizarDivisas",
                "ConsultarSolicitudesDivisas",
                "ConfigurarRolesDivisas"
            },
            ["Administrador"] = new HashSet<string>
            {
                "ConsultarSaldo",
                "ConsultarSaldoActual",
                "ConsultarSaldoHistorico",
                "Transferencias",
                "Historial",
                "EstadoCuenta",
                "RegistrarMovimientos",
                "ConsultarClientes",
                "AprobarTransferencias",
                "ReportesGerenciales",
                "GestionUsuarios",
                "AdministrarUsuarios",
                "ConfiguracionSistema",
                "ExportarCompleto",
                "FiltrosAvanzados",
                "AutorizarDivisas",
                "ConsultarSolicitudesDivisas",
                "ConfigurarRolesDivisas"
            }
        };

        public static bool TienePermiso(string rol, string permiso)
        {
            if (string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(permiso))
                return false;

            if (!permisos.ContainsKey(rol))
                return false;

            return permisos[rol].Contains(permiso);
        }

        public static HashSet<string> ObtenerPermisos(string rol)
        {
            if (string.IsNullOrEmpty(rol) || !permisos.ContainsKey(rol))
                return new HashSet<string>();

            return new HashSet<string>(permisos[rol]);
        }

        public static string ObtenerDescripcionRol(string rol)
        {
            return rol switch
            {
                "Cliente" => "Acceso a operaciones básicas de cuenta",
                "Cajero" => "Consulta de saldos actuales y registro de movimientos",
                "Ejecutivo" => "Acceso completo a saldos históricos y reportes",
                "Gerente" => "Acceso completo a operaciones y reportes gerenciales",
                "Administrador" => "Acceso total al sistema",
                _ => "Rol desconocido"
            };
        }

        public static bool PuedeVerHistorico(string rol)
        {
            return TienePermiso(rol, "ConsultarSaldoHistorico");
        }

        public static bool PuedeExportarCompleto(string rol)
        {
            return TienePermiso(rol, "ExportarCompleto");
        }

        public static bool PuedeFiltrosAvanzados(string rol)
        {
            return TienePermiso(rol, "FiltrosAvanzados");
        }

        public static bool PuedeAutorizarDivisas(string rol)
        {
            return TienePermiso(rol, "AutorizarDivisas");
        }

        public static bool PuedeConsultarSolicitudesDivisas(string rol)
        {
            return TienePermiso(rol, "ConsultarSolicitudesDivisas");
        }

        public static bool PuedeConfigurarRolesDivisas(string rol)
        {
            return TienePermiso(rol, "ConfigurarRolesDivisas");
        }
    }
}
