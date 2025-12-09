using System;
using System.Collections.Generic;

namespace SistemaBanco
{
    /// <summary>
    /// Clase estática que gestiona el sistema de roles y permisos del sistema bancario.
    /// Define 5 roles: Cliente, Cajero, Ejecutivo, Gerente y Administrador.
    /// Cada rol tiene permisos específicos para acceder a diferentes funcionalidades.
    /// </summary>
    public static class RoleManager
    {
        /// <summary>
        /// Diccionario que almacena los permisos de cada rol.
        /// Utiliza HashSet para búsquedas rápidas de permisos.
        /// </summary>
        private static readonly Dictionary<string, HashSet<string>> permisos = new Dictionary<string, HashSet<string>>
        {
            // Cliente: Usuario final con operaciones básicas de cuenta
            ["Cliente"] = new HashSet<string>
            {
                "ConsultarSaldo",
                "Transferencias",
                "Historial",
                "EstadoCuenta"
            },
            
            // Cajero: Personal de ventanilla con permisos limitados
            ["Cajero"] = new HashSet<string>
            {
                "ConsultarSaldo",
                "ConsultarSaldoActual",
                "RegistrarMovimientos",
                "ExportarPDFBasico"
            },
            
            // Ejecutivo: Personal especializado con acceso amplio
            ["Ejecutivo"] = new HashSet<string>
            {
                "ConsultarSaldo",
                "ConsultarSaldoActual",
                "ConsultarSaldoHistorico",
                "Transferencias",
                "Historial",
                "EstadoCuenta",
                "RegistrarMovimientos",
                "ConsultarClientes",
                "ExportarCompleto",
                "FiltrosAvanzados",
                "AutorizarDivisas",
                "ConsultarSolicitudesDivisas"
            },
            
            // Gerente: Nivel gerencial con supervisión completa
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
            
            // Administrador: Acceso total al sistema
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

        /// <summary>
        /// Verifica si un rol específico tiene un permiso determinado.
        /// </summary>
        /// <param name="rol">Nombre del rol a verificar</param>
        /// <param name="permiso">Nombre del permiso a verificar</param>
        /// <returns>True si el rol tiene el permiso, False en caso contrario</returns>
        public static bool TienePermiso(string rol, string permiso)
        {
            // Validar que los parámetros no estén vacíos
            if (string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(permiso))
                return false;

            // Verificar que el rol exista en el diccionario
            if (!permisos.ContainsKey(rol))
                return false;

            // Verificar si el rol tiene el permiso solicitado
            return permisos[rol].Contains(permiso);
        }

        /// <summary>
        /// Obtiene todos los permisos asociados a un rol específico.
        /// </summary>
        /// <param name="rol">Nombre del rol</param>
        /// <returns>HashSet con todos los permisos del rol, o vacío si el rol no existe</returns>
        public static HashSet<string> ObtenerPermisos(string rol)
        {
            // Validar que el rol exista
            if (string.IsNullOrEmpty(rol) || !permisos.ContainsKey(rol))
                return new HashSet<string>();

            // Devolver una copia de los permisos del rol
            return new HashSet<string>(permisos[rol]);
        }

        /// <summary>
        /// Obtiene una descripción legible del rol.
        /// </summary>
        /// <param name="rol">Nombre del rol</param>
        /// <returns>Descripción del rol</returns>
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

        /// <summary>
        /// Verifica si el rol puede ver saldos históricos.
        /// </summary>
        public static bool PuedeVerHistorico(string rol)
        {
            return TienePermiso(rol, "ConsultarSaldoHistorico");
        }

        /// <summary>
        /// Verifica si el rol puede exportar en todos los formatos (PDF, Word, Excel).
        /// </summary>
        public static bool PuedeExportarCompleto(string rol)
        {
            return TienePermiso(rol, "ExportarCompleto");
        }

        /// <summary>
        /// Verifica si el rol puede usar filtros avanzados en reportes.
        /// </summary>
        public static bool PuedeFiltrosAvanzados(string rol)
        {
            return TienePermiso(rol, "FiltrosAvanzados");
        }

        /// <summary>
        /// Verifica si el rol puede autorizar operaciones en divisas extranjeras.
        /// </summary>
        public static bool PuedeAutorizarDivisas(string rol)
        {
            return TienePermiso(rol, "AutorizarDivisas");
        }

        /// <summary>
        /// Verifica si el rol puede consultar solicitudes de divisas.
        /// </summary>
        public static bool PuedeConsultarSolicitudesDivisas(string rol)
        {
            return TienePermiso(rol, "ConsultarSolicitudesDivisas");
        }

        /// <summary>
        /// Verifica si el rol puede configurar roles autorizadores de divisas.
        /// </summary>
        public static bool PuedeConfigurarRolesDivisas(string rol)
        {
            return TienePermiso(rol, "ConfigurarRolesDivisas");
        }
    }
}
