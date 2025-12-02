using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using Npgsql;

namespace SistemaBanco
{
    // BAN-32 a BAN-40: Revisión Completa de Movimientos Financieros
    public partial class FormRevisionMovimientos : Form
    {
        private DataGridView dgvMovimientos;
        private TextBox txtBusqueda; // BAN-37
        private ComboBox cmbBeneficiario; // BAN-36
        private Label lblTotalMovimientos; // BAN-40
        private Label lblTotalCargos; // BAN-40
        private Label lblTotalAbonos; // BAN-40
        private Label lblSaldoResultante; // BAN-40
        private string ordenActual = "fecha_hora DESC";
        private string filtroTipo = "TODOS";

        public FormRevisionMovimientos()
        {
            InitializeComponent();
            CargarBeneficiarios();
            CargarMovimientos();
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Revisión de Movimientos Financieros";
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Header
            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(1400, 80),
                BackColor = BankTheme.PrimaryBlue
            };

            HomeButton.AddToForm(this, headerPanel);

            Label lblTitulo = new Label
            {
                Text = "🏦 REVISIÓN DE MOVIMIENTOS FINANCIEROS",
                Location = new System.Drawing.Point(100, 25),
                Size = new System.Drawing.Size(700, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White
            };

            headerPanel.Controls.Add(lblTitulo);

            // BAN-37: Barra de Búsqueda General
            Panel panelBusqueda = new Panel
            {
                Location = new System.Drawing.Point(20, 100),
                Size = new System.Drawing.Size(1360, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblBuscar = new Label
            {
                Text = "🔍",
                Location = new System.Drawing.Point(10, 15),
                Size = new System.Drawing.Size(30, 20),
                Font = new Font("Segoe UI", 12F)
            };

            txtBusqueda = new TextBox
            {
                Location = new System.Drawing.Point(45, 12),
                Size = new System.Drawing.Size(400, 25),
                Font = BankTheme.BodyFont,
                PlaceholderText = "Buscar por concepto, referencia, monto..."
            };
            txtBusqueda.TextChanged += TxtBusqueda_TextChanged;

            // BAN-38: Botón Limpiar Filtros
            Button btnLimpiarTodo = new Button
            {
                Text = "🗑️ LIMPIAR FILTROS",
                Location = new System.Drawing.Point(1200, 8),
                Size = new System.Drawing.Size(150, 35),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(60, 60, 60),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnLimpiarTodo.FlatAppearance.BorderColor = Color.Gray;
            btnLimpiarTodo.Click += (s, e) => LimpiarTodosFiltros();

            panelBusqueda.Controls.AddRange(new Control[] { lblBuscar, txtBusqueda, btnLimpiarTodo });

            // BAN-35 y BAN-36: Botones de Acceso Rápido
            Panel panelAccesoRapido = new Panel
            {
                Location = new System.Drawing.Point(20, 160),
                Size = new System.Drawing.Size(1360, 50),
                BackColor = Color.White
            };

            Button btnVerTodos = new Button
            {
                Text = "📋 VER TODOS",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(150, 30),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnVerTodos.FlatAppearance.BorderSize = 0;
            btnVerTodos.Click += (s, e) => { filtroTipo = "TODOS"; CargarMovimientos(); };

            Button btnVerCargos = new Button
            {
                Text = "➖ SOLO CARGOS",
                Location = new System.Drawing.Point(170, 10),
                Size = new System.Drawing.Size(150, 30),
                BackColor = Color.FromArgb(255, 193, 193),
                ForeColor = Color.FromArgb(139, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            btnVerCargos.FlatAppearance.BorderSize = 0;
            btnVerCargos.Click += (s, e) => { filtroTipo = "CARGO"; CargarMovimientos(); };

            Button btnVerAbonos = new Button
            {
                Text = "➕ SOLO ABONOS",
                Location = new System.Drawing.Point(330, 10),
                Size = new System.Drawing.Size(150, 30),
                BackColor = Color.FromArgb(193, 255, 193),
                ForeColor = Color.FromArgb(0, 100, 0),
                FlatStyle = FlatStyle.Flat
            };
            btnVerAbonos.FlatAppearance.BorderSize = 0;
            btnVerAbonos.Click += (s, e) => { filtroTipo = "ABONO"; CargarMovimientos(); };

            // BAN-36: Filtro por Beneficiario
            Label lblBeneficiario = new Label
            {
                Text = "👤 Beneficiario:",
                Location = new System.Drawing.Point(500, 15),
                Size = new System.Drawing.Size(100, 20),
                Font = BankTheme.SmallFont
            };

            cmbBeneficiario = new ComboBox
            {
                Location = new System.Drawing.Point(610, 12),
                Size = new System.Drawing.Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = BankTheme.SmallFont
            };
            cmbBeneficiario.SelectedIndexChanged += (s, e) => CargarMovimientos();

            Button btnExportPDF = new Button
            {
                Text = "📄 PDF",
                Location = new System.Drawing.Point(1280, 10),
                Size = new System.Drawing.Size(70, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExportPDF.FlatAppearance.BorderSize = 0;

            panelAccesoRapido.Controls.AddRange(new Control[] {
                btnVerTodos, btnVerCargos, btnVerAbonos, lblBeneficiario, cmbBeneficiario, btnExportPDF
            });

            // BAN-40: Panel de Resumen Ejecutivo
            Panel panelResumen = new Panel
            {
                Location = new System.Drawing.Point(20, 220),
                Size = new System.Drawing.Size(1360, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblResumenTitulo = new Label
            {
                Text = "📊 RESUMEN EJECUTIVO",
                Location = new System.Drawing.Point(15, 10),
                Size = new System.Drawing.Size(300, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = BankTheme.PrimaryBlue
            };

            // Total Movimientos
            Label lblTotalMovTitulo = new Label
            {
                Text = "Total Movimientos:",
                Location = new System.Drawing.Point(30, 40),
                Size = new System.Drawing.Size(150, 15),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };

            lblTotalMovimientos = new Label
            {
                Text = "0",
                Location = new System.Drawing.Point(30, 55),
                Size = new System.Drawing.Size(150, 20),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = BankTheme.PrimaryBlue
            };

            // Total Cargos
            Label lblCargosTitulo = new Label
            {
                Text = "Total Cargos:",
                Location = new System.Drawing.Point(350, 40),
                Size = new System.Drawing.Size(250, 15),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };

            lblTotalCargos = new Label
            {
                Text = "$0.00",
                Location = new System.Drawing.Point(350, 55),
                Size = new System.Drawing.Size(250, 20),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69)
            };

            // Total Abonos
            Label lblAbonosTitulo = new Label
            {
                Text = "Total Abonos:",
                Location = new System.Drawing.Point(670, 40),
                Size = new System.Drawing.Size(250, 15),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };

            lblTotalAbonos = new Label
            {
                Text = "$0.00",
                Location = new System.Drawing.Point(670, 55),
                Size = new System.Drawing.Size(250, 20),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69)
            };

            // Saldo Resultante
            Label lblSaldoTitulo = new Label
            {
                Text = "Saldo Resultante:",
                Location = new System.Drawing.Point(1000, 40),
                Size = new System.Drawing.Size(300, 15),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };

            lblSaldoResultante = new Label
            {
                Text = "$0.00",
                Location = new System.Drawing.Point(1000, 55),
                Size = new System.Drawing.Size(300, 20),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = BankTheme.PrimaryBlue
            };

            panelResumen.Controls.AddRange(new Control[] {
                lblResumenTitulo, lblTotalMovTitulo, lblTotalMovimientos,
                lblCargosTitulo, lblTotalCargos, lblAbonosTitulo, lblTotalAbonos,
                lblSaldoTitulo, lblSaldoResultante
            });

            // BAN-39: DataGridView con formato mejorado
            dgvMovimientos = new DataGridView
            {
                Location = new System.Drawing.Point(20, 310),
                Size = new System.Drawing.Size(1360, 520),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 35 }
            };

            dgvMovimientos.ColumnHeadersDefaultCellStyle.BackColor = BankTheme.PrimaryBlue;
            dgvMovimientos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMovimientos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvMovimientos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMovimientos.EnableHeadersVisualStyles = false;
            dgvMovimientos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            // BAN-34: Ordenamiento por columnas
            dgvMovimientos.ColumnHeaderMouseClick += DgvMovimientos_ColumnHeaderMouseClick;

            // Pie de página
            Label lblPie = new Label
            {
                Text = $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Usuario: {FormLogin.NombreUsuario}",
                Location = new System.Drawing.Point(20, 840),
                Size = new System.Drawing.Size(1360, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.Controls.AddRange(new Control[] {
                headerPanel, panelBusqueda, panelAccesoRapido, panelResumen, dgvMovimientos, lblPie
            });
        }

        // BAN-36: Cargar beneficiarios
        private void CargarBeneficiarios()
        {
            try
            {
                cmbBeneficiario.Items.Clear();
                cmbBeneficiario.Items.Add("Todos los beneficiarios");

                string query = "SELECT nombre_completo FROM beneficiarios WHERE activo = TRUE ORDER BY nombre_completo";
                DataTable dt = Database.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    cmbBeneficiario.Items.Add(row["nombre_completo"].ToString());
                }

                cmbBeneficiario.SelectedIndex = 0;
            }
            catch
            {
                cmbBeneficiario.Items.Add("Todos los beneficiarios");
                cmbBeneficiario.SelectedIndex = 0;
            }
        }

        private void CargarMovimientos()
        {
            try
            {
                string query = @"SELECT 
                    folio as ""Folio"",
                    fecha_hora as ""Fecha"",
                    tipo_operacion as ""Tipo"",
                    concepto as ""Descripción"",
                    beneficiario as ""Beneficiario"",
                    importe as ""Monto"",
                    moneda as ""Moneda"",
                    estado as ""Estado"",
                    referencia as ""Referencia""
                FROM movimientos_financieros
                WHERE 1=1";

                // Filtro de tipo (BAN-35)
                if (filtroTipo != "TODOS")
                {
                    query += $" AND tipo_operacion = '{filtroTipo}'";
                }

                // BAN-36: Filtro por beneficiario
                if (cmbBeneficiario != null && cmbBeneficiario.SelectedIndex > 0)
                {
                    string beneficiario = cmbBeneficiario.SelectedItem.ToString().Replace("'", "''");
                    query += $" AND beneficiario = '{beneficiario}'";
                }

                // BAN-37: Búsqueda general
                if (!string.IsNullOrWhiteSpace(txtBusqueda?.Text))
                {
                    string busqueda = txtBusqueda.Text.Replace("'", "''");
                    query += $@" AND (
                        concepto ILIKE '%{busqueda}%' OR 
                        referencia ILIKE '%{busqueda}%' OR 
                        beneficiario ILIKE '%{busqueda}%' OR
                        CAST(importe AS TEXT) LIKE '%{busqueda}%'
                    )";
                }

                query += $" ORDER BY {ordenActual}";

                DataTable dt = Database.ExecuteQuery(query);
                dgvMovimientos.DataSource = dt;

                // BAN-39: Formato de columnas
                if (dgvMovimientos.Columns.Contains("Fecha"))
                    dgvMovimientos.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                if (dgvMovimientos.Columns.Contains("Monto"))
                {
                    dgvMovimientos.Columns["Monto"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.Columns["Monto"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvMovimientos.Columns["Monto"].DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                }

                // Colorear estados (BAN-27)
                foreach (DataGridViewRow row in dgvMovimientos.Rows)
                {
                    string estado = row.Cells["Estado"].Value?.ToString() ?? "";
                    switch (estado)
                    {
                        case "PENDIENTE":
                            row.Cells["Estado"].Style.BackColor = Color.FromArgb(255, 243, 205);
                            row.Cells["Estado"].Style.ForeColor = Color.FromArgb(133, 100, 4);
                            break;
                        case "PROCESADO":
                            row.Cells["Estado"].Style.BackColor = Color.FromArgb(212, 237, 218);
                            row.Cells["Estado"].Style.ForeColor = Color.FromArgb(21, 87, 36);
                            break;
                        case "RECHAZADO":
                            row.Cells["Estado"].Style.BackColor = Color.FromArgb(248, 215, 218);
                            row.Cells["Estado"].Style.ForeColor = Color.FromArgb(114, 28, 36);
                            break;
                    }
                }

                // BAN-40: Actualizar resumen ejecutivo
                ActualizarResumen(dt);

                // BAN-39: Mensaje si no hay resultados
                if (dt.Rows.Count == 0)
                {
                    lblTotalMovimientos.Text = "No se encontraron movimientos";
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Cargar Movimientos",
                    $"No se pudieron cargar los movimientos.\n\n{ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        // BAN-40: Actualizar resumen ejecutivo
        private void ActualizarResumen(DataTable dt)
        {
            try
            {
                int totalMovimientos = dt.Rows.Count;
                decimal totalCargos = 0;
                decimal totalAbonos = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string tipo = row["Tipo"].ToString();
                    decimal monto = Convert.ToDecimal(row["Monto"]);

                    if (tipo == "CARGO")
                        totalCargos += monto;
                    else if (tipo == "ABONO")
                        totalAbonos += monto;
                }

                decimal saldoResultante = totalAbonos - totalCargos;

                lblTotalMovimientos.Text = totalMovimientos.ToString();
                lblTotalCargos.Text = totalCargos.ToString("C2");
                lblTotalAbonos.Text = totalAbonos.ToString("C2");
                lblSaldoResultante.Text = saldoResultante.ToString("C2");

                // Colorear saldo según sea positivo o negativo
                lblSaldoResultante.ForeColor = saldoResultante >= 0 ? 
                    Color.FromArgb(40, 167, 69) : Color.FromArgb(220, 53, 69);
            }
            catch
            {
                lblTotalMovimientos.Text = "0";
                lblTotalCargos.Text = "$0.00";
                lblTotalAbonos.Text = "$0.00";
                lblSaldoResultante.Text = "$0.00";
            }
        }

        // BAN-37: Búsqueda en tiempo real
        private void TxtBusqueda_TextChanged(object sender, EventArgs e)
        {
            CargarMovimientos();
        }

        // BAN-38: Limpiar todos los filtros
        private void LimpiarTodosFiltros()
        {
            txtBusqueda.Clear();
            if (cmbBeneficiario != null)
                cmbBeneficiario.SelectedIndex = 0;
            filtroTipo = "TODOS";
            ordenActual = "fecha_hora DESC";
            CargarMovimientos();
        }

        // BAN-34: Ordenamiento dinámico
        private void DgvMovimientos_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string columnName = dgvMovimientos.Columns[e.ColumnIndex].Name;
            
            if (columnName == "Fecha")
            {
                ordenActual = ordenActual.Contains("DESC") ? "fecha_hora ASC" : "fecha_hora DESC";
                CargarMovimientos();
            }
            else if (columnName == "Monto")
            {
                ordenActual = ordenActual.Contains("importe DESC") ? "importe ASC" : "importe DESC";
                CargarMovimientos();
            }
        }
    }
}
