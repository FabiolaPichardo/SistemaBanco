using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormHistorial : Form
    {
        private DataGridView dgvMovimientos;
        private TextBox txtBusqueda;
        private ComboBox cmbFiltroEstado;
        private DataTable dtMovimientos;

        public FormHistorial()
        {
            InitializeComponent();
            IconHelper.SetFormIcon(this);
            CargarMovimientos();
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Historial de Movimientos";
            this.ClientSize = new System.Drawing.Size(1100, 660);
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
                Text = "📊 HISTORIAL DE MOVIMIENTOS",
                Location = new System.Drawing.Point(350, 25),
                Size = new System.Drawing.Size(400, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Botón de inicio
            HomeButton.AddToForm(this, headerPanel);

            headerPanel.Controls.Add(lblTitulo);

            // Panel de búsqueda y filtros
            Panel panelFiltros = BankTheme.CreateCard(30, 110, 1040, 70);

            Label lblBusqueda = new Label
            {
                Text = "🔍 Buscar por descripción:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(180, 25),
                Font = BankTheme.BodyFont
            };

            txtBusqueda = new TextBox
            {
                Location = new System.Drawing.Point(200, 18),
                Size = new System.Drawing.Size(300, 25),
                Font = BankTheme.BodyFont,
                PlaceholderText = "Escriba para buscar..."
            };
            txtBusqueda.TextChanged += TxtBusqueda_TextChanged;

            Label lblEstado = new Label
            {
                Text = "Estado:",
                Location = new System.Drawing.Point(530, 20),
                Size = new System.Drawing.Size(60, 25),
                Font = BankTheme.BodyFont
            };

            cmbFiltroEstado = new ComboBox
            {
                Location = new System.Drawing.Point(590, 18),
                Size = new System.Drawing.Size(150, 25),
                Font = BankTheme.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFiltroEstado.Items.AddRange(new string[] { "Todos", "DEPOSITO", "RETIRO", "TRANSFERENCIA ENVIADA", "TRANSFERENCIA RECIBIDA" });
            cmbFiltroEstado.SelectedIndex = 0;
            cmbFiltroEstado.SelectedIndexChanged += (s, e) => AplicarFiltros();

            panelFiltros.Controls.AddRange(new Control[] { lblBusqueda, txtBusqueda, lblEstado, cmbFiltroEstado });

            // Card con DataGridView
            Panel mainCard = BankTheme.CreateCard(30, 200, 1040, 350);

            dgvMovimientos = new DataGridView
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(1020, 330),
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

            mainCard.Controls.Add(dgvMovimientos);

            // Botones de exportación
            Panel panelBotones = new Panel
            {
                Location = new System.Drawing.Point(30, 560),
                Size = new System.Drawing.Size(1040, 50),
                BackColor = System.Drawing.Color.Transparent
            };

            Button btnExportarPDF = new Button
            {
                Text = "📄 PDF",
                Location = new System.Drawing.Point(300, 5),
                Size = new System.Drawing.Size(120, 40)
            };
            BankTheme.StyleButton(btnExportarPDF, false);
            btnExportarPDF.Click += (s, e) => ExportarDatos("PDF");

            Button btnExportarWord = new Button
            {
                Text = "📝 Word",
                Location = new System.Drawing.Point(430, 5),
                Size = new System.Drawing.Size(120, 40)
            };
            BankTheme.StyleButton(btnExportarWord, false);
            btnExportarWord.Click += (s, e) => ExportarDatos("Word");

            Button btnExportarExcel = new Button
            {
                Text = "📊 Excel",
                Location = new System.Drawing.Point(560, 5),
                Size = new System.Drawing.Size(120, 40)
            };
            BankTheme.StyleButton(btnExportarExcel, false);
            btnExportarExcel.Click += (s, e) => ExportarDatos("Excel");

            panelBotones.Controls.AddRange(new Control[] { btnExportarPDF, btnExportarWord, btnExportarExcel });

            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new System.Drawing.Point(450, 620),
                Size = new System.Drawing.Size(200, 40)
            };
            BankTheme.StyleButton(btnCerrar, false);
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, panelFiltros, mainCard, panelBotones, btnCerrar });
        }

        private void TxtBusqueda_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            if (dtMovimientos == null) return;

            string filtro = "";
            
            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(txtBusqueda.Text))
            {
                string busqueda = txtBusqueda.Text.Trim().ToLower();
                filtro = $"Concepto LIKE '%{busqueda}%' OR Tipo LIKE '%{busqueda}%'";
            }

            // Filtro por estado
            if (cmbFiltroEstado.SelectedIndex > 0)
            {
                string estado = cmbFiltroEstado.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(filtro))
                    filtro += " AND ";
                filtro += $"Tipo = '{estado}'";
            }

            try
            {
                if (string.IsNullOrEmpty(filtro))
                    dtMovimientos.DefaultView.RowFilter = "";
                else
                    dtMovimientos.DefaultView.RowFilter = filtro;
            }
            catch
            {
                // Si hay error en el filtro, ignorar
            }
        }

        private void ExportarDatos(string formato)
        {
            try
            {
                // Obtener TODOS los datos sin paginación
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

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                string contenido = "";

                switch (formato)
                {
                    case "PDF":
                        saveDialog.Filter = "Archivo HTML (*.html)|*.html";
                        saveDialog.FileName = $"Historial_Movimientos_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                        saveDialog.Title = "Exportar a PDF (se abrirá en navegador)";
                        
                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            contenido = GenerarHTML(dt);
                            System.IO.File.WriteAllText(saveDialog.FileName, contenido);
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveDialog.FileName) { UseShellExecute = true });
                            MessageBox.Show("Archivo HTML generado. Se abrirá en su navegador.\nDesde ahí puede guardarlo como PDF usando Ctrl+P.", 
                                "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;

                    case "Word":
                        saveDialog.Filter = "Documento Word (*.doc)|*.doc";
                        saveDialog.FileName = $"Historial_Movimientos_{DateTime.Now:yyyyMMdd_HHmmss}.doc";
                        
                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            contenido = GenerarWord(dt);
                            System.IO.File.WriteAllText(saveDialog.FileName, contenido);
                            MessageBox.Show($"Documento Word generado exitosamente en:\n{saveDialog.FileName}", 
                                "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;

                    case "Excel":
                        saveDialog.Filter = "Archivo CSV (*.csv)|*.csv";
                        saveDialog.FileName = $"Historial_Movimientos_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                        
                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            contenido = GenerarCSV(dt);
                            System.IO.File.WriteAllText(saveDialog.FileName, contenido);
                            MessageBox.Show($"Archivo CSV generado exitosamente en:\n{saveDialog.FileName}\n\nPuede abrirlo con Excel.", 
                                "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerarHTML(DataTable dt)
        {
            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Historial de Movimientos</title>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 40px; }}
        h1 {{ color: #1e40af; text-align: center; }}
        .info {{ background: #f3f4f6; padding: 15px; margin: 20px 0; border-radius: 8px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        th {{ background: #1e40af; color: white; padding: 12px; text-align: left; }}
        td {{ padding: 10px; border-bottom: 1px solid #e5e7eb; }}
        tr:nth-child(even) {{ background: #f9fafb; }}
        .footer {{ text-align: center; margin-top: 30px; color: #6b7280; font-size: 12px; }}
    </style>
</head>
<body>
    <h1>🏦 Historial de Movimientos</h1>
    <div class='info'>
        <strong>Usuario:</strong> {FormLogin.NombreUsuario}<br>
        <strong>Fecha de generación:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}<br>
        <strong>Total de movimientos:</strong> {dt.Rows.Count}
    </div>
    <table>
        <tr>
            <th>Fecha</th>
            <th>Tipo</th>
            <th>Monto</th>
            <th>Concepto</th>
            <th>Saldo Anterior</th>
            <th>Saldo Nuevo</th>
        </tr>";

            foreach (DataRow row in dt.Rows)
            {
                html += $@"
        <tr>
            <td>{Convert.ToDateTime(row["Fecha"]):dd/MM/yyyy}</td>
            <td>{row["Tipo"]}</td>
            <td>${Convert.ToDecimal(row["Monto"]):N2}</td>
            <td>{row["Concepto"]}</td>
            <td>${Convert.ToDecimal(row["Saldo Anterior"]):N2}</td>
            <td>${Convert.ToDecimal(row["Saldo Nuevo"]):N2}</td>
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

        private string GenerarWord(DataTable dt)
        {
            string doc = $@"
MÓDULO BANCO - HISTORIAL DE MOVIMIENTOS
========================================

Usuario: {FormLogin.NombreUsuario}
Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
Total de movimientos: {dt.Rows.Count}

----------------------------------------

";

            foreach (DataRow row in dt.Rows)
            {
                doc += $@"
Fecha: {Convert.ToDateTime(row["Fecha"]):dd/MM/yyyy}
Tipo: {row["Tipo"]}
Monto: ${Convert.ToDecimal(row["Monto"]):N2}
Concepto: {row["Concepto"]}
Saldo Anterior: ${Convert.ToDecimal(row["Saldo Anterior"]):N2}
Saldo Nuevo: ${Convert.ToDecimal(row["Saldo Nuevo"]):N2}
----------------------------------------
";
            }

            doc += @"

© 2025 Módulo Banco - Documento Confidencial
";

            return doc;
        }

        private string GenerarCSV(DataTable dt)
        {
            string csv = "# HISTORIAL DE MOVIMIENTOS\n";
            csv += $"# Usuario: {FormLogin.NombreUsuario}\n";
            csv += $"# Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n";
            csv += $"# Total de movimientos: {dt.Rows.Count}\n\n";
            csv += "Fecha,Tipo,Monto,Concepto,Saldo Anterior,Saldo Nuevo\n";

            foreach (DataRow row in dt.Rows)
            {
                csv += $"{Convert.ToDateTime(row["Fecha"]):dd/MM/yyyy},";
                csv += $"\"{row["Tipo"]}\",";
                csv += $"{Convert.ToDecimal(row["Monto"]):N2},";
                csv += $"\"{row["Concepto"]}\",";
                csv += $"{Convert.ToDecimal(row["Saldo Anterior"]):N2},";
                csv += $"{Convert.ToDecimal(row["Saldo Nuevo"]):N2}\n";
            }

            return csv;
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
                                ORDER BY fecha DESC
                                LIMIT 100";
                
                dtMovimientos = Database.ExecuteQuery(query, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));
                dgvMovimientos.DataSource = dtMovimientos;

                // Formato de columnas
                if (dgvMovimientos.Columns.Count > 0)
                {
                    dgvMovimientos.Columns["Monto"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.Columns["Saldo Anterior"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.Columns["Saldo Nuevo"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Aplicar colores según tipo de movimiento
                    dgvMovimientos.CellFormatting += (s, e) =>
                    {
                        if (e.ColumnIndex == dgvMovimientos.Columns["Tipo"].Index && e.Value != null)
                        {
                            string tipo = e.Value.ToString();
                            if (tipo.Contains("DEPOSITO") || tipo.Contains("RECIBIDA") || tipo.Contains("ABONO"))
                            {
                                e.CellStyle.BackColor = System.Drawing.Color.FromArgb(220, 252, 231);
                                e.CellStyle.ForeColor = System.Drawing.Color.FromArgb(22, 101, 52);
                            }
                            else if (tipo.Contains("RETIRO") || tipo.Contains("ENVIADA") || tipo.Contains("CARGO"))
                            {
                                e.CellStyle.BackColor = System.Drawing.Color.FromArgb(254, 226, 226);
                                e.CellStyle.ForeColor = System.Drawing.Color.FromArgb(153, 27, 27);
                            }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
