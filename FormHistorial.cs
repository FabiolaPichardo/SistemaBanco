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
            this.ClientSize = new System.Drawing.Size(1100, 750);
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
                Text = "📊 HISTORIAL DE MOVIMIENTOS",
                Location = new System.Drawing.Point(350, 25),
                Size = new System.Drawing.Size(400, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            HomeButton.AddToForm(this, headerPanel);

            headerPanel.Controls.Add(lblTitulo);

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

            Panel mainCard = BankTheme.CreateCard(30, 200, 1040, 440);

            dgvMovimientos = new DataGridView
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(1020, 420),
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

            Button btnExportarPDF = new Button
            {
                Text = "📄 EXPORTAR A PDF",
                Location = new System.Drawing.Point(350, 655),
                Size = new System.Drawing.Size(180, 40)
            };
            BankTheme.StyleButton(btnExportarPDF, false);
            btnExportarPDF.Click += (s, e) => ExportarDatos("PDF");

            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new System.Drawing.Point(540, 655),
                Size = new System.Drawing.Size(180, 40)
            };
            BankTheme.StyleButton(btnCerrar, false);
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, panelFiltros, mainCard, btnExportarPDF, btnCerrar });
        }

        private void TxtBusqueda_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            if (dtMovimientos == null) return;

            string filtro = "";

            if (!string.IsNullOrWhiteSpace(txtBusqueda.Text))
            {
                string busqueda = txtBusqueda.Text.Trim().ToLower();
                filtro = $"Concepto LIKE '%{busqueda}%' OR Tipo LIKE '%{busqueda}%'";
            }

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

            }
        }

        private void ExportarDatos(string formato)
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

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ExportHelper.ExportarPDF(dt, "Historial de Movimientos", "Historial_Movimientos");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                if (dgvMovimientos.Columns.Count > 0)
                {
                    dgvMovimientos.Columns["Monto"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.Columns["Saldo Anterior"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.Columns["Saldo Nuevo"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

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
