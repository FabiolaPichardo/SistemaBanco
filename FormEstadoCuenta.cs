using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormEstadoCuenta : Form
    {
        private DataGridView dgvMovimientos;
        private Label lblSaldoInicial;
        private Label lblTotalIngresos;
        private Label lblTotalEgresos;
        private Label lblSaldoFinal;
        private DateTimePicker dtpInicio;
        private DateTimePicker dtpFin;

        public FormEstadoCuenta()
        {
            InitializeComponent();
            CargarEstadoCuenta();
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Estado de Cuenta";
            this.ClientSize = new System.Drawing.Size(1100, 760);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Header
            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(1100, 80),
                BackColor = BankTheme.PrimaryBlue
            };

            Label lblTitulo = new Label
            {
                Text = "📄 ESTADO DE CUENTA",
                Location = new System.Drawing.Point(400, 25),
                Size = new System.Drawing.Size(300, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Botón de inicio
            HomeButton.AddToForm(this, headerPanel);

            headerPanel.Controls.Add(lblTitulo);

            // Panel de filtros
            Panel filterPanel = BankTheme.CreateCard(30, 100, 1040, 80);

            Label lblFechaInicio = new Label
            {
                Text = "Fecha Inicio:",
                Location = new System.Drawing.Point(30, 25),
                Size = new System.Drawing.Size(100, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            dtpInicio = new DateTimePicker
            {
                Location = new System.Drawing.Point(140, 22),
                Size = new System.Drawing.Size(200, 25),
                Font = BankTheme.BodyFont,
                Value = DateTime.Now.AddMonths(-1)
            };

            Label lblFechaFin = new Label
            {
                Text = "Fecha Fin:",
                Location = new System.Drawing.Point(380, 25),
                Size = new System.Drawing.Size(100, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            dtpFin = new DateTimePicker
            {
                Location = new System.Drawing.Point(480, 22),
                Size = new System.Drawing.Size(200, 25),
                Font = BankTheme.BodyFont,
                Value = DateTime.Now
            };

            Button btnFiltrar = new Button
            {
                Text = "🔍 FILTRAR",
                Location = new System.Drawing.Point(720, 18),
                Size = new System.Drawing.Size(130, 35)
            };
            BankTheme.StyleButton(btnFiltrar, true);
            btnFiltrar.Click += (s, e) => CargarEstadoCuenta();

            filterPanel.Controls.AddRange(new Control[] { lblFechaInicio, dtpInicio, lblFechaFin, dtpFin, btnFiltrar });

            // Panel de resumen
            Panel summaryPanel = BankTheme.CreateCard(30, 200, 1040, 100);

            Label lblResumen = new Label
            {
                Text = "Resumen del Período",
                Location = new System.Drawing.Point(20, 15),
                Size = new System.Drawing.Size(200, 20),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            lblSaldoInicial = new Label
            {
                Location = new System.Drawing.Point(50, 50),
                Size = new System.Drawing.Size(200, 35),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextPrimary
            };

            lblTotalIngresos = new Label
            {
                Location = new System.Drawing.Point(300, 50),
                Size = new System.Drawing.Size(200, 35),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.Success
            };

            lblTotalEgresos = new Label
            {
                Location = new System.Drawing.Point(550, 50),
                Size = new System.Drawing.Size(200, 35),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.Danger
            };

            lblSaldoFinal = new Label
            {
                Location = new System.Drawing.Point(800, 50),
                Size = new System.Drawing.Size(200, 35),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            summaryPanel.Controls.AddRange(new Control[] { lblResumen, lblSaldoInicial, lblTotalIngresos, lblTotalEgresos, lblSaldoFinal });

            // Panel de movimientos
            Panel movPanel = BankTheme.CreateCard(30, 320, 1040, 330);

            dgvMovimientos = new DataGridView
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(1020, 310),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = BankTheme.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BankTheme.PrimaryBlue,
                    ForeColor = BankTheme.White,
                    Font = BankTheme.HeaderFont,
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = BankTheme.BodyFont,
                    SelectionBackColor = BankTheme.SecondaryBlue,
                    SelectionForeColor = BankTheme.White
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BankTheme.LightGray
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            movPanel.Controls.Add(dgvMovimientos);

            // Botones
            Button btnExportar = new Button
            {
                Text = "📥 EXPORTAR PDF",
                Location = new System.Drawing.Point(350, 690),
                Size = new System.Drawing.Size(180, 50)
            };
            BankTheme.StyleButton(btnExportar, true);
            btnExportar.Click += (s, e) => MessageBox.Show("Funcionalidad de exportación en desarrollo", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new System.Drawing.Point(550, 690),
                Size = new System.Drawing.Size(180, 50)
            };
            BankTheme.StyleButton(btnCerrar, false);
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, filterPanel, summaryPanel, movPanel, btnExportar, btnCerrar });
        }

        private void CargarEstadoCuenta()
        {
            try
            {
                DateTime fechaInicio = dtpInicio.Value.Date;
                DateTime fechaFin = dtpFin.Value.Date.AddDays(1).AddSeconds(-1);

                // Obtener movimientos del período
                string query = @"SELECT 
                                    fecha::date as Fecha,
                                    tipo as Tipo,
                                    monto as Monto,
                                    concepto as Concepto,
                                    saldo_nuevo as ""Saldo""
                                FROM movimientos 
                                WHERE id_cuenta = @id 
                                AND fecha BETWEEN @inicio AND @fin
                                ORDER BY fecha DESC";

                DataTable dt = Database.ExecuteQuery(query,
                    new NpgsqlParameter("@id", FormLogin.IdCuentaActual),
                    new NpgsqlParameter("@inicio", fechaInicio),
                    new NpgsqlParameter("@fin", fechaFin));

                dgvMovimientos.DataSource = dt;

                if (dgvMovimientos.Columns.Count > 0)
                {
                    dgvMovimientos.Columns["Monto"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.Columns["Saldo"].DefaultCellStyle.Format = "C2";
                }

                // Calcular resumen
                string queryResumen = @"SELECT 
                                        COALESCE(SUM(CASE WHEN tipo IN ('DEPOSITO', 'ABONO', 'TRANSFERENCIA RECIBIDA') THEN monto ELSE 0 END), 0) as ingresos,
                                        COALESCE(SUM(CASE WHEN tipo IN ('RETIRO', 'CARGO', 'TRANSFERENCIA ENVIADA') THEN monto ELSE 0 END), 0) as egresos
                                       FROM movimientos 
                                       WHERE id_cuenta = @id 
                                       AND fecha BETWEEN @inicio AND @fin";

                DataTable dtResumen = Database.ExecuteQuery(queryResumen,
                    new NpgsqlParameter("@id", FormLogin.IdCuentaActual),
                    new NpgsqlParameter("@inicio", fechaInicio),
                    new NpgsqlParameter("@fin", fechaFin));

                decimal ingresos = Convert.ToDecimal(dtResumen.Rows[0]["ingresos"]);
                decimal egresos = Convert.ToDecimal(dtResumen.Rows[0]["egresos"]);

                // Obtener saldo actual
                string querySaldo = "SELECT saldo FROM cuentas WHERE id_cuenta = @id";
                DataTable dtSaldo = Database.ExecuteQuery(querySaldo, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));
                decimal saldoActual = Convert.ToDecimal(dtSaldo.Rows[0]["saldo"]);

                decimal saldoInicial = saldoActual - ingresos + egresos;

                lblSaldoInicial.Text = $"Saldo Inicial:\n${saldoInicial:N2}";
                lblTotalIngresos.Text = $"Total Ingresos:\n+${ingresos:N2}";
                lblTotalEgresos.Text = $"Total Egresos:\n-${egresos:N2}";
                lblSaldoFinal.Text = $"Saldo Final:\n${saldoActual:N2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
