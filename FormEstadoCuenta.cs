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
            IconHelper.SetFormIcon(this);
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

            HomeButton.AddToForm(this, headerPanel);

            headerPanel.Controls.Add(lblTitulo);

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

            Button btnLimpiarFiltros = new Button
            {
                Text = "🔄 LIMPIAR",
                Location = new System.Drawing.Point(870, 18),
                Size = new System.Drawing.Size(130, 35)
            };
            BankTheme.StyleButton(btnLimpiarFiltros, false);
            btnLimpiarFiltros.Click += (s, e) =>
            {
                dtpInicio.Value = DateTime.Now.AddMonths(-1);
                dtpFin.Value = DateTime.Now;
                CargarEstadoCuenta();
            };

            filterPanel.Controls.AddRange(new Control[] { lblFechaInicio, dtpInicio, lblFechaFin, dtpFin, btnFiltrar, btnLimpiarFiltros });

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
                Size = new System.Drawing.Size(220, 35),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            summaryPanel.Controls.AddRange(new Control[] { lblResumen, lblSaldoInicial, lblTotalIngresos, lblTotalEgresos, lblSaldoFinal });

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

            Panel panelBotones = new Panel
            {
                Location = new System.Drawing.Point(30, 660),
                Size = new System.Drawing.Size(1040, 50),
                BackColor = System.Drawing.Color.Transparent
            };

            Button btnExportarPDF = new Button
            {
                Text = "📄 PDF",
                Location = new System.Drawing.Point(300, 5),
                Size = new System.Drawing.Size(140, 40)
            };
            BankTheme.StyleButton(btnExportarPDF, false);
            btnExportarPDF.Click += (s, e) => ExportarEstadoCuenta("PDF");

            Button btnExportarWord = new Button
            {
                Text = "📝 Word",
                Location = new System.Drawing.Point(450, 5),
                Size = new System.Drawing.Size(140, 40)
            };
            BankTheme.StyleButton(btnExportarWord, false);
            btnExportarWord.Click += (s, e) => ExportarEstadoCuenta("Word");

            Button btnExportarExcel = new Button
            {
                Text = "📊 Excel",
                Location = new System.Drawing.Point(600, 5),
                Size = new System.Drawing.Size(140, 40)
            };
            BankTheme.StyleButton(btnExportarExcel, false);
            btnExportarExcel.Click += (s, e) => ExportarEstadoCuenta("Excel");

            panelBotones.Controls.AddRange(new Control[] { btnExportarPDF, btnExportarWord, btnExportarExcel });

            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new System.Drawing.Point(450, 720),
                Size = new System.Drawing.Size(200, 40)
            };
            BankTheme.StyleButton(btnCerrar, false);
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, filterPanel, summaryPanel, movPanel, panelBotones, btnCerrar });
        }

        private void ExportarEstadoCuenta(string formato)
        {
            try
            {
                DateTime fechaInicio = dtpInicio.Value.Date;
                DateTime fechaFin = dtpFin.Value.Date.AddDays(1).AddSeconds(-1);

                string query = @"SELECT 
                                    fecha::date as Fecha,
                                    tipo as Tipo,
                                    monto as Monto,
                                    concepto as Concepto,
                                    saldo_nuevo as Saldo
                                FROM movimientos 
                                WHERE id_cuenta = @id 
                                AND fecha BETWEEN @inicio AND @fin
                                ORDER BY fecha DESC";

                DataTable dt = Database.ExecuteQuery(query,
                    new NpgsqlParameter("@id", FormLogin.IdCuentaActual),
                    new NpgsqlParameter("@inicio", fechaInicio),
                    new NpgsqlParameter("@fin", fechaFin));

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar en el período seleccionado.", "Información", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                switch (formato)
                {
                    case "PDF":
                        ExportHelper.ExportarPDF(dt, "Estado de Cuenta", "Estado_Cuenta");
                        break;
                    case "Word":
                        ExportHelper.ExportarWord(dt, "Estado de Cuenta", "Estado_Cuenta");
                        break;
                    case "Excel":
                        ExportHelper.ExportarExcel(dt, "Estado de Cuenta", "Estado_Cuenta");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerarHTMLEstadoCuenta(DataTable dt)
        {
            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Estado de Cuenta</title>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 40px; }}
        h1 {{ color: #1e40af; text-align: center; }}
        .info {{ background: #f3f4f6; padding: 15px; margin: 20px 0; border-radius: 8px; }}
        .resumen {{ background: #dbeafe; padding: 15px; margin: 20px 0; border-radius: 8px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        th {{ background: #1e40af; color: white; padding: 12px; text-align: left; }}
        td {{ padding: 10px; border-bottom: 1px solid #e5e7eb; }}
        tr:nth-child(even) {{ background: #f9fafb; }}
        .footer {{ text-align: center; margin-top: 30px; color: #6b7280; font-size: 12px; }}
    </style>
</head>
<body>
    <h1>🏦 Estado de Cuenta</h1>
    <div class='info'>
        <strong>Usuario:</strong> {FormLogin.NombreUsuario}<br>
        <strong>Período:</strong> {dtpInicio.Value:dd/MM/yyyy} - {dtpFin.Value:dd/MM/yyyy}<br>
        <strong>Fecha de generación:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}
    </div>
    <div class='resumen'>
        <h3>Resumen del Período</h3>
        {lblSaldoInicial.Text}<br>
        {lblTotalIngresos.Text}<br>
        {lblTotalEgresos.Text}<br>
        <strong>{lblSaldoFinal.Text}</strong>
    </div>
    <h3>Detalle de Movimientos</h3>
    <table>
        <tr>
            <th>Fecha</th>
            <th>Tipo</th>
            <th>Monto</th>
            <th>Concepto</th>
            <th>Saldo</th>
        </tr>";

            foreach (DataRow row in dt.Rows)
            {
                html += $@"
        <tr>
            <td>{Convert.ToDateTime(row["Fecha"]):dd/MM/yyyy}</td>
            <td>{row["Tipo"]}</td>
            <td>${Convert.ToDecimal(row["Monto"]):N2}</td>
            <td>{row["Concepto"]}</td>
            <td>${Convert.ToDecimal(row["Saldo"]):N2}</td>
        </tr>";
            }

            html += @"
    </table>
    <div class='footer'>
        © 2025 Módulo Banco - Documento Confidencial
    </div>
</body>
</html>";

            return html;
        }

        private string GenerarWordEstadoCuenta(DataTable dt)
        {
            string doc = $@"
MÓDULO BANCO - ESTADO DE CUENTA
================================

Usuario: {FormLogin.NombreUsuario}
Período: {dtpInicio.Value:dd/MM/yyyy} - {dtpFin.Value:dd/MM/yyyy}
Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}

RESUMEN DEL PERÍODO
-------------------
{lblSaldoInicial.Text}
{lblTotalIngresos.Text}
{lblTotalEgresos.Text}
{lblSaldoFinal.Text}

DETALLE DE MOVIMIENTOS
----------------------

";

            foreach (DataRow row in dt.Rows)
            {
                doc += $@"
Fecha: {Convert.ToDateTime(row["Fecha"]):dd/MM/yyyy}
Tipo: {row["Tipo"]}
Monto: ${Convert.ToDecimal(row["Monto"]):N2}
Concepto: {row["Concepto"]}
Saldo: ${Convert.ToDecimal(row["Saldo"]):N2}
----------------------------------------
";
            }

            doc += @"

© 2025 Módulo Banco - Documento Confidencial
";

            return doc;
        }

        private string GenerarCSVEstadoCuenta(DataTable dt)
        {
            string csv = "# ESTADO DE CUENTA\n";
            csv += $"# Usuario: {FormLogin.NombreUsuario}\n";
            csv += $"# Período: {dtpInicio.Value:dd/MM/yyyy} - {dtpFin.Value:dd/MM/yyyy}\n";
            csv += $"# Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n";
            csv += $"# {lblSaldoInicial.Text.Replace("\n", " ")}\n";
            csv += $"# {lblTotalIngresos.Text.Replace("\n", " ")}\n";
            csv += $"# {lblTotalEgresos.Text.Replace("\n", " ")}\n";
            csv += $"# {lblSaldoFinal.Text.Replace("\n", " ")}\n\n";
            csv += "Fecha,Tipo,Monto,Concepto,Saldo\n";

            foreach (DataRow row in dt.Rows)
            {
                csv += $"{Convert.ToDateTime(row["Fecha"]):dd/MM/yyyy},";
                csv += $"\"{row["Tipo"]}\",";
                csv += $"{Convert.ToDecimal(row["Monto"]):N2},";
                csv += $"\"{row["Concepto"]}\",";
                csv += $"{Convert.ToDecimal(row["Saldo"]):N2}\n";
            }

            return csv;
        }

        private void CargarEstadoCuenta()
        {
            try
            {
                DateTime fechaInicio = dtpInicio.Value.Date;
                DateTime fechaFin = dtpFin.Value.Date.AddDays(1).AddSeconds(-1);

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
