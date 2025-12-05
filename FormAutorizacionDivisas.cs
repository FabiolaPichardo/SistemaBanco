using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormAutorizacionDivisas : Form
    {
        private DataGridView dgvSolicitudes;
        private DateTimePicker dtpFechaInicio;
        private DateTimePicker dtpFechaFin;
        private TextBox txtBuscarID;
        private TextBox txtBuscarNombre;
        private ComboBox cmbDivisa;
        private ComboBox cmbEstado;
        private Button btnBuscar;
        private Button btnLimpiar;
        private DateTimePicker dtpExpiracion;
        private Button btnAplicarExpiracion;
        private Label lblTotalSolicitudes;
        private Button btnExportarPDF;
        private Button btnExportarWord;
        private Button btnExportarExcel;
        private Button btnConfigRoles;

        public FormAutorizacionDivisas()
        {
            InitializeComponent();
            this.Shown += FormAutorizacionDivisas_Shown;
        }

        private void FormAutorizacionDivisas_Shown(object sender, EventArgs e)
        {
            try
            {
                CargarDivisas();
                ActualizarSolicitudesExpiradas();
                CargarSolicitudes();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Inicializar",
                    $"Error al cargar datos iniciales.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Autorización de Operaciones en Divisas";
            this.ClientSize = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Panel headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1400, 90),
                BackColor = BankTheme.PrimaryBlue
            };

            HomeButton.AddToForm(this, headerPanel);

            Label lblLogo = new Label
            {
                Text = "💱",
                Location = new Point(450, 20),
                Size = new Size(50, 50),
                Font = new Font("Segoe UI", 32F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblTitulo = new Label
            {
                Text = "AUTORIZACIÓN DE OPERACIONES EN DIVISAS",
                Location = new Point(510, 30),
                Size = new Size(550, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.AddRange(new Control[] { lblLogo, lblTitulo });

            Panel filtrosPanel = BankTheme.CreateCard(20, 110, 1360, 140);

            Label lblFiltros = new Label
            {
                Text = "Filtros de Búsqueda",
                Location = new Point(20, 15),
                Size = new Size(200, 25),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            Label lblFechaInicio = new Label
            {
                Text = "Fecha Inicio:",
                Location = new Point(20, 50),
                Size = new Size(100, 20),
                Font = BankTheme.BodyFont
            };

            dtpFechaInicio = new DateTimePicker
            {
                Location = new Point(120, 48),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short
            };
            dtpFechaInicio.Value = DateTime.Now.AddMonths(-1);

            Label lblFechaFin = new Label
            {
                Text = "Fecha Fin:",
                Location = new Point(290, 50),
                Size = new Size(80, 20),
                Font = BankTheme.BodyFont
            };

            dtpFechaFin = new DateTimePicker
            {
                Location = new Point(370, 48),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short
            };

            Label lblBuscarID = new Label
            {
                Text = "ID Transacción:",
                Location = new Point(540, 50),
                Size = new Size(110, 20),
                Font = BankTheme.BodyFont
            };

            txtBuscarID = new TextBox
            {
                Location = new Point(650, 48),
                Size = new Size(150, 25),
                Font = BankTheme.BodyFont,
                PlaceholderText = "Buscar ID..."
            };
            txtBuscarID.TextChanged += (s, e) => BuscarConValidacion();

            Label lblBuscarNombre = new Label
            {
                Text = "Nombre:",
                Location = new Point(820, 50),
                Size = new Size(70, 20),
                Font = BankTheme.BodyFont
            };

            txtBuscarNombre = new TextBox
            {
                Location = new Point(890, 48),
                Size = new Size(200, 25),
                Font = BankTheme.BodyFont
            };

            Label lblDivisa = new Label
            {
                Text = "Divisa:",
                Location = new Point(20, 85),
                Size = new Size(100, 20),
                Font = BankTheme.BodyFont
            };

            cmbDivisa = new ComboBox
            {
                Location = new Point(120, 83),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = BankTheme.BodyFont
            };

            Label lblEstado = new Label
            {
                Text = "Estado:",
                Location = new Point(290, 85),
                Size = new Size(80, 20),
                Font = BankTheme.BodyFont
            };

            cmbEstado = new ComboBox
            {
                Location = new Point(370, 83),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = BankTheme.BodyFont
            };
            cmbEstado.Items.AddRange(new string[] { "Todos", "Pendiente", "En Revisión", "Autorizada", "Rechazada", "Expirada" });
            cmbEstado.SelectedIndex = 0;

            btnBuscar = new Button
            {
                Text = "🔍 Buscar",
                Location = new Point(540, 80),
                Size = new Size(120, 30),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnBuscar, true);
            btnBuscar.Click += (s, e) => BuscarConValidacion();

            btnLimpiar = new Button
            {
                Text = "🗑 Limpiar",
                Location = new Point(670, 80),
                Size = new Size(120, 30),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnLimpiar, false);
            btnLimpiar.Click += BtnLimpiar_Click;

            if (RoleManager.PuedeConfigurarRolesDivisas(FormLogin.RolUsuario))
            {
                btnConfigRoles = new Button
                {
                    Text = "⚙ Config de Roles Divisas",
                    Location = new Point(1110, 80),
                    Size = new Size(230, 30),
                    Font = BankTheme.BodyFont,
                    BackColor = Color.FromArgb(59, 130, 246),
                    ForeColor = Color.White
                };
                BankTheme.StyleButton(btnConfigRoles, false);
                btnConfigRoles.Click += (s, e) =>
                {
                    FormConfigRolesDivisas formConfig = new FormConfigRolesDivisas();
                    formConfig.ShowDialog();
                    CargarSolicitudes(); // Recargar después de configurar
                };
                filtrosPanel.Controls.Add(btnConfigRoles);
            }

            filtrosPanel.Controls.AddRange(new Control[] {
                lblFiltros, lblFechaInicio, dtpFechaInicio, lblFechaFin, dtpFechaFin,
                lblBuscarID, txtBuscarID, lblBuscarNombre, txtBuscarNombre,
                lblDivisa, cmbDivisa, lblEstado, cmbEstado, btnBuscar, btnLimpiar
            });

            Panel expiracionPanel = BankTheme.CreateCard(20, 260, 1360, 70);

            Label lblExpiracion = new Label
            {
                Text = "Seleccione tiempo de expiración:",
                Location = new Point(20, 20),
                Size = new Size(250, 25),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            dtpExpiracion = new DateTimePicker
            {
                Location = new Point(270, 18),
                Size = new Size(200, 25),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm"
            };
            dtpExpiracion.Value = DateTime.Now.AddDays(3);

            btnAplicarExpiracion = new Button
            {
                Text = "Aplicar",
                Location = new Point(490, 15),
                Size = new Size(120, 30),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnAplicarExpiracion, true);
            btnAplicarExpiracion.Click += BtnAplicarExpiracion_Click;

            Label lblInfoExpiracion = new Label
            {
                Text = "💡 Seleccione una o más solicitudes en la tabla y aplique una fecha de expiración",
                Location = new Point(690, 20),
                Size = new Size(650, 25),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };

            expiracionPanel.Controls.AddRange(new Control[] {
                lblExpiracion, dtpExpiracion, btnAplicarExpiracion, lblInfoExpiracion
            });

            dgvSolicitudes = new DataGridView
            {
                Location = new Point(20, 340),
                Size = new Size(1360, 350),
                Font = BankTheme.BodyFont,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false
            };

            dgvSolicitudes.ColumnHeadersDefaultCellStyle.BackColor = BankTheme.PrimaryBlue;
            dgvSolicitudes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSolicitudes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvSolicitudes.ColumnHeadersHeight = 40;
            dgvSolicitudes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            Panel bottomPanel = new Panel
            {
                Location = new Point(20, 700),
                Size = new Size(1360, 60),
                BackColor = Color.Transparent
            };

            lblTotalSolicitudes = new Label
            {
                Location = new Point(0, 15),
                Size = new Size(400, 30),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            btnExportarPDF = new Button
            {
                Text = "📄 PDF",
                Location = new Point(900, 10),
                Size = new Size(140, 40),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnExportarPDF, false);
            btnExportarPDF.Click += (s, e) => ExportarReporte("PDF");

            btnExportarWord = new Button
            {
                Text = "📝 Word",
                Location = new Point(1050, 10),
                Size = new Size(140, 40),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnExportarWord, false);
            btnExportarWord.Click += (s, e) => ExportarReporte("Word");

            btnExportarExcel = new Button
            {
                Text = "📊 Excel",
                Location = new Point(1200, 10),
                Size = new Size(140, 40),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnExportarExcel, false);
            btnExportarExcel.Click += (s, e) => ExportarReporte("Excel");

            bottomPanel.Controls.AddRange(new Control[] {
                lblTotalSolicitudes, btnExportarPDF, btnExportarWord, btnExportarExcel
            });

            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new Point(600, 730), // (1400 - 200) / 2 = 600, Y=730 para ventana de 800px
                Size = new Size(200, 45)
            };
            BankTheme.StyleButton(btnCerrar, false);
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                headerPanel, filtrosPanel, expiracionPanel, dgvSolicitudes, bottomPanel, btnCerrar
            });
        }

        private void CargarDivisas()
        {
            try
            {

                if (cmbDivisa == null)
                    return;

                cmbDivisa.Items.Clear();
                cmbDivisa.Items.Add("Todas");

                string query = "SELECT codigo, nombre FROM divisas WHERE activa = TRUE ORDER BY codigo";
                DataTable dt = Database.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    cmbDivisa.Items.Add($"{row["codigo"]} - {row["nombre"]}");
                }

                if (cmbDivisa.Items.Count > 0)
                    cmbDivisa.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Cargar Divisas",
                    $"No se pudieron cargar las divisas.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        private void ActualizarSolicitudesExpiradas()
        {
            try
            {
                string query = "SELECT actualizar_solicitudes_expiradas()";
                Database.ExecuteNonQuery(query);
            }
            catch
            {

            }
        }

        private void CargarSolicitudes()
        {
            try
            {

                if (dgvSolicitudes == null || dtpFechaInicio == null || dtpFechaFin == null)
                {
                    System.Diagnostics.Debug.WriteLine("Controles no inicializados");
                    return;
                }

                ActualizarSolicitudesExpiradas();

                string query = @"
                    SELECT 
                        s.id_solicitud,
                        s.id_transaccion,
                        s.descripcion,
                        u.nombre_completo AS titular,
                        d.codigo AS divisa,
                        s.tasa_cambio,
                        s.monto_mxn,
                        s.monto_divisa,
                        s.estado,
                        s.fecha_solicitud,
                        s.fecha_expiracion,
                        COALESCE(u_aut.nombre_completo, '-') AS autorizador
                    FROM solicitudes_autorizacion_divisas s
                    INNER JOIN usuarios u ON s.id_usuario_solicitante = u.id_usuario
                    INNER JOIN divisas d ON s.id_divisa = d.id_divisa
                    LEFT JOIN usuarios u_aut ON s.id_usuario_autorizador = u_aut.id_usuario
                    WHERE 1=1";

                var parametros = new System.Collections.Generic.List<NpgsqlParameter>();

                if (dtpFechaInicio != null && dtpFechaFin != null)
                {
                    query += " AND s.fecha_solicitud >= @fechaInicio AND s.fecha_solicitud <= @fechaFin";
                    parametros.Add(new NpgsqlParameter("@fechaInicio", dtpFechaInicio.Value.Date));
                    parametros.Add(new NpgsqlParameter("@fechaFin", dtpFechaFin.Value.Date.AddDays(1).AddSeconds(-1)));
                }

                if (txtBuscarID != null && !string.IsNullOrWhiteSpace(txtBuscarID.Text))
                {
                    query += " AND s.id_transaccion ILIKE @idTransaccion";
                    parametros.Add(new NpgsqlParameter("@idTransaccion", $"%{txtBuscarID.Text}%"));
                }

                if (txtBuscarNombre != null && !string.IsNullOrWhiteSpace(txtBuscarNombre.Text))
                {
                    query += " AND u.nombre_completo ILIKE @nombre";
                    parametros.Add(new NpgsqlParameter("@nombre", $"%{txtBuscarNombre.Text}%"));
                }

                if (cmbDivisa != null && cmbDivisa.SelectedIndex > 0 && cmbDivisa.SelectedItem != null)
                {
                    string codigoDivisa = cmbDivisa.SelectedItem.ToString().Split('-')[0].Trim();
                    query += " AND d.codigo = @divisa";
                    parametros.Add(new NpgsqlParameter("@divisa", codigoDivisa));
                }

                if (cmbEstado != null && cmbEstado.SelectedIndex > 0 && cmbEstado.SelectedItem != null)
                {
                    query += " AND s.estado = @estado";
                    parametros.Add(new NpgsqlParameter("@estado", cmbEstado.SelectedItem.ToString()));
                }

                query += " ORDER BY s.fecha_solicitud DESC";

                DataTable dt = Database.ExecuteQuery(query, parametros.ToArray());

                if (dgvSolicitudes != null)
                {
                    dgvSolicitudes.DataSource = dt;

                    if (dt.Columns.Count > 0)
                    {
                        ConfigurarColumnas();
                    }
                }

                if (lblTotalSolicitudes != null)
                {
                    if (dt.Rows.Count == 0)
                        lblTotalSolicitudes.Text = "No se encontraron solicitudes. La tabla está lista para mostrar datos cuando existan.";
                    else
                        lblTotalSolicitudes.Text = $"Total de solicitudes: {dt.Rows.Count}";
                }
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("does not exist") || ex.Message.Contains("no existe"))
                {

                    DataTable dtVacia = new DataTable();
                    dtVacia.Columns.Add("id_solicitud", typeof(int));
                    dtVacia.Columns.Add("id_transaccion", typeof(string));
                    dtVacia.Columns.Add("descripcion", typeof(string));
                    dtVacia.Columns.Add("titular", typeof(string));
                    dtVacia.Columns.Add("divisa", typeof(string));
                    dtVacia.Columns.Add("tasa_cambio", typeof(decimal));
                    dtVacia.Columns.Add("monto_mxn", typeof(decimal));
                    dtVacia.Columns.Add("monto_divisa", typeof(decimal));
                    dtVacia.Columns.Add("estado", typeof(string));
                    dtVacia.Columns.Add("fecha_solicitud", typeof(DateTime));
                    dtVacia.Columns.Add("fecha_expiracion", typeof(DateTime));
                    dtVacia.Columns.Add("autorizador", typeof(string));

                    if (dgvSolicitudes != null)
                    {
                        dgvSolicitudes.DataSource = dtVacia;
                        ConfigurarColumnas();
                    }

                    if (lblTotalSolicitudes != null)
                        lblTotalSolicitudes.Text = "No hay solicitudes registradas. La tabla está lista para mostrar datos.";
                }
                else
                {

                    string errorMsg = $"Error al cargar solicitudes.\n\n";
                    errorMsg += $"Mensaje: {ex.Message}\n\n";
                    errorMsg += $"Tipo: {ex.GetType().Name}\n\n";
                    if (ex.InnerException != null)
                        errorMsg += $"Error interno: {ex.InnerException.Message}\n\n";

                    CustomMessageBox.Show("Error al Cargar Solicitudes", errorMsg, MessageBoxIcon.Error);
                }
            }
        }

        private void ConfigurarColumnas()
        {
            try
            {
                if (dgvSolicitudes == null || dgvSolicitudes.Columns.Count == 0) return;

                if (dgvSolicitudes.Columns.Contains("id_solicitud"))
                    dgvSolicitudes.Columns["id_solicitud"].Visible = false;

                if (dgvSolicitudes.Columns.Contains("id_transaccion"))
                {
                    dgvSolicitudes.Columns["id_transaccion"].HeaderText = "ID Transacción";
                    dgvSolicitudes.Columns["id_transaccion"].Width = 120;
                }

                if (dgvSolicitudes.Columns.Contains("descripcion"))
                {
                    dgvSolicitudes.Columns["descripcion"].HeaderText = "Descripción";
                    dgvSolicitudes.Columns["descripcion"].Width = 200;
                }

                if (dgvSolicitudes.Columns.Contains("titular"))
                {
                    dgvSolicitudes.Columns["titular"].HeaderText = "Titular";
                    dgvSolicitudes.Columns["titular"].Width = 150;
                }

                if (dgvSolicitudes.Columns.Contains("divisa"))
                {
                    dgvSolicitudes.Columns["divisa"].HeaderText = "Divisa";
                    dgvSolicitudes.Columns["divisa"].Width = 70;
                }

                if (dgvSolicitudes.Columns.Contains("tasa_cambio"))
                {
                    dgvSolicitudes.Columns["tasa_cambio"].HeaderText = "Tasa Cambio";
                    dgvSolicitudes.Columns["tasa_cambio"].Width = 90;
                    dgvSolicitudes.Columns["tasa_cambio"].DefaultCellStyle.Format = "N4";
                }

                if (dgvSolicitudes.Columns.Contains("monto_mxn"))
                {
                    dgvSolicitudes.Columns["monto_mxn"].HeaderText = "Monto MXN";
                    dgvSolicitudes.Columns["monto_mxn"].Width = 110;
                    dgvSolicitudes.Columns["monto_mxn"].DefaultCellStyle.Format = "C2";
                }

                if (dgvSolicitudes.Columns.Contains("monto_divisa"))
                {
                    dgvSolicitudes.Columns["monto_divisa"].HeaderText = "Monto Divisa";
                    dgvSolicitudes.Columns["monto_divisa"].Width = 110;
                    dgvSolicitudes.Columns["monto_divisa"].DefaultCellStyle.Format = "N2";
                }

                if (dgvSolicitudes.Columns.Contains("estado"))
                {
                    dgvSolicitudes.Columns["estado"].HeaderText = "Estado";
                    dgvSolicitudes.Columns["estado"].Width = 100;
                }

                if (dgvSolicitudes.Columns.Contains("fecha_solicitud"))
                {
                    dgvSolicitudes.Columns["fecha_solicitud"].HeaderText = "Fecha Solicitud";
                    dgvSolicitudes.Columns["fecha_solicitud"].Width = 130;
                    dgvSolicitudes.Columns["fecha_solicitud"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }

                if (dgvSolicitudes.Columns.Contains("fecha_expiracion"))
                {
                    dgvSolicitudes.Columns["fecha_expiracion"].HeaderText = "Fecha Expiración";
                    dgvSolicitudes.Columns["fecha_expiracion"].Width = 130;
                    dgvSolicitudes.Columns["fecha_expiracion"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }

                if (dgvSolicitudes.Columns.Contains("autorizador"))
                {
                    dgvSolicitudes.Columns["autorizador"].HeaderText = "Autorizador";
                    dgvSolicitudes.Columns["autorizador"].Width = 150;
                }

                if (RoleManager.PuedeAutorizarDivisas(FormLogin.RolUsuario))
                {
                    if (!dgvSolicitudes.Columns.Contains("Acciones"))
                    {
                        DataGridViewButtonColumn btnAcciones = new DataGridViewButtonColumn
                        {
                            Name = "Acciones",
                            HeaderText = "Acciones",
                            Text = "Ver Detalles",
                            UseColumnTextForButtonValue = true,
                            Width = 120
                        };
                        dgvSolicitudes.Columns.Add(btnAcciones);
                    }

                    dgvSolicitudes.CellContentClick -= DgvSolicitudes_CellContentClick; // Evitar duplicados
                    dgvSolicitudes.CellContentClick += DgvSolicitudes_CellContentClick;
                }

                dgvSolicitudes.CellFormatting += (s, e) =>
                {
                    if (dgvSolicitudes.Columns.Contains("estado") && 
                        e.ColumnIndex == dgvSolicitudes.Columns["estado"].Index && 
                        e.Value != null)
                    {
                        string estado = e.Value.ToString();
                        switch (estado)
                        {
                            case "Pendiente":
                                e.CellStyle.BackColor = Color.FromArgb(255, 243, 205);
                                e.CellStyle.ForeColor = Color.FromArgb(146, 64, 14);
                                break;
                            case "En Revisión":
                                e.CellStyle.BackColor = Color.FromArgb(219, 234, 254);
                                e.CellStyle.ForeColor = Color.FromArgb(30, 64, 175);
                                break;
                            case "Autorizada":
                                e.CellStyle.BackColor = Color.FromArgb(220, 252, 231);
                                e.CellStyle.ForeColor = Color.FromArgb(22, 101, 52);
                                break;
                            case "Rechazada":
                                e.CellStyle.BackColor = Color.FromArgb(254, 226, 226);
                                e.CellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                                break;
                            case "Expirada":
                                e.CellStyle.BackColor = Color.FromArgb(229, 229, 229);
                                e.CellStyle.ForeColor = Color.FromArgb(64, 64, 64);
                                break;
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ConfigurarColumnas: {ex.Message}");

            }
        }

        private void DgvSolicitudes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvSolicitudes.Columns[e.ColumnIndex].Name == "Acciones")
            {
                int idSolicitud = Convert.ToInt32(dgvSolicitudes.Rows[e.RowIndex].Cells["id_solicitud"].Value);
                MostrarDetallesSolicitud(idSolicitud);
            }
        }

        private void MostrarDetallesSolicitud(int idSolicitud)
        {
            FormDetalleSolicitudDivisa formDetalle = new FormDetalleSolicitudDivisa(idSolicitud);
            formDetalle.ShowDialog();
            CargarSolicitudes(); // Recargar después de procesar
        }

        private void BuscarConValidacion()
        {

            if (dtpFechaInicio.Value > dtpFechaFin.Value)
            {
                CustomMessageBox.Show("Fechas Inválidas",
                    "La fecha de inicio no puede ser posterior a la fecha fin.",
                    MessageBoxIcon.Warning);
                return;
            }

            CargarSolicitudes();
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            dtpFechaInicio.Value = DateTime.Now.AddMonths(-1);
            dtpFechaFin.Value = DateTime.Now;
            txtBuscarID.Clear();
            txtBuscarNombre.Clear();
            cmbDivisa.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 0;
            CargarSolicitudes();
        }

        private void BtnAplicarExpiracion_Click(object sender, EventArgs e)
        {
            if (dgvSolicitudes.SelectedRows.Count == 0)
            {
                CustomMessageBox.Show("Selección Requerida",
                    "Por favor, seleccione al menos una solicitud para aplicar la fecha de expiración.",
                    MessageBoxIcon.Warning);
                return;
            }

            if (dtpExpiracion.Value <= DateTime.Now)
            {
                CustomMessageBox.Show("Fecha Inválida",
                    "La fecha de expiración debe ser posterior a la fecha actual.",
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int actualizadas = 0;
                int noElegibles = 0;

                foreach (DataGridViewRow row in dgvSolicitudes.SelectedRows)
                {
                    int idSolicitud = Convert.ToInt32(row.Cells["id_solicitud"].Value);
                    string estado = row.Cells["estado"].Value.ToString();

                    if (estado == "Pendiente" || estado == "En Revisión")
                    {
                        string query = @"UPDATE solicitudes_autorizacion_divisas 
                                       SET fecha_expiracion = @fechaExpiracion 
                                       WHERE id_solicitud = @idSolicitud";

                        Database.ExecuteNonQuery(query,
                            new NpgsqlParameter("@fechaExpiracion", dtpExpiracion.Value),
                            new NpgsqlParameter("@idSolicitud", idSolicitud));

                        actualizadas++;
                    }
                    else
                    {
                        noElegibles++;
                    }
                }

                string mensaje = $"Se aplicó la fecha de expiración a {actualizadas} solicitud(es).";
                if (noElegibles > 0)
                {
                    mensaje += $"\n\n{noElegibles} solicitud(es) no son elegibles (solo se puede aplicar a solicitudes Pendientes o En Revisión).";
                }

                CustomMessageBox.Show("Expiración Aplicada", mensaje, MessageBoxIcon.Information);

                CargarSolicitudes();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Aplicar Expiración",
                    $"No se pudo aplicar la fecha de expiración.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        private void ExportarReporte(string formato)
        {
            try
            {
                if (dgvSolicitudes.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Sin Datos",
                        "No hay solicitudes para exportar.",
                        MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                string contenido = "";

                switch (formato)
                {
                    case "PDF":
                        saveDialog.Filter = "Archivo HTML (*.html)|*.html";
                        saveDialog.FileName = $"Autorizacion_Divisas_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                        saveDialog.Title = "Exportar a PDF (se abrirá en navegador)";

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            contenido = GenerarHTMLDivisas();
                            System.IO.File.WriteAllText(saveDialog.FileName, contenido);
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveDialog.FileName) { UseShellExecute = true });
                            CustomMessageBox.Show("Exportación Exitosa",
                                "Archivo HTML generado. Se abrirá en su navegador.\nDesde ahí puede guardarlo como PDF usando Ctrl+P.",
                                MessageBoxIcon.Information);
                        }
                        break;

                    case "Word":
                        saveDialog.Filter = "Documento Word (*.doc)|*.doc";
                        saveDialog.FileName = $"Autorizacion_Divisas_{DateTime.Now:yyyyMMdd_HHmmss}.doc";

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            contenido = GenerarWordDivisas();
                            System.IO.File.WriteAllText(saveDialog.FileName, contenido);
                            CustomMessageBox.Show("Exportación Exitosa",
                                $"Documento Word generado exitosamente en:\n{saveDialog.FileName}",
                                MessageBoxIcon.Information);
                        }
                        break;

                    case "Excel":
                        saveDialog.Filter = "Archivo CSV (*.csv)|*.csv";
                        saveDialog.FileName = $"Autorizacion_Divisas_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            contenido = GenerarCSVDivisas();
                            System.IO.File.WriteAllText(saveDialog.FileName, contenido);
                            CustomMessageBox.Show("Exportación Exitosa",
                                $"Archivo CSV generado exitosamente en:\n{saveDialog.FileName}\n\nPuede abrirlo con Excel.",
                                MessageBoxIcon.Information);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error al Exportar {formato}",
                    $"No se pudo exportar el reporte.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        private string GenerarHTMLDivisas()
        {
            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Autorización de Divisas</title>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 40px; }}
        h1 {{ color: #1e40af; text-align: center; }}
        .info {{ background: #f3f4f6; padding: 15px; margin: 20px 0; border-radius: 8px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 20px 0; font-size: 11px; }}
        th {{ background: #1e40af; color: white; padding: 10px; text-align: left; }}
        td {{ padding: 8px; border-bottom: 1px solid #e5e7eb; }}
        tr:nth-child(even) {{ background: #f9fafb; }}
        .footer {{ text-align: center; margin-top: 30px; color: #6b7280; font-size: 12px; }}
    </style>
</head>
<body>
    <h1>💱 Autorización de Operaciones en Divisas</h1>
    <div class='info'>
        <strong>Usuario:</strong> {FormLogin.NombreUsuario} ({FormLogin.RolUsuario})<br>
        <strong>Fecha de generación:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}<br>
        <strong>Total de solicitudes:</strong> {dgvSolicitudes.Rows.Count}
    </div>
    <table>
        <tr>
            <th>ID Transacción</th>
            <th>Descripción</th>
            <th>Titular</th>
            <th>Divisa</th>
            <th>Tasa</th>
            <th>Monto MXN</th>
            <th>Monto Divisa</th>
            <th>Estado</th>
            <th>Fecha</th>
        </tr>";

            foreach (DataGridViewRow row in dgvSolicitudes.Rows)
            {
                html += $@"
        <tr>
            <td>{row.Cells["id_transaccion"].Value}</td>
            <td>{row.Cells["descripcion"].Value}</td>
            <td>{row.Cells["titular"].Value}</td>
            <td>{row.Cells["divisa"].Value}</td>
            <td>{Convert.ToDecimal(row.Cells["tasa_cambio"].Value):N4}</td>
            <td>${Convert.ToDecimal(row.Cells["monto_mxn"].Value):N2}</td>
            <td>{Convert.ToDecimal(row.Cells["monto_divisa"].Value):N2}</td>
            <td>{row.Cells["estado"].Value}</td>
            <td>{Convert.ToDateTime(row.Cells["fecha_solicitud"].Value):dd/MM/yyyy}</td>
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

        private string GenerarWordDivisas()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("MÓDULO BANCO - AUTORIZACIÓN DE OPERACIONES EN DIVISAS");
            sb.AppendLine("=======================================================");
            sb.AppendLine();
            sb.AppendLine($"Usuario: {FormLogin.NombreUsuario} ({FormLogin.RolUsuario})");
            sb.AppendLine($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Total de solicitudes: {dgvSolicitudes.Rows.Count}");
            sb.AppendLine();
            sb.AppendLine(new string('-', 100));
            sb.AppendLine();

            foreach (DataGridViewRow row in dgvSolicitudes.Rows)
            {
                sb.AppendLine($"ID Transacción: {row.Cells["id_transaccion"].Value}");
                sb.AppendLine($"Descripción: {row.Cells["descripcion"].Value}");
                sb.AppendLine($"Titular: {row.Cells["titular"].Value}");
                sb.AppendLine($"Divisa: {row.Cells["divisa"].Value}");
                sb.AppendLine($"Tasa de Cambio: {Convert.ToDecimal(row.Cells["tasa_cambio"].Value):N4}");
                sb.AppendLine($"Monto MXN: ${Convert.ToDecimal(row.Cells["monto_mxn"].Value):N2}");
                sb.AppendLine($"Monto Divisa: {Convert.ToDecimal(row.Cells["monto_divisa"].Value):N2}");
                sb.AppendLine($"Estado: {row.Cells["estado"].Value}");
                sb.AppendLine($"Fecha Solicitud: {Convert.ToDateTime(row.Cells["fecha_solicitud"].Value):dd/MM/yyyy HH:mm}");
                sb.AppendLine($"Autorizador: {row.Cells["autorizador"].Value}");
                sb.AppendLine(new string('-', 100));
            }

            sb.AppendLine();
            sb.AppendLine("© 2025 Módulo Banco - Documento Confidencial");

            return sb.ToString();
        }

        private string GenerarCSVDivisas()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("# AUTORIZACIÓN DE OPERACIONES EN DIVISAS");
            sb.AppendLine($"# Usuario: {FormLogin.NombreUsuario} ({FormLogin.RolUsuario})");
            sb.AppendLine($"# Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"# Total de solicitudes: {dgvSolicitudes.Rows.Count}");
            sb.AppendLine();
            sb.AppendLine("ID Transacción,Descripción,Titular,Divisa,Tasa Cambio,Monto MXN,Monto Divisa,Estado,Fecha Solicitud,Autorizador");

            foreach (DataGridViewRow row in dgvSolicitudes.Rows)
            {
                sb.Append($"\"{row.Cells["id_transaccion"].Value}\",");
                sb.Append($"\"{row.Cells["descripcion"].Value}\",");
                sb.Append($"\"{row.Cells["titular"].Value}\",");
                sb.Append($"{row.Cells["divisa"].Value},");
                sb.Append($"{Convert.ToDecimal(row.Cells["tasa_cambio"].Value):N4},");
                sb.Append($"{Convert.ToDecimal(row.Cells["monto_mxn"].Value):N2},");
                sb.Append($"{Convert.ToDecimal(row.Cells["monto_divisa"].Value):N2},");
                sb.Append($"\"{row.Cells["estado"].Value}\",");
                sb.Append($"{Convert.ToDateTime(row.Cells["fecha_solicitud"].Value):dd/MM/yyyy HH:mm},");
                sb.AppendLine($"\"{row.Cells["autorizador"].Value}\"");
            }

            return sb.ToString();
        }

        private string GenerarContenidoReporte()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("MÓDULO BANCO - REPORTE DE AUTORIZACIÓN DE DIVISAS");
            sb.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Usuario: {FormLogin.NombreUsuario} ({FormLogin.RolUsuario})");
            sb.AppendLine(new string('=', 100));
            sb.AppendLine();

            foreach (DataGridViewRow row in dgvSolicitudes.Rows)
            {
                sb.AppendLine($"ID Transacción: {row.Cells["id_transaccion"].Value}");
                sb.AppendLine($"Descripción: {row.Cells["descripcion"].Value}");
                sb.AppendLine($"Titular: {row.Cells["titular"].Value}");
                sb.AppendLine($"Divisa: {row.Cells["divisa"].Value}");
                sb.AppendLine($"Tasa de Cambio: {row.Cells["tasa_cambio"].Value}");
                sb.AppendLine($"Monto MXN: ${row.Cells["monto_mxn"].Value:N2}");
                sb.AppendLine($"Monto Divisa: {row.Cells["monto_divisa"].Value:N2}");
                sb.AppendLine($"Estado: {row.Cells["estado"].Value}");
                sb.AppendLine($"Fecha Solicitud: {row.Cells["fecha_solicitud"].Value}");
                sb.AppendLine($"Autorizador: {row.Cells["autorizador"].Value}");
                sb.AppendLine(new string('-', 100));
            }

            return sb.ToString();
        }
    }
}
