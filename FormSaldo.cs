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
            this.Text = "Módulo Banco - Consulta de Saldo";
            this.ClientSize = new System.Drawing.Size(600, 460);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Header
            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(600, 80),
                BackColor = BankTheme.PrimaryBlue
            };

            Label lblTitulo = new Label
            {
                Text = "💰 CONSULTA DE SALDO",
                Location = new System.Drawing.Point(150, 25),
                Size = new System.Drawing.Size(300, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.Add(lblTitulo);

            // Card principal
            Panel mainCard = BankTheme.CreateCard(50, 110, 500, 250);

            Label lblCuentaLabel = new Label
            {
                Text = "Número de Cuenta",
                Location = new System.Drawing.Point(40, 30),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            lblCuenta = new Label
            {
                Location = new System.Drawing.Point(40, 55),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold),
                ForeColor = BankTheme.PrimaryBlue
            };

            Panel separador = new Panel
            {
                Location = new System.Drawing.Point(40, 100),
                Size = new System.Drawing.Size(420, 1),
                BackColor = BankTheme.LightGray
            };

            Label lblSaldoLabel = new Label
            {
                Text = "Saldo Disponible",
                Location = new System.Drawing.Point(40, 120),
                Size = new System.Drawing.Size(420, 25),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.TextSecondary,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            lblSaldo = new Label
            {
                Location = new System.Drawing.Point(40, 150),
                Size = new System.Drawing.Size(420, 50),
                Font = BankTheme.MoneyFont,
                ForeColor = BankTheme.Success,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Label lblActualizacion = new Label
            {
                Text = $"Actualizado: {DateTime.Now:dd/MM/yyyy HH:mm}",
                Location = new System.Drawing.Point(40, 210),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            mainCard.Controls.AddRange(new Control[] { lblCuentaLabel, lblCuenta, separador, lblSaldoLabel, lblSaldo, lblActualizacion });

            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new System.Drawing.Point(200, 390),
                Size = new System.Drawing.Size(200, 45)
            };
            BankTheme.StyleButton(btnCerrar, false);

            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, mainCard, btnCerrar });
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
