using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormSaldo : Form
    {
        public FormSaldo()
        {
            InitializeComponent();
            CargarSaldo();
        }

        private Label lblSaldo;
        private Label lblCuenta;

        private void InitializeComponent()
        {
            this.Text = "Consultar Saldo";
            this.Size = new System.Drawing.Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTitulo = new Label { Text = "SALDO ACTUAL", Location = new System.Drawing.Point(150, 30), Size = new System.Drawing.Size(200, 30), Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold) };
            lblCuenta = new Label { Location = new System.Drawing.Point(100, 80), Size = new System.Drawing.Size(300, 25), Font = new System.Drawing.Font("Arial", 11) };
            lblSaldo = new Label { Location = new System.Drawing.Point(100, 120), Size = new System.Drawing.Size(300, 40), Font = new System.Drawing.Font("Arial", 18, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.Green };
            Button btnCerrar = new Button { Text = "Cerrar", Location = new System.Drawing.Point(200, 200), Size = new System.Drawing.Size(100, 35) };

            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitulo, lblCuenta, lblSaldo, btnCerrar });
        }

        private void CargarSaldo()
        {
            try
            {
                string query = "SELECT numero_cuenta, saldo FROM cuentas WHERE id_cuenta = @id";
                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));

                if (dt.Rows.Count > 0)
                {
                    lblCuenta.Text = "Cuenta: " + dt.Rows[0]["numero_cuenta"].ToString();
                    lblSaldo.Text = "$" + Convert.ToDecimal(dt.Rows[0]["saldo"]).ToString("N2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
