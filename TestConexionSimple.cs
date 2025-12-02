using System;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public class TestConexionSimple
    {
        public static void Probar()
        {
            string connectionString = "Host=db.ovfaxfhvcjrvujtgiaaf.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=ModuloBanco2025;SSL Mode=Require;Trust Server Certificate=true;Timeout=30;";

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    
                    using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM usuarios", conn))
                    {
                        var count = cmd.ExecuteScalar();
                        MessageBox.Show(
                            $"✓ Conexión exitosa!\n\n" +
                            $"Base de datos: {conn.Database}\n" +
                            $"Servidor: {conn.Host}\n" +
                            $"Total de usuarios: {count}",
                            "Prueba de Conexión",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"✗ Error de conexión:\n\n" +
                    $"Tipo: {ex.GetType().Name}\n" +
                    $"Mensaje: {ex.Message}\n\n" +
                    $"Detalle: {ex.InnerException?.Message ?? "Sin detalles"}",
                    "Error de Conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
