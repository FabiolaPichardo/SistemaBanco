using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using Npgsql;

namespace SistemaBanco
{
    // BAN-32 a BAN-50: Revisión Completa de Movimientos Financieros
    public partial class FormRevisionMovimientos : Form
    {
        private DataGridView dgvMovimientos;
        private TextBox txtBusqueda; // BAN-37
        private ComboBox cmbBeneficiario; // BAN-36
        private Label lblTotalMovimientos; // BAN-40
        private Label lblTotalCargos; // BAN-40
        private Label lblTotalAbonos; // BAN-40
        private Label lblSaldoResultante; // BAN-40
        private Label lblUltimaActualizacion; // BAN-48
        private string ordenActual = "fecha_hora DESC";
        private string filtroTipo = "TODOS";
        private int paginaActual = 1; // BAN-45
        private int registrosPorPagina = 20; // BAN-45
        private int totalPaginas = 1; // BAN-45
        private System.Windows.Forms.Timer timerActualizacion; // BAN-48

        public FormRevisionMovimientos()
        {
            InitializeComponent();
            CargarBeneficiarios();
            CargarMovimientos();
            IniciarActualizacionAutomatica(); // BAN-48
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

            // BAN-46: Botones de exportación
            Button btnExportPDF = new Button
            {
                Text = "📄 PDF",
                Location = new System.Drawing.Point(1150, 10),
                Size = new System.Drawing.Size(65, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = BankTheme.SmallFont
            };
            btnExportPDF.FlatAppearance.BorderSize = 0;
            btnExportPDF.Click += (s, e) => ExportarDatos("PDF");

            Button btnExportWord = new Button
            {
                Text = "📝 Word",
                Location = new System.Drawing.Point(1220, 10),
                Size = new System.Drawing.Size(65, 30),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = BankTheme.SmallFont
            };
            btnExportWord.FlatAppearance.BorderSize = 0;
            btnExportWord.Click += (s, e) => ExportarDatos("WORD");

            Button btnExportExcel = new Button
            {
                Text = "📊 Excel",
                Location = new System.Drawing.Point(1290, 10),
                Size = new System.Drawing.Size(65, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = BankTheme.SmallFont
            };
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.Click += (s, e) => ExportarDatos("EXCEL");

            panelAccesoRapido.Controls.AddRange(new Control[] {
                btnVerTodos, btnVerCargos, btnVerAbonos, lblBeneficiario, cmbBeneficiario, 
                btnExportPDF, btnExportWord, btnExportExcel
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
            
            // BAN-41: Doble clic para ver detalles
            dgvMovimientos.CellDoubleClick += DgvMovimientos_CellDoubleClick;

            // BAN-45: Panel de paginación
            Panel panelPaginacion = new Panel
            {
                Location = new System.Drawing.Point(20, 835),
                Size = new System.Drawing.Size(1360, 40),
                BackColor = Color.White
            };

            Button btnPaginaAnterior = new Button
            {
                Text = "◀ Anterior",
                Location = new System.Drawing.Point(500, 5),
                Size = new System.Drawing.Size(120, 30),
                BackColor = BankTheme.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = BankTheme.SmallFont
            };
            btnPaginaAnterior.FlatAppearance.BorderSize = 0;
            btnPaginaAnterior.Click += (s, e) => CambiarPagina(-1);

            Label lblPaginaInfo = new Label
            {
                Name = "lblPaginaInfo",
                Text = "Página 1 de 1",
                Location = new System.Drawing.Point(630, 10),
                Size = new System.Drawing.Size(120, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnPaginaSiguiente = new Button
            {
                Text = "Siguiente ▶",
                Location = new System.Drawing.Point(760, 5),
                Size = new System.Drawing.Size(120, 30),
                BackColor = BankTheme.PrimaryBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = BankTheme.SmallFont
            };
            btnPaginaSiguiente.FlatAppearance.BorderSize = 0;
            btnPaginaSiguiente.Click += (s, e) => CambiarPagina(1);

            panelPaginacion.Controls.AddRange(new Control[] { btnPaginaAnterior, lblPaginaInfo, btnPaginaSiguiente });

            // BAN-48 y BAN-50: Pie de página con última actualización y botón refrescar
            Panel panelPie = new Panel
            {
                Location = new System.Drawing.Point(20, 880),
                Size = new System.Drawing.Size(1360, 30),
                BackColor = Color.Transparent
            };

            lblUltimaActualizacion = new Label
            {
                Text = $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Usuario: {FormLogin.NombreUsuario}",
                Location = new System.Drawing.Point(0, 5),
                Size = new System.Drawing.Size(1100, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // BAN-50: Botón refrescar manual
            Button btnRefrescar = new Button
            {
                Text = "🔄 Refrescar",
                Location = new System.Drawing.Point(1240, 0),
                Size = new System.Drawing.Size(120, 25),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = BankTheme.SmallFont
            };
            btnRefrescar.FlatAppearance.BorderSize = 0;
            btnRefrescar.Click += (s, e) => RefrescarManual();

            panelPie.Controls.AddRange(new Control[] { lblUltimaActualizacion, btnRefrescar });

            this.Controls.AddRange(new Control[] {
                headerPanel, panelBusqueda, panelAccesoRapido, panelResumen, dgvMovimientos, panelPaginacion, panelPie
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
                // BAN-45: Primero contar total de registros para paginación
                string countQuery = @"SELECT COUNT(*) as total FROM movimientos_financieros WHERE 1=1";

                // Filtro de tipo (BAN-35)
                if (filtroTipo != "TODOS")
                {
                    countQuery += $" AND tipo_operacion = '{filtroTipo}'";
                }

                // BAN-36: Filtro por beneficiario
                if (cmbBeneficiario != null && cmbBeneficiario.SelectedIndex > 0)
                {
                    string beneficiario = cmbBeneficiario.SelectedItem.ToString().Replace("'", "''");
                    countQuery += $" AND beneficiario = '{beneficiario}'";
                }

                // BAN-37: Búsqueda general
                if (!string.IsNullOrWhiteSpace(txtBusqueda?.Text))
                {
                    string busqueda = txtBusqueda.Text.Replace("'", "''");
                    countQuery += $@" AND (
                        concepto ILIKE '%{busqueda}%' OR 
                        referencia ILIKE '%{busqueda}%' OR 
                        beneficiario ILIKE '%{busqueda}%' OR
                        CAST(importe AS TEXT) LIKE '%{busqueda}%'
                    )";
                }

                DataTable dtCount = Database.ExecuteQuery(countQuery);
                int totalRegistros = Convert.ToInt32(dtCount.Rows[0]["total"]);
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
                
                if (totalPaginas == 0) totalPaginas = 1;
                if (paginaActual > totalPaginas) paginaActual = totalPaginas;

                // Ahora cargar los datos con paginación
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

                // Aplicar los mismos filtros
                if (filtroTipo != "TODOS")
                {
                    query += $" AND tipo_operacion = '{filtroTipo}'";
                }

                if (cmbBeneficiario != null && cmbBeneficiario.SelectedIndex > 0)
                {
                    string beneficiario = cmbBeneficiario.SelectedItem.ToString().Replace("'", "''");
                    query += $" AND beneficiario = '{beneficiario}'";
                }

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
                
                // BAN-45: Agregar LIMIT y OFFSET para paginación
                int offset = (paginaActual - 1) * registrosPorPagina;
                query += $" LIMIT {registrosPorPagina} OFFSET {offset}";

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

                // BAN-40: Actualizar resumen ejecutivo (con todos los datos, no solo la página actual)
                ActualizarResumenCompleto();

                // BAN-45: Actualizar información de paginación
                ActualizarInfoPaginacion();

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

        // BAN-40: Actualizar resumen con TODOS los datos (no solo página actual)
        private void ActualizarResumenCompleto()
        {
            try
            {
                string queryResumen = @"SELECT 
                    COUNT(*) as total,
                    SUM(CASE WHEN tipo_operacion = 'CARGO' THEN importe ELSE 0 END) as total_cargos,
                    SUM(CASE WHEN tipo_operacion = 'ABONO' THEN importe ELSE 0 END) as total_abonos
                FROM movimientos_financieros
                WHERE 1=1";

                // Aplicar los mismos filtros
                if (filtroTipo != "TODOS")
                {
                    queryResumen += $" AND tipo_operacion = '{filtroTipo}'";
                }

                if (cmbBeneficiario != null && cmbBeneficiario.SelectedIndex > 0)
                {
                    string beneficiario = cmbBeneficiario.SelectedItem.ToString().Replace("'", "''");
                    queryResumen += $" AND beneficiario = '{beneficiario}'";
                }

                if (!string.IsNullOrWhiteSpace(txtBusqueda?.Text))
                {
                    string busqueda = txtBusqueda.Text.Replace("'", "''");
                    queryResumen += $@" AND (
                        concepto ILIKE '%{busqueda}%' OR 
                        referencia ILIKE '%{busqueda}%' OR 
                        beneficiario ILIKE '%{busqueda}%' OR
                        CAST(importe AS TEXT) LIKE '%{busqueda}%'
                    )";
                }

                DataTable dtResumen = Database.ExecuteQuery(queryResumen);
                
                if (dtResumen.Rows.Count > 0)
                {
                    int totalMovimientos = Convert.ToInt32(dtResumen.Rows[0]["total"]);
                    decimal totalCargos = dtResumen.Rows[0]["total_cargos"] != DBNull.Value ? 
                        Convert.ToDecimal(dtResumen.Rows[0]["total_cargos"]) : 0;
                    decimal totalAbonos = dtResumen.Rows[0]["total_abonos"] != DBNull.Value ? 
                        Convert.ToDecimal(dtResumen.Rows[0]["total_abonos"]) : 0;
                    decimal saldoResultante = totalAbonos - totalCargos;

                    lblTotalMovimientos.Text = totalMovimientos.ToString();
                    lblTotalCargos.Text = totalCargos.ToString("C2");
                    lblTotalAbonos.Text = totalAbonos.ToString("C2");
                    lblSaldoResultante.Text = saldoResultante.ToString("C2");

                    // Colorear saldo según sea positivo o negativo
                    lblSaldoResultante.ForeColor = saldoResultante >= 0 ? 
                        Color.FromArgb(40, 167, 69) : Color.FromArgb(220, 53, 69);
                }
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

        // ============================================
        // BAN-41: DETALLES EXPANDIBLES (MODAL)
        // ============================================
        private void DgvMovimientos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                string folio = dgvMovimientos.Rows[e.RowIndex].Cells["Folio"].Value.ToString();
                MostrarDetallesMovimiento(folio);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", $"No se pudieron cargar los detalles.\n{ex.Message}", MessageBoxIcon.Error);
            }
        }

        private void MostrarDetallesMovimiento(string folio)
        {
            try
            {
                string query = $@"SELECT * FROM movimientos_financieros WHERE folio = '{folio}'";
                DataTable dt = Database.ExecuteQuery(query);

                if (dt.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Error", "No se encontró el movimiento.", MessageBoxIcon.Warning);
                    return;
                }

                DataRow row = dt.Rows[0];

                // Crear modal de detalles
                Form modalDetalles = new Form
                {
                    Text = $"Detalles del Movimiento - {folio}",
                    Size = new Size(700, 650),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.White
                };

                Panel panelHeader = new Panel
                {
                    Location = new Point(0, 0),
                    Size = new Size(700, 60),
                    BackColor = BankTheme.PrimaryBlue
                };

                Label lblTituloModal = new Label
                {
                    Text = $"📄 FOLIO: {folio}",
                    Location = new Point(20, 15),
                    Size = new Size(660, 30),
                    Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                    ForeColor = Color.White
                };

                panelHeader.Controls.Add(lblTituloModal);

                // Panel de contenido con scroll
                Panel panelContenido = new Panel
                {
                    Location = new Point(20, 80),
                    Size = new Size(640, 420),
                    AutoScroll = true,
                    BackColor = Color.White
                };

                int yPos = 10;

                // Información del movimiento
                AgregarCampoDetalle(panelContenido, "Fecha y Hora:", row["fecha_hora"].ToString(), ref yPos);
                AgregarCampoDetalle(panelContenido, "Tipo de Operación:", row["tipo_operacion"].ToString(), ref yPos);
                AgregarCampoDetalle(panelContenido, "Cuenta Ordenante:", row["cuenta_ordenante"].ToString(), ref yPos);
                AgregarCampoDetalle(panelContenido, "Cuenta Beneficiaria:", row["cuenta_beneficiaria"].ToString(), ref yPos);
                AgregarCampoDetalle(panelContenido, "Beneficiario:", row["beneficiario"].ToString(), ref yPos);
                AgregarCampoDetalle(panelContenido, "Importe:", Convert.ToDecimal(row["importe"]).ToString("C2"), ref yPos);
                AgregarCampoDetalle(panelContenido, "Moneda:", row["moneda"].ToString(), ref yPos);
                AgregarCampoDetalle(panelContenido, "Concepto:", row["concepto"].ToString(), ref yPos);
                AgregarCampoDetalle(panelContenido, "Referencia:", row["referencia"]?.ToString() ?? "N/A", ref yPos);
                AgregarCampoDetalle(panelContenido, "Cuenta Contable:", row["cuenta_contable"].ToString(), ref yPos);
                AgregarCampoDetalle(panelContenido, "Estado:", row["estado"].ToString(), ref yPos);

                // Panel de botones
                Panel panelBotones = new Panel
                {
                    Location = new Point(20, 510),
                    Size = new Size(640, 60),
                    BackColor = Color.WhiteSmoke
                };

                // BAN-42: Botón descargar comprobante PDF
                Button btnDescargarPDF = new Button
                {
                    Text = "📄 Descargar Comprobante PDF",
                    Location = new Point(20, 15),
                    Size = new Size(200, 35),
                    BackColor = Color.FromArgb(220, 53, 69),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnDescargarPDF.FlatAppearance.BorderSize = 0;
                btnDescargarPDF.Click += (s, e) => DescargarComprobantePDF(folio);

                // BAN-43: Botón editar (solo usuarios autorizados)
                Button btnEditar = new Button
                {
                    Text = "✏️ Editar",
                    Location = new Point(230, 15),
                    Size = new Size(120, 35),
                    BackColor = Color.FromArgb(255, 193, 7),
                    ForeColor = Color.Black,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Enabled = EsUsuarioAutorizado()
                };
                btnEditar.FlatAppearance.BorderSize = 0;
                btnEditar.Click += (s, e) => EditarMovimiento(folio, modalDetalles);

                // BAN-44: Botón eliminar (solo usuarios autorizados)
                Button btnEliminar = new Button
                {
                    Text = "🗑️ Eliminar",
                    Location = new Point(360, 15),
                    Size = new Size(120, 35),
                    BackColor = Color.FromArgb(108, 117, 125),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Enabled = EsUsuarioAutorizado()
                };
                btnEliminar.FlatAppearance.BorderSize = 0;
                btnEliminar.Click += (s, e) => EliminarMovimiento(folio, modalDetalles);

                Button btnCerrar = new Button
                {
                    Text = "Cerrar",
                    Location = new Point(490, 15),
                    Size = new Size(120, 35),
                    BackColor = BankTheme.PrimaryBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnCerrar.FlatAppearance.BorderSize = 0;
                btnCerrar.Click += (s, e) => modalDetalles.Close();

                panelBotones.Controls.AddRange(new Control[] { btnDescargarPDF, btnEditar, btnEliminar, btnCerrar });

                modalDetalles.Controls.AddRange(new Control[] { panelHeader, panelContenido, panelBotones });
                modalDetalles.ShowDialog();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", $"Error al mostrar detalles.\n{ex.Message}", MessageBoxIcon.Error);
            }
        }

        private void AgregarCampoDetalle(Panel panel, string etiqueta, string valor, ref int yPos)
        {
            Label lblEtiqueta = new Label
            {
                Text = etiqueta,
                Location = new Point(10, yPos),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = BankTheme.TextSecondary
            };

            Label lblValor = new Label
            {
                Text = valor,
                Location = new Point(220, yPos),
                Size = new Size(400, 40),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextPrimary,
                AutoSize = false
            };

            panel.Controls.AddRange(new Control[] { lblEtiqueta, lblValor });
            yPos += 45;
        }

        // ============================================
        // BAN-42: DESCARGAR COMPROBANTE PDF
        // ============================================
        private void DescargarComprobantePDF(string folio)
        {
            try
            {
                string query = $@"SELECT * FROM movimientos_financieros WHERE folio = '{folio}'";
                DataTable dt = Database.ExecuteQuery(query);

                if (dt.Rows.Count == 0) return;

                DataRow row = dt.Rows[0];

                // Crear contenido del comprobante
                string contenido = $@"
═══════════════════════════════════════════════════════
            COMPROBANTE DE MOVIMIENTO FINANCIERO
═══════════════════════════════════════════════════════

FOLIO: {folio}
FECHA: {row["fecha_hora"]}
TIPO: {row["tipo_operacion"]}

───────────────────────────────────────────────────────
DETALLES DE LA OPERACIÓN
───────────────────────────────────────────────────────

Cuenta Ordenante:    {row["cuenta_ordenante"]}
Cuenta Beneficiaria: {row["cuenta_beneficiaria"]}
Beneficiario:        {row["beneficiario"]}

IMPORTE:             {Convert.ToDecimal(row["importe"]):C2} {row["moneda"]}

Concepto:            {row["concepto"]}
Referencia:          {row["referencia"]}
Cuenta Contable:     {row["cuenta_contable"]}

Estado:              {row["estado"]}

───────────────────────────────────────────────────────
Este documento es un comprobante válido de la operación
Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
═══════════════════════════════════════════════════════
";

                string rutaArchivo = $"Comprobante_{folio}.txt";
                System.IO.File.WriteAllText(rutaArchivo, contenido);

                CustomMessageBox.Show("Éxito", $"Comprobante descargado:\n{rutaArchivo}", MessageBoxIcon.Information);
                System.Diagnostics.Process.Start("notepad.exe", rutaArchivo);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", $"No se pudo generar el comprobante.\n{ex.Message}", MessageBoxIcon.Error);
            }
        }

        // ============================================
        // BAN-43: EDITAR MOVIMIENTO
        // ============================================
        private bool EsUsuarioAutorizado()
        {
            // Verificar si el usuario tiene rol de Gerente o Administrador
            try
            {
                string query = $@"SELECT rol FROM usuarios WHERE usuario = '{FormLogin.NombreUsuario}'";
                DataTable dt = Database.ExecuteQuery(query);
                
                if (dt.Rows.Count > 0)
                {
                    string rol = dt.Rows[0]["rol"].ToString();
                    return rol == "Gerente" || rol == "Administrador";
                }
            }
            catch { }
            
            return false;
        }

        private void EditarMovimiento(string folio, Form modalPadre)
        {
            try
            {
                string query = $@"SELECT * FROM movimientos_financieros WHERE folio = '{folio}'";
                DataTable dt = Database.ExecuteQuery(query);

                if (dt.Rows.Count == 0) return;

                DataRow row = dt.Rows[0];

                // Crear formulario de edición
                Form formEditar = new Form
                {
                    Text = $"Editar Movimiento - {folio}",
                    Size = new Size(600, 500),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.White
                };

                int yPos = 20;

                // Campos editables
                Label lblConcepto = new Label { Text = "Concepto:", Location = new Point(20, yPos), Size = new Size(150, 20) };
                TextBox txtConcepto = new TextBox { Text = row["concepto"].ToString(), Location = new Point(180, yPos), Size = new Size(380, 25) };
                yPos += 40;

                Label lblReferencia = new Label { Text = "Referencia:", Location = new Point(20, yPos), Size = new Size(150, 20) };
                TextBox txtReferencia = new TextBox { Text = row["referencia"]?.ToString(), Location = new Point(180, yPos), Size = new Size(380, 25) };
                yPos += 40;

                Label lblEstado = new Label { Text = "Estado:", Location = new Point(20, yPos), Size = new Size(150, 20) };
                ComboBox cmbEstado = new ComboBox 
                { 
                    Location = new Point(180, yPos), 
                    Size = new Size(380, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                cmbEstado.Items.AddRange(new string[] { "PENDIENTE", "PROCESADO", "RECHAZADO" });
                cmbEstado.SelectedItem = row["estado"].ToString();
                yPos += 40;

                Label lblComentarios = new Label { Text = "Comentarios:", Location = new Point(20, yPos), Size = new Size(150, 20) };
                TextBox txtComentarios = new TextBox 
                { 
                    Location = new Point(180, yPos), 
                    Size = new Size(380, 80),
                    Multiline = true
                };
                yPos += 100;

                Button btnGuardar = new Button
                {
                    Text = "💾 Guardar Cambios",
                    Location = new Point(180, yPos),
                    Size = new Size(180, 35),
                    BackColor = Color.FromArgb(40, 167, 69),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnGuardar.FlatAppearance.BorderSize = 0;
                btnGuardar.Click += (s, e) =>
                {
                    try
                    {
                        string updateQuery = $@"
                            UPDATE movimientos_financieros 
                            SET concepto = '{txtConcepto.Text.Replace("'", "''")}',
                                referencia = '{txtReferencia.Text.Replace("'", "''")}',
                                estado = '{cmbEstado.SelectedItem}',
                                comentarios_autorizacion = '{txtComentarios.Text.Replace("'", "''")}'
                            WHERE folio = '{folio}'";

                        Database.ExecuteNonQuery(updateQuery);

                        CustomMessageBox.Show("Éxito", "Movimiento actualizado correctamente.", MessageBoxIcon.Information);
                        formEditar.Close();
                        modalPadre.Close();
                        CargarMovimientos();
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show("Error", $"No se pudo actualizar.\n{ex.Message}", MessageBoxIcon.Error);
                    }
                };

                Button btnCancelar = new Button
                {
                    Text = "Cancelar",
                    Location = new Point(380, yPos),
                    Size = new Size(180, 35),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnCancelar.FlatAppearance.BorderSize = 0;
                btnCancelar.Click += (s, e) => formEditar.Close();

                formEditar.Controls.AddRange(new Control[] 
                { 
                    lblConcepto, txtConcepto, lblReferencia, txtReferencia, 
                    lblEstado, cmbEstado, lblComentarios, txtComentarios,
                    btnGuardar, btnCancelar 
                });

                formEditar.ShowDialog();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", $"No se pudo abrir el editor.\n{ex.Message}", MessageBoxIcon.Error);
            }
        }

        // ============================================
        // BAN-44: ELIMINAR CON AUDITORÍA
        // ============================================
        private void EliminarMovimiento(string folio, Form modalPadre)
        {
            try
            {
                DialogResult resultado = MessageBox.Show(
                    $"¿Está seguro de eliminar el movimiento {folio}?\n\nEsta acción quedará registrada en la auditoría.",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    // Soft delete: marcar como eliminado en lugar de borrar
                    string updateQuery = $@"
                        UPDATE movimientos_financieros 
                        SET estado = 'ELIMINADO',
                            comentarios_autorizacion = 'Eliminado por {FormLogin.NombreUsuario} el {DateTime.Now:dd/MM/yyyy HH:mm:ss}'
                        WHERE folio = '{folio}'";

                    Database.ExecuteNonQuery(updateQuery);

                    CustomMessageBox.Show("Éxito", "Movimiento eliminado correctamente.", MessageBoxIcon.Information);
                    modalPadre.Close();
                    CargarMovimientos();
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", $"No se pudo eliminar.\n{ex.Message}", MessageBoxIcon.Error);
            }
        }

        // ============================================
        // BAN-45: PAGINACIÓN
        // ============================================
        private void CambiarPagina(int direccion)
        {
            int nuevaPagina = paginaActual + direccion;

            if (nuevaPagina < 1 || nuevaPagina > totalPaginas)
                return;

            paginaActual = nuevaPagina;
            CargarMovimientos();
        }

        private void ActualizarInfoPaginacion()
        {
            Label lblPaginaInfo = this.Controls.Find("lblPaginaInfo", true).FirstOrDefault() as Label;
            if (lblPaginaInfo != null)
            {
                lblPaginaInfo.Text = $"Página {paginaActual} de {totalPaginas}";
            }
        }

        // ============================================
        // BAN-48: ACTUALIZACIÓN AUTOMÁTICA
        // ============================================
        private void IniciarActualizacionAutomatica()
        {
            timerActualizacion = new System.Windows.Forms.Timer();
            timerActualizacion.Interval = 30000; // 30 segundos
            timerActualizacion.Tick += (s, e) => ActualizarAutomaticamente();
            timerActualizacion.Start();
        }

        private void ActualizarAutomaticamente()
        {
            try
            {
                CargarMovimientos();
                lblUltimaActualizacion.Text = $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Usuario: {FormLogin.NombreUsuario}";
            }
            catch
            {
                lblUltimaActualizacion.Text = $"⚠️ Error en actualización automática - {DateTime.Now:HH:mm:ss}";
            }
        }

        // ============================================
        // BAN-50: REFRESCAR MANUAL
        // ============================================
        private void RefrescarManual()
        {
            CargarMovimientos();
            lblUltimaActualizacion.Text = $"✅ Actualizado manualmente: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            CustomMessageBox.Show("Actualizado", "Los datos se han actualizado correctamente.", MessageBoxIcon.Information);
        }

        // ============================================
        // BAN-46 y BAN-47: EXPORTACIÓN CON VISTA PREVIA
        // ============================================
        private void ExportarDatos(string formato)
        {
            try
            {
                // BAN-47: Mostrar vista previa antes de exportar
                if (!MostrarVistaPrevia())
                    return;

                // Obtener todos los datos con los filtros aplicados (sin paginación)
                string query = @"SELECT 
                    folio, fecha_hora, tipo_operacion, concepto, beneficiario, 
                    importe, moneda, estado, referencia
                FROM movimientos_financieros
                WHERE 1=1";

                if (filtroTipo != "TODOS")
                    query += $" AND tipo_operacion = '{filtroTipo}'";

                if (cmbBeneficiario != null && cmbBeneficiario.SelectedIndex > 0)
                {
                    string beneficiario = cmbBeneficiario.SelectedItem.ToString().Replace("'", "''");
                    query += $" AND beneficiario = '{beneficiario}'";
                }

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

                if (dt.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Sin Datos", "No hay datos para exportar.", MessageBoxIcon.Warning);
                    return;
                }

                string nombreArchivo = $"Movimientos_{DateTime.Now:yyyyMMdd_HHmmss}";

                switch (formato)
                {
                    case "PDF":
                        ExportarAPDF(dt, nombreArchivo);
                        break;
                    case "WORD":
                        ExportarAWord(dt, nombreArchivo);
                        break;
                    case "EXCEL":
                        ExportarAExcel(dt, nombreArchivo);
                        break;
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", $"Error al exportar.\n{ex.Message}", MessageBoxIcon.Error);
            }
        }

        // BAN-47: Vista previa antes de exportar
        private bool MostrarVistaPrevia()
        {
            try
            {
                // Obtener primeras 20 filas para vista previa
                string queryPreview = @"SELECT 
                    folio, fecha_hora, tipo_operacion, beneficiario, importe
                FROM movimientos_financieros
                WHERE 1=1";

                if (filtroTipo != "TODOS")
                    queryPreview += $" AND tipo_operacion = '{filtroTipo}'";

                if (cmbBeneficiario != null && cmbBeneficiario.SelectedIndex > 0)
                {
                    string beneficiario = cmbBeneficiario.SelectedItem.ToString().Replace("'", "''");
                    queryPreview += $" AND beneficiario = '{beneficiario}'";
                }

                if (!string.IsNullOrWhiteSpace(txtBusqueda?.Text))
                {
                    string busqueda = txtBusqueda.Text.Replace("'", "''");
                    queryPreview += $@" AND (
                        concepto ILIKE '%{busqueda}%' OR 
                        referencia ILIKE '%{busqueda}%' OR 
                        beneficiario ILIKE '%{busqueda}%'
                    )";
                }

                queryPreview += $" ORDER BY {ordenActual} LIMIT 20";

                DataTable dtPreview = Database.ExecuteQuery(queryPreview);

                Form formPreview = new Form
                {
                    Text = "Vista Previa de Exportación",
                    Size = new Size(900, 600),
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor = Color.White
                };

                Label lblInfo = new Label
                {
                    Text = $"Se exportarán los datos con los filtros aplicados.\nMostrando las primeras 20 filas:",
                    Location = new Point(20, 20),
                    Size = new Size(850, 40),
                    Font = BankTheme.BodyFont
                };

                DataGridView dgvPreview = new DataGridView
                {
                    Location = new Point(20, 70),
                    Size = new Size(850, 420),
                    DataSource = dtPreview,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };

                Button btnConfirmar = new Button
                {
                    Text = "✅ Confirmar Exportación",
                    Location = new Point(500, 510),
                    Size = new Size(180, 35),
                    BackColor = Color.FromArgb(40, 167, 69),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.OK
                };
                btnConfirmar.FlatAppearance.BorderSize = 0;

                Button btnCancelar = new Button
                {
                    Text = "❌ Cancelar",
                    Location = new Point(690, 510),
                    Size = new Size(180, 35),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.Cancel
                };
                btnCancelar.FlatAppearance.BorderSize = 0;

                formPreview.Controls.AddRange(new Control[] { lblInfo, dgvPreview, btnConfirmar, btnCancelar });

                return formPreview.ShowDialog() == DialogResult.OK;
            }
            catch
            {
                return true; // Si falla la vista previa, continuar con exportación
            }
        }

        private void ExportarAPDF(DataTable dt, string nombreArchivo)
        {
            try
            {
                string contenido = "═══════════════════════════════════════════════════════\n";
                contenido += "        REPORTE DE MOVIMIENTOS FINANCIEROS\n";
                contenido += "═══════════════════════════════════════════════════════\n\n";
                contenido += $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n";
                contenido += $"Usuario: {FormLogin.NombreUsuario}\n";
                contenido += $"Total de registros: {dt.Rows.Count}\n\n";
                contenido += "───────────────────────────────────────────────────────\n\n";

                foreach (DataRow row in dt.Rows)
                {
                    contenido += $"FOLIO: {row["folio"]}\n";
                    contenido += $"Fecha: {row["fecha_hora"]}\n";
                    contenido += $"Tipo: {row["tipo_operacion"]}\n";
                    contenido += $"Beneficiario: {row["beneficiario"]}\n";
                    contenido += $"Importe: {Convert.ToDecimal(row["importe"]):C2} {row["moneda"]}\n";
                    contenido += $"Concepto: {row["concepto"]}\n";
                    contenido += $"Estado: {row["estado"]}\n";
                    contenido += "───────────────────────────────────────────────────────\n\n";
                }

                string rutaArchivo = $"{nombreArchivo}.txt";
                System.IO.File.WriteAllText(rutaArchivo, contenido);

                CustomMessageBox.Show("Éxito", $"Archivo exportado:\n{rutaArchivo}", MessageBoxIcon.Information);
                System.Diagnostics.Process.Start("notepad.exe", rutaArchivo);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", $"Error al exportar PDF.\n{ex.Message}", MessageBoxIcon.Error);
            }
        }

        private void ExportarAWord(DataTable dt, string nombreArchivo)
        {
            try
            {
                string contenido = "REPORTE DE MOVIMIENTOS FINANCIEROS\n\n";
                contenido += $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n";
                contenido += $"Usuario: {FormLogin.NombreUsuario}\n";
                contenido += $"Total: {dt.Rows.Count} registros\n\n";
                contenido += "═══════════════════════════════════════════════════════\n\n";

                foreach (DataRow row in dt.Rows)
                {
                    contenido += $"{row["folio"]} | {row["fecha_hora"]} | {row["tipo_operacion"]} | ";
                    contenido += $"{row["beneficiario"]} | {Convert.ToDecimal(row["importe"]):C2}\n";
                }

                string rutaArchivo = $"{nombreArchivo}.doc";
                System.IO.File.WriteAllText(rutaArchivo, contenido);

                CustomMessageBox.Show("Éxito", $"Archivo exportado:\n{rutaArchivo}", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", $"Error al exportar Word.\n{ex.Message}", MessageBoxIcon.Error);
            }
        }

        private void ExportarAExcel(DataTable dt, string nombreArchivo)
        {
            try
            {
                // Exportar como CSV (compatible con Excel)
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                // Encabezados
                sb.AppendLine("Folio,Fecha,Tipo,Concepto,Beneficiario,Importe,Moneda,Estado,Referencia");

                // Datos
                foreach (DataRow row in dt.Rows)
                {
                    sb.AppendLine($"\"{row["folio"]}\",\"{row["fecha_hora"]}\",\"{row["tipo_operacion"]}\",\"{row["concepto"]}\",\"{row["beneficiario"]}\",{row["importe"]},\"{row["moneda"]}\",\"{row["estado"]}\",\"{row["referencia"]}\"");
                }

                string rutaArchivo = $"{nombreArchivo}.csv";
                System.IO.File.WriteAllText(rutaArchivo, sb.ToString());

                CustomMessageBox.Show("Éxito", $"Archivo exportado:\n{rutaArchivo}\n\nPuede abrirlo con Excel.", MessageBoxIcon.Information);
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{System.IO.Path.GetFullPath(rutaArchivo)}\"");
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", $"Error al exportar Excel.\n{ex.Message}", MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timerActualizacion?.Stop();
            timerActualizacion?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
