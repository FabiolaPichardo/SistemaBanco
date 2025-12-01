using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormHistorial : Form
    {
        private DataGridView dgvMovimientos;

        public FormHistorial()
        {
            InitializeComponent();
            CargarMovimientos();
        }

        private void InitializeComponent()
        {
            this.Text = "Historial de Movimientos";
            this.Size = new System.Drawing.Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTitulo = new Label { Text = "HISTORIAL DE MOVIMIENTOS", Location = new System.Drawing.Point(280, 20), Size = new System.Drawing.Size(340, 30), Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold) };
            
            dgvMovimientos = new DataGridView 
            { 
                Location = new System.Drawing.Point(20, 60), 
                Size = new System.Drawing.Size(850, 350),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            Button btnCerrar = new Button { Text = "Cerrar", Location = new System.Drawing.Point(400, 420), Size = new System.Drawing.Size(100, 35) };
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitulo, dgvMovimientos, btnCerrar });
        }

        private void CargarMovimientos()
        {
            try
            {
                string query = @"SELECT 
                                    fecha::date as Fecha,
                                    tipo as Tipo,
                                    monto as Monto,
                                    concepto as Concepto,
                                    saldo_anterior as ""Saldo Anterior"",
                                    saldo_nuevo as ""Saldo Nuevo""
                                FROM movimientos 
                                WHERE id_cuenta = @id 
                                ORDER BY fecha DESC";
                
                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));
                dgvMovimientos.DataSource = dt;

                // Formato de columnas
                if (dgvMovimientos.Columns.Count > 0)
                {
                    dgvMovimientos.Columns["Monto"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.Columns["Saldo Anterior"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.Columns["Saldo Nuevo"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
